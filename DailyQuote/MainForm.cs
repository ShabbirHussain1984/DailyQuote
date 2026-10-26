using System.Reflection;
using System.Text.Json;
using DailyQuote.Models;
using DailyQuote.Services;

namespace DailyQuote
{
    /// <summary>
    /// Main window. Shows one entry at a time; all reading, saving and ordering logic lives in Services.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string AppTitle = "Arabic Reader";
        private const int MaxRefsListed = 15;

        private readonly ProgressStore _store = new();
        private readonly List<string> _startupWarnings = new();
        private AppProgress _progress = new();
        private List<Entry> _entries = new();
        private EntryNavigator _navigator;
        private Entry? _current;
        private Action? _messageAction;
        private string? _missingFontNotice;
        private bool _applyingSettings;

        public MainForm()
        {
            InitializeComponent();
            ApplyReadingFonts();
            LoadSavedState();
            _navigator = CreateNavigator();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Applied after DPI auto-scaling so the saved pixel size is not scaled a second time.
            RestoreWindowPlacement(_progress.Settings.Window);
            ShowEntry(_navigator.GetNext(null));
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            foreach (var warning in _startupWarnings)
            {
                ShowWarning(AppTitle, warning);
            }
            _startupWarnings.Clear();

            if (_missingFontNotice != null && !_progress.Settings.FontNoticeShown)
            {
                MessageBox.Show(this, _missingFontNotice, "Recommended Fonts",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _progress.Settings.FontNoticeShown = true;
                TrySaveProgress();
            }

            // No cached entries (e.g. entries.json was deleted): reload the last Excel file if it still exists.
            if (_entries.Count == 0 && File.Exists(_progress.LastExcelPath))
            {
                LoadExcel(_progress.LastExcelPath!);
            }
        }

        /// <summary>
        /// Enter is consumed by a focused button before KeyDown fires, so it is handled here.
        /// Other shortcuts go through KeyPreview / KeyDown (S) and the menu (Ctrl+O).
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && !orderComboBox.DroppedDown)
            {
                MarkCurrentAsRead();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.None)
            {
                SkipCurrent();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
            _progress.Settings.Window = new WindowPlacement
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height,
                Maximized = WindowState == FormWindowState.Maximized
            };
            TrySaveProgress();
        }

        #region Startup

        private void LoadSavedState()
        {
            try
            {
                var progressResult = _store.LoadProgress();
                _progress = progressResult.Value;
                AddStartupWarning(progressResult.Warning);

                var entriesResult = _store.LoadEntries();
                _entries = CleanEntries(entriesResult.Value);
                AddStartupWarning(entriesResult.Warning);
            }
            catch (Exception ex)
            {
                _startupWarnings.Add($"Saved data could not be loaded from:\n{_store.DataFolder}\n\n{ex.Message}");
            }

            _applyingSettings = true;
            showEnglishCheckBox.Checked = _progress.Settings.ShowEnglish;
            showUrduCheckBox.Checked = _progress.Settings.ShowUrdu;
            orderComboBox.SelectedIndex = _progress.Settings.Order == OrderMode.Random ? 1 : 0;
            _applyingSettings = false;
        }

        private void AddStartupWarning(string? warning)
        {
            if (warning != null)
            {
                _startupWarnings.Add(warning);
            }
        }

        /// <summary>Guards against a hand-edited entries.json: no nulls, trimmed unique Refs.</summary>
        private static List<Entry> CleanEntries(IEnumerable<Entry?> entries)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<Entry>();
            foreach (var entry in entries)
            {
                if (entry == null)
                {
                    continue;
                }
                entry.Ref = AppProgress.NormalizeRef(entry.Ref);
                entry.Arabic ??= "";
                entry.English ??= "";
                entry.Urdu ??= "";
                if (entry.Ref.Length > 0 && seen.Add(entry.Ref))
                {
                    result.Add(entry);
                }
            }
            return result;
        }

        private void ApplyReadingFonts()
        {
            var arabicFamily = FontResolver.FirstInstalled(FontResolver.ArabicFamilies);
            var urduFamily = FontResolver.FirstInstalled(FontResolver.UrduFamilies);

            arabicTextBox.Font = new Font(arabicFamily ?? FontResolver.FallbackFamily, 26F);
            englishTextBox.Font = new Font(FontResolver.FallbackFamily, 12F);
            urduTextBox.Font = new Font(urduFamily ?? FontResolver.FallbackFamily, 18F);

            var missing = new List<string>();
            if (arabicFamily == null)
            {
                missing.Add("Arabic: " + string.Join(", ", FontResolver.ArabicFamilies));
            }
            if (urduFamily == null)
            {
                missing.Add("Urdu: " + string.Join(", ", FontResolver.UrduFamilies));
            }
            if (missing.Count > 0)
            {
                _missingFontNotice =
                    $"For the best reading experience, install one of these fonts:\n\n{string.Join("\n", missing)}\n\n" +
                    $"\"{FontResolver.FallbackFamily}\" is being used for now. " +
                    "Amiri, Scheherazade New and Noto Nastaliq Urdu are free to download. " +
                    "Restart the app after installing a font.";
            }
        }

        private void RestoreWindowPlacement(WindowPlacement? placement)
        {
            if (placement == null || placement.Width <= 0 || placement.Height <= 0)
            {
                return;
            }

            var bounds = new Rectangle(placement.X, placement.Y,
                Math.Max(placement.Width, MinimumSize.Width), Math.Max(placement.Height, MinimumSize.Height));

            // Ignore positions on a monitor that is no longer connected.
            if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(bounds)))
            {
                StartPosition = FormStartPosition.Manual;
                Bounds = bounds;
            }
            if (placement.Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        #endregion

        #region Display

        private EntryNavigator CreateNavigator() =>
            new(_entries, _progress.IsRead) { Mode = _progress.Settings.Order };

        /// <summary>Shows the entry, or the "open a file" / "all read" message when it is null.</summary>
        private void ShowEntry(Entry? entry)
        {
            _current = entry;
            statusLabel.Text = "";

            if (_entries.Count == 0)
            {
                ShowMessage("Open an Excel file to start reading.\n\n" +
                            "The first row must have the columns: Arabic, English, Urdu, Ref.",
                            "Open Excel File...", OpenExcel);
            }
            else if (entry == null)
            {
                ShowMessage($"You have read all {_entries.Count} entries.",
                            "Reset Progress...", ResetProgress);
            }
            else
            {
                messagePanel.Visible = false;
                readingPanel.Visible = true;

                readingPanel.SuspendLayout();
                UpdateTranslationVisibility();
                arabicTextBox.SetText(entry.Arabic);
                englishTextBox.SetText(entry.English);
                urduTextBox.SetText(entry.Urdu);
                refLabel.Text = entry.Ref;
                readingPanel.ResumeLayout(true);
                readingPanel.AutoScrollPosition = Point.Empty;
            }

            markReadButton.Enabled = entry != null;
            skipButton.Enabled = entry != null;
            UpdateProgress();
        }

        private void ShowMessage(string text, string buttonText, Action action)
        {
            readingPanel.Visible = false;
            messageLabel.Text = text;
            messageButton.Text = buttonText;
            _messageAction = action;
            messagePanel.Visible = true;
            UpdateMessageLabelWidth();
        }

        private void UpdateTranslationVisibility()
        {
            // Boxes with no text for this entry are hidden too, so no empty gaps appear.
            englishTextBox.Visible = showEnglishCheckBox.Checked && _current?.English.Length > 0;
            urduTextBox.Visible = showUrduCheckBox.Checked && _current?.Urdu.Length > 0;
            arabicTextBox.Visible = _current?.Arabic.Length > 0;
        }

        private void UpdateProgress()
        {
            int total = _navigator.TotalCount;
            int read = _navigator.ReadCount;
            progressLabel.Text = $"Read {read} of {total}";
            progressBar.Maximum = Math.Max(total, 1);
            progressBar.Value = Math.Min(read, progressBar.Maximum);
        }

        private void UpdateMessageLabelWidth()
        {
            messageLabel.MaximumSize = new Size(Math.Max(100, messagePanel.ClientSize.Width - 40), 0);
        }

        private void messagePanel_Resize(object? sender, EventArgs e) => UpdateMessageLabelWidth();

        private void messageButton_Click(object? sender, EventArgs e) => _messageAction?.Invoke();

        #endregion

        #region Reading

        private void MarkCurrentAsRead()
        {
            if (_current == null)
            {
                return;
            }

            _progress.MarkRead(_current.Ref);
            // Saved immediately so progress survives a crash or power loss.
            TrySaveProgress();
            ShowEntry(_navigator.GetNext(_current));
        }

        private void SkipCurrent()
        {
            if (_current == null)
            {
                return;
            }

            var next = _navigator.GetNext(_current);
            if (ReferenceEquals(next, _current))
            {
                statusLabel.Text = "This is the only unread entry left.";
                return;
            }
            ShowEntry(next);
        }

        private void markReadButton_Click(object? sender, EventArgs e) => MarkCurrentAsRead();

        private void skipButton_Click(object? sender, EventArgs e) => SkipCurrent();

        private void showEnglishCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateTranslationVisibility();
            if (!_applyingSettings)
            {
                _progress.Settings.ShowEnglish = showEnglishCheckBox.Checked;
                TrySaveProgress();
            }
        }

        private void showUrduCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateTranslationVisibility();
            if (!_applyingSettings)
            {
                _progress.Settings.ShowUrdu = showUrduCheckBox.Checked;
                TrySaveProgress();
            }
        }

        private void orderComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var mode = orderComboBox.SelectedIndex == 1 ? OrderMode.Random : OrderMode.Sequential;
            if (_navigator != null)
            {
                _navigator.Mode = mode;
            }
            if (!_applyingSettings)
            {
                _progress.Settings.Order = mode;
                TrySaveProgress();
            }
        }

        #endregion

        #region File menu

        private void openExcelMenuItem_Click(object? sender, EventArgs e) => OpenExcel();

        private void exportProgressMenuItem_Click(object? sender, EventArgs e) => ExportProgress();

        private void importProgressMenuItem_Click(object? sender, EventArgs e) => ImportProgress();

        private void exitMenuItem_Click(object? sender, EventArgs e) => Close();

        private void resetProgressMenuItem_Click(object? sender, EventArgs e) => ResetProgress();

        private void aboutMenuItem_Click(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
            MessageBox.Show(this,
                $"{AppTitle} {version}\n\n" +
                "Read Arabic text with English and Urdu translations, one entry at a time.\n\n" +
                "Shortcuts:\n  Enter – Mark as Read\n  S – Skip\n  Ctrl+O – Open Excel\n\n" +
                $"Your progress is saved in:\n{_store.DataFolder}",
                "About " + AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenExcel()
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Open Excel File",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                CheckFileExists = true
            };
            var lastFolder = Path.GetDirectoryName(_progress.LastExcelPath ?? "");
            if (!string.IsNullOrEmpty(lastFolder) && Directory.Exists(lastFolder))
            {
                dialog.InitialDirectory = lastFolder;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadExcel(dialog.FileName);
            }
        }

        /// <summary>
        /// Reads an Excel file, replaces the cached entries and shows the first unread entry.
        /// Read Refs are kept, so only new Refs appear as unread.
        /// </summary>
        private bool LoadExcel(string path)
        {
            ExcelReadResult result;
            try
            {
                UseWaitCursor = true;
                Application.DoEvents();
                result = ExcelReader.Read(path);
            }
            catch (ExcelFormatException ex)
            {
                ShowError("Invalid Excel File", ex.Message);
                return false;
            }
            catch (FileNotFoundException)
            {
                ShowError("File Not Found", $"The file was not found:\n{path}");
                return false;
            }
            catch (IOException ex)
            {
                ShowError("Cannot Open File",
                    $"The file could not be opened. If another program is using it, close it and try again.\n\n{ex.Message}");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError("Access Denied", $"You do not have permission to read this file.\n\n{ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // ClosedXML/OpenXML throw several exception types for damaged or non-xlsx files.
                ShowError("Cannot Read File",
                    $"The file could not be read as an Excel workbook (.xlsx). It may be damaged or in another format.\n\n{ex.Message}");
                return false;
            }
            finally
            {
                UseWaitCursor = false;
            }

            if (result.Entries.Count == 0)
            {
                ShowError("No Entries Found",
                    "No rows with text and a Ref were found below the header row.");
                return false;
            }

            var warnings = new List<string>();
            if (result.DuplicateRefs.Count > 0)
            {
                warnings.Add($"{result.DuplicateRefs.Count} duplicate Ref(s) found. Only the first row of each was kept:\n" +
                             FormatList(result.DuplicateRefs.Distinct(StringComparer.OrdinalIgnoreCase)));
            }
            if (result.RowsWithoutRef.Count > 0)
            {
                warnings.Add($"{result.RowsWithoutRef.Count} row(s) have text but no Ref and were skipped. Excel row(s):\n" +
                             FormatList(result.RowsWithoutRef.Select(r => r.ToString())));
            }
            if (warnings.Count > 0)
            {
                ShowWarning("Excel File Warnings", string.Join("\n\n", warnings));
            }

            _entries = result.Entries;
            _progress.LastExcelPath = Path.GetFullPath(path);
            try
            {
                _store.SaveEntries(_entries);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ShowWarning(AppTitle,
                    $"The entries were loaded but could not be cached, so you may need to open the file again next time.\n\n{ex.Message}");
            }
            TrySaveProgress();

            _navigator = CreateNavigator();
            ShowEntry(_navigator.GetNext(null));
            statusLabel.Text = $"Loaded {_navigator.TotalCount} entries from {Path.GetFileName(path)} " +
                               $"({_navigator.UnreadCount} unread).";
            return true;
        }

        private void ExportProgress()
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Export Progress",
                Filter = "JSON file (*.json)|*.json",
                FileName = $"ArabicReader-progress-{DateTime.Now:yyyy-MM-dd}.json",
                OverwritePrompt = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _store.ExportProgress(_progress, dialog.FileName);
                statusLabel.Text = $"Exported {_progress.ReadRefs.Count} read Ref(s) to {Path.GetFileName(dialog.FileName)}.";
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ShowError("Export Failed", $"The progress could not be exported.\n\n{ex.Message}");
            }
        }

        private void ImportProgress()
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Import Progress",
                Filter = "JSON file (*.json)|*.json|All files (*.*)|*.*",
                CheckFileExists = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            HashSet<string> imported;
            try
            {
                imported = _store.ImportReadRefs(dialog.FileName);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or JsonException)
            {
                ShowError("Import Failed", $"The file could not be imported.\n\n{ex.Message}");
                return;
            }

            if (imported.Count == 0)
            {
                ShowWarning("Import Progress", "The file does not contain any read Refs.");
                return;
            }

            int matching = _entries.Count(e => imported.Contains(e.Ref));
            var choice = MessageBox.Show(this,
                $"The backup contains {imported.Count} read Ref(s); {matching} match the loaded entries.\n\n" +
                "Yes – Merge with your current progress\n" +
                "No – Replace your current progress\n" +
                "Cancel – Do nothing",
                "Import Progress", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (choice == DialogResult.Yes)
            {
                _progress.ReadRefs.UnionWith(imported);
            }
            else if (choice == DialogResult.No)
            {
                _progress.ReadRefs = imported;
            }
            else
            {
                return;
            }

            TrySaveProgress();
            AfterReadStateChanged();
            statusLabel.Text = $"Imported {imported.Count} read Ref(s).";
        }

        private void ResetProgress()
        {
            if (_progress.ReadRefs.Count == 0)
            {
                MessageBox.Show(this, "Nothing has been marked as read yet.", "Reset Progress",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(this,
                $"This will mark all {_progress.ReadRefs.Count} read entries as unread.\n\n" +
                "Consider using File > Export Progress first to keep a backup.\n\nReset progress?",
                "Reset Progress", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes)
            {
                return;
            }

            _progress.ReadRefs.Clear();
            TrySaveProgress();
            ShowEntry(_navigator.GetNext(null));
            statusLabel.Text = "Progress has been reset.";
        }

        /// <summary>Moves on if the current entry became read (or none was shown); otherwise just refreshes progress.</summary>
        private void AfterReadStateChanged()
        {
            if (_current == null || _progress.IsRead(_current.Ref))
            {
                ShowEntry(_navigator.GetNext(null));
            }
            else
            {
                UpdateProgress();
            }
        }

        #endregion

        #region Helpers

        /// <summary>Saves progress.json; shows a friendly message if it fails. Returns true on success.</summary>
        private bool TrySaveProgress()
        {
            try
            {
                _store.SaveProgress(_progress);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ShowError("Could Not Save Progress",
                    $"Your progress could not be saved to:\n{_store.ProgressPath}\n\n{ex.Message}\n\n" +
                    "It is kept in memory and will be saved again on the next change.");
                return false;
            }
        }

        private static string FormatList(IEnumerable<string> items)
        {
            var list = items.ToList();
            var shown = string.Join(", ", list.Take(MaxRefsListed));
            return list.Count > MaxRefsListed ? $"{shown} ... and {list.Count - MaxRefsListed} more" : shown;
        }

        private void ShowError(string title, string message) =>
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void ShowWarning(string title, string message) =>
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        #endregion
    }
}
