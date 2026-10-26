using ClosedXML.Excel;
using DailyQuote.Models;

namespace DailyQuote.Services
{
    /// <summary>Thrown when the Excel file does not have the expected layout.</summary>
    public sealed class ExcelFormatException : Exception
    {
        public ExcelFormatException(string message) : base(message) { }
    }

    /// <summary>Entries read from an Excel file, plus anything the user should be warned about.</summary>
    public sealed class ExcelReadResult
    {
        public List<Entry> Entries { get; } = new();

        /// <summary>Refs that appeared more than once (only the first row was kept).</summary>
        public List<string> DuplicateRefs { get; } = new();

        /// <summary>Excel row numbers that had text but no Ref (skipped).</summary>
        public List<int> RowsWithoutRef { get; } = new();

        public string SheetName { get; set; } = "";
    }

    /// <summary>
    /// Reads entries from an .xlsx file with ClosedXML (no Microsoft Office required).
    /// </summary>
    public static class ExcelReader
    {
        public const string ArabicColumn = "Arabic";
        public const string EnglishColumn = "English";
        public const string UrduColumn = "Urdu";
        public const string RefColumn = "Ref";

        private static readonly string[] RequiredColumns = { ArabicColumn, EnglishColumn, UrduColumn, RefColumn };

        // The header row is searched for within this many used rows from the top of a sheet,
        // so a title row above the headers does not break the import.
        private const int HeaderSearchRows = 10;

        /// <summary>
        /// Reads and validates the workbook. Columns are matched by header name, not position.
        /// Empty rows are skipped; for duplicate Refs the first row wins.
        /// </summary>
        /// <exception cref="ExcelFormatException">A required column is missing.</exception>
        /// <exception cref="IOException">The file cannot be opened.</exception>
        public static ExcelReadResult Read(string path)
        {
            // FileShare.ReadWrite lets us read the file even while it is open in Excel.
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var workbook = new XLWorkbook(stream);

            Dictionary<string, int>? bestColumns = null;
            string bestSheet = "";

            foreach (var sheet in workbook.Worksheets)
            {
                var (headerRow, columns) = FindHeaderRow(sheet);
                if (headerRow > 0)
                {
                    return ReadRows(sheet, headerRow, columns);
                }
                if (bestColumns == null || columns.Count > bestColumns.Count)
                {
                    bestColumns = columns;
                    bestSheet = sheet.Name;
                }
            }

            if (bestColumns == null || bestColumns.Count == 0)
            {
                throw new ExcelFormatException(
                    "No header row was found in the workbook.\n\n" +
                    $"The first row must contain these column names: {string.Join(", ", RequiredColumns)}.");
            }

            var missing = RequiredColumns.Where(c => !bestColumns.ContainsKey(c));
            throw new ExcelFormatException(
                $"The Excel file is missing required column(s): {string.Join(", ", missing)}.\n\n" +
                $"Sheet \"{bestSheet}\" has: {string.Join(", ", bestColumns.Keys)}.\n\n" +
                $"Required columns: {string.Join(", ", RequiredColumns)}.");
        }

        /// <summary>
        /// Looks for a row containing all required headers. Returns its row number (0 if not found)
        /// and the header-to-column map of the best matching row.
        /// </summary>
        private static (int Row, Dictionary<string, int> Columns) FindHeaderRow(IXLWorksheet sheet)
        {
            var best = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var first = sheet.FirstRowUsed();
            var last = sheet.LastRowUsed();
            if (first == null || last == null)
            {
                return (0, best);
            }

            int end = Math.Min(last.RowNumber(), first.RowNumber() + HeaderSearchRows - 1);
            for (int r = first.RowNumber(); r <= end; r++)
            {
                var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in sheet.Row(r).CellsUsed())
                {
                    var name = cell.GetFormattedString().Trim();
                    if (name.Length > 0)
                    {
                        columns.TryAdd(name, cell.Address.ColumnNumber);
                    }
                }

                if (RequiredColumns.All(columns.ContainsKey))
                {
                    return (r, columns);
                }
                if (RequiredColumns.Count(columns.ContainsKey) > RequiredColumns.Count(best.ContainsKey))
                {
                    best = columns;
                }
            }
            return (0, best);
        }

        private static ExcelReadResult ReadRows(IXLWorksheet sheet, int headerRow, Dictionary<string, int> columns)
        {
            var result = new ExcelReadResult { SheetName = sheet.Name };
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int lastRow = sheet.LastRowUsed()?.RowNumber() ?? headerRow;

            for (int r = headerRow + 1; r <= lastRow; r++)
            {
                var entry = new Entry
                {
                    Arabic = CellText(sheet, r, columns[ArabicColumn]),
                    English = CellText(sheet, r, columns[EnglishColumn]),
                    Urdu = CellText(sheet, r, columns[UrduColumn]),
                    Ref = AppProgress.NormalizeRef(CellText(sheet, r, columns[RefColumn]))
                };

                bool hasText = entry.Arabic.Length > 0 || entry.English.Length > 0 || entry.Urdu.Length > 0;
                if (!hasText && entry.Ref.Length == 0)
                {
                    continue; // empty row
                }
                if (entry.Ref.Length == 0)
                {
                    result.RowsWithoutRef.Add(r);
                    continue;
                }
                if (!seen.Add(entry.Ref))
                {
                    result.DuplicateRefs.Add(entry.Ref);
                    continue;
                }
                if (!hasText)
                {
                    continue; // a Ref with no text has nothing to show
                }
                result.Entries.Add(entry);
            }
            return result;
        }

        // GetFormattedString returns what Excel displays, so a Ref such as "2:255" stays as typed
        // even if Excel stored it as a number or time.
        private static string CellText(IXLWorksheet sheet, int row, int column) =>
            sheet.Cell(row, column).GetFormattedString().Trim();
    }
}
