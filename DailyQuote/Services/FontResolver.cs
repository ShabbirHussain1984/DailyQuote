using System.Drawing.Text;

namespace DailyQuote.Services
{
    /// <summary>Picks the first installed font from a list of preferred families.</summary>
    public static class FontResolver
    {
        /// <summary>Font used when none of the preferred fonts is installed.</summary>
        public const string FallbackFamily = "Segoe UI";

        public static readonly string[] ArabicFamilies = { "Traditional Arabic", "Amiri", "Scheherazade New" };

        public static readonly string[] UrduFamilies = { "Jameel Noori Nastaleeq", "Noto Nastaliq Urdu" };

        private static readonly Lazy<HashSet<string>> Installed = new(() =>
        {
            using var fonts = new InstalledFontCollection();
            return fonts.Families.Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        });

        /// <summary>Returns true if a font family with this name is installed.</summary>
        public static bool IsInstalled(string family) => Installed.Value.Contains(family);

        /// <summary>
        /// Returns the first installed family from <paramref name="preferred"/>, or null if none is installed.
        /// </summary>
        public static string? FirstInstalled(IEnumerable<string> preferred) =>
            preferred.FirstOrDefault(IsInstalled);
    }
}
