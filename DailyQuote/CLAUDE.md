# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

"Arabic Reader": a WinForms (.NET 8, `net8.0-windows`) desktop app that loads Arabic text with English and Urdu translations from an `.xlsx` file and shows one unread entry at a time. The project, assembly and namespace are named `DailyQuote` (the original template name); the user-facing name is "Arabic Reader". The solution file `DailyQuote.slnx` is one directory up.

## Commands

Run these from this directory:

```
dotnet build                                          # build (Debug)
dotnet run                                            # run the app
dotnet build -c Release
```

- The only NuGet dependency is ClosedXML, used to read Excel files. Do not use Office Interop: the app must not need Microsoft Office installed.
- `RollForward=Major` is set in the `.csproj`, so the app also runs on machines that only have a newer .NET Desktop Runtime.
- There is no test project and no linter configuration.
- To test the services, create a separate console project (`net8.0-windows`, `UseWindowsForms=true`) with a `ProjectReference` to `DailyQuote.csproj`. Construct `ProgressStore` with a temp `dataFolder` so the real `%AppData%` files are left alone.

## Architecture

`MainForm` contains only UI code and delegates all logic to `Services/`.

- **Identity is the `Ref` column, not the row number.** Refs are normalized with `AppProgress.NormalizeRef` (trimmed) and compared with `StringComparer.OrdinalIgnoreCase` everywhere: in the read-Refs set, in duplicate detection in `ExcelReader`, and in the navigator's index. Keep all three consistent.
  - The `AppProgress.ReadRefs` setter copies and re-normalizes its input. This is how deserialized JSON gets the case-insensitive comparer back.
- **Read state lives only in `AppProgress`.**
  - `EntryNavigator` takes an `isRead` callback, `_progress.IsRead`, and never caches read state, so marking, importing or resetting takes effect immediately.
  - After `_entries` is replaced (a new Excel load), `MainForm` must call `CreateNavigator()` again.
- **Loading a new Excel file never clears read Refs.** Only `_entries` and `entries.json` are replaced, so Refs read earlier stay read and only new Refs appear as unread. Read Refs that are not in the current file are kept, but `ReadCount` doesn't count them.
- **Persistence (`ProgressStore`)** stores data in `%AppData%\ArabicReader\`:
  - `progress.json` holds the read Refs, settings and the last Excel path.
  - `entries.json` is a cache of the loaded entries, so the app opens without re-selecting the file. If the cache is missing, `OnShown` reloads `LastExcelPath`.
  - **Writes:** every write goes to a `.tmp` file first, then `File.Replace` swaps it in, keeping the previous version as `.bak`.
  - **Loads:** a damaged file falls back to `.bak`. If that also fails, the bad file is renamed to `*.corrupt-<timestamp>.json`. Loads return `LoadResult<T>(Value, Warning)` instead of throwing, and the UI shows the warning.
  - **JSON format:** camelCase property names, enums as strings, Arabic and Urdu left unescaped.
  - Progress is saved immediately on every Mark as Read and every settings change.
- **Keyboard:**
  - Enter is handled in `MainForm.ProcessCmdKey`, because a focused button consumes Enter before `KeyDown` fires.
  - S is handled through `KeyPreview` and `KeyDown`.
  - Ctrl+O is the `ShortcutKeys` of the File → Open Excel menu item.
- **Text display:** `Controls/ReadingTextBox` is a borderless, read-only `RichTextBox`.
  - It has no scroll bars and sets its own height from `ContentsResized`. It re-requests that resize (`EM_REQUESTRESIZE`) when its width changes or it becomes visible.
  - It forwards the mouse wheel to the parent `readingPanel`, which is the panel that scrolls.
  - `LineSpacing` is applied through `EM_SETPARAFORMAT`, and `SetText` re-applies it after each text change.
  - The text boxes are docked `Top` in `readingPanel`. Docked controls stack in reverse order of being added, so `refLabel` is added first and `arabicTextBox` last.
- **Layout and DPI:**
  - Only the Arabic and Urdu boxes are `RightToLeft.Yes`; the rest of the form stays left-to-right.
  - The form uses `AutoScaleMode.Dpi` with `AutoScaleDimensions` of 96, and `Program.cs` sets `PerMonitorV2`.
  - The saved window bounds are in device pixels, so they are applied in `OnLoad` after auto-scaling. Don't move this into the constructor, or the size gets scaled twice.
  - Fonts are picked at runtime by `FontResolver`, which overrides the Segoe UI placeholders in the designer.
- **`MainForm.Designer.cs`** is hand-written in designer style. Inside the form class, refer to the custom control by its full name, `DailyQuote.Controls.ReadingTextBox`, because a bare `Controls.` resolves to the form's `Controls` property.
