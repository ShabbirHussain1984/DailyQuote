using System.Text.Json.Serialization;

namespace DailyQuote.Models
{
    /// <summary>How the next unread entry is chosen.</summary>
    public enum OrderMode
    {
        Sequential,
        Random
    }

    /// <summary>Saved window position and size, in device pixels.</summary>
    public sealed class WindowPlacement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Maximized { get; set; }
    }

    /// <summary>User preferences persisted with the progress.</summary>
    public sealed class AppSettings
    {
        public bool ShowEnglish { get; set; } = true;
        public bool ShowUrdu { get; set; } = true;
        public OrderMode Order { get; set; } = OrderMode.Sequential;
        public WindowPlacement? Window { get; set; }
        public bool FontNoticeShown { get; set; }
    }

    /// <summary>
    /// Everything stored in progress.json: the read Refs, settings and the last loaded Excel file.
    /// </summary>
    public sealed class AppProgress
    {
        private HashSet<string> _readRefs = CreateRefSet();

        /// <summary>
        /// Refs that have been read. Values are trimmed and compared case-insensitively.
        /// Assigning a collection copies and normalizes it.
        /// </summary>
        public HashSet<string> ReadRefs
        {
            get => _readRefs;
            set => _readRefs = CreateRefSet(value);
        }

        public AppSettings Settings { get; set; } = new();

        public string? LastExcelPath { get; set; }

        /// <summary>Returns true if the given Ref has been marked as read.</summary>
        public bool IsRead(string? reference) => _readRefs.Contains(NormalizeRef(reference));

        /// <summary>Marks the given Ref as read. Returns false if it was already read or empty.</summary>
        public bool MarkRead(string? reference)
        {
            var key = NormalizeRef(reference);
            return key.Length > 0 && _readRefs.Add(key);
        }

        /// <summary>Trims a Ref so that "2:255 " and "2:255" are the same entry.</summary>
        public static string NormalizeRef(string? reference) => (reference ?? "").Trim();

        /// <summary>Creates a case-insensitive set of normalized Refs, ignoring empty values.</summary>
        public static HashSet<string> CreateRefSet(IEnumerable<string?>? refs = null)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (refs != null)
            {
                foreach (var r in refs)
                {
                    var key = NormalizeRef(r);
                    if (key.Length > 0)
                    {
                        set.Add(key);
                    }
                }
            }
            return set;
        }
    }

    /// <summary>File format used by Export Progress / Import Progress.</summary>
    public sealed class ProgressExport
    {
        [JsonPropertyName("app")]
        public string App { get; set; } = "ArabicReader";

        [JsonPropertyName("exportedAt")]
        public DateTime ExportedAt { get; set; }

        [JsonPropertyName("readRefs")]
        public List<string> ReadRefs { get; set; } = new();
    }
}
