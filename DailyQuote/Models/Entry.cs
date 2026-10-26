namespace DailyQuote.Models
{
    /// <summary>
    /// One row of the Excel file: Arabic text, its translations, and a unique reference.
    /// </summary>
    public sealed class Entry
    {
        /// <summary>Unique reference for the entry (e.g. "2:255" or "Bukhari 1").</summary>
        public string Ref { get; set; } = "";

        public string Arabic { get; set; } = "";

        public string English { get; set; } = "";

        public string Urdu { get; set; } = "";
    }
}
