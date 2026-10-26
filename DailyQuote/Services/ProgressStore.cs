using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DailyQuote.Models;

namespace DailyQuote.Services
{
    /// <summary>A loaded value plus an optional message the user should see (e.g. a recovered file).</summary>
    public sealed record LoadResult<T>(T Value, string? Warning);

    /// <summary>
    /// Loads and saves progress.json and entries.json in %AppData%\ArabicReader\.
    /// Every write goes to a temp file first and then replaces the target, so a crash
    /// mid-write never leaves a half-written JSON file.
    /// </summary>
    public sealed class ProgressStore
    {
        private const string ProgressFileName = "progress.json";
        private const string EntriesFileName = "entries.json";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            // Keep Arabic and Urdu readable in the JSON instead of \uXXXX escapes.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>Folder where all data files are stored.</summary>
        public string DataFolder { get; }

        public string ProgressPath => Path.Combine(DataFolder, ProgressFileName);

        public string EntriesPath => Path.Combine(DataFolder, EntriesFileName);

        /// <summary>Creates a store in %AppData%\ArabicReader, or in <paramref name="dataFolder"/> if given.</summary>
        public ProgressStore(string? dataFolder = null)
        {
            DataFolder = dataFolder ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArabicReader");
        }

        /// <summary>
        /// Loads progress.json. Returns defaults if it does not exist. If it is corrupted, the
        /// backup from the previous save is used; if that fails too, the bad file is kept aside
        /// and defaults are returned with a warning.
        /// </summary>
        public LoadResult<AppProgress> LoadProgress() =>
            LoadWithRecovery(ProgressPath, () => new AppProgress(), "progress");

        /// <summary>Saves progress.json atomically.</summary>
        /// <exception cref="IOException">The file could not be written.</exception>
        public void SaveProgress(AppProgress progress) => WriteJson(ProgressPath, progress);

        /// <summary>Loads the cached entries from entries.json (empty if there is no cache).</summary>
        public LoadResult<List<Entry>> LoadEntries() =>
            LoadWithRecovery(EntriesPath, () => new List<Entry>(), "entries cache");

        /// <summary>Saves the cached entries to entries.json atomically.</summary>
        /// <exception cref="IOException">The file could not be written.</exception>
        public void SaveEntries(IReadOnlyList<Entry> entries) => WriteJson(EntriesPath, entries);

        /// <summary>Writes the read Refs to a backup file chosen by the user.</summary>
        public void ExportProgress(AppProgress progress, string path)
        {
            var export = new ProgressExport
            {
                ExportedAt = DateTime.Now,
                ReadRefs = progress.ReadRefs.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList()
            };
            WriteJson(path, export, keepBackup: false);
        }

        /// <summary>
        /// Reads Refs from an exported backup. Also accepts a copy of progress.json or a plain JSON array of Refs.
        /// </summary>
        /// <exception cref="InvalidDataException">The file is not a recognised progress backup.</exception>
        public HashSet<string> ImportReadRefs(string path)
        {
            var json = File.ReadAllText(path);
            try
            {
                using var doc = JsonDocument.Parse(json, new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                });

                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    return AppProgress.CreateRefSet(root.Deserialize<List<string>>(JsonOptions));
                }
                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in root.EnumerateObject())
                    {
                        if (property.Name.Equals("readRefs", StringComparison.OrdinalIgnoreCase))
                        {
                            return AppProgress.CreateRefSet(property.Value.Deserialize<List<string>>(JsonOptions));
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("The file is not valid JSON: " + ex.Message, ex);
            }
            throw new InvalidDataException("The file does not contain a \"readRefs\" list.");
        }

        private LoadResult<T> LoadWithRecovery<T>(string path, Func<T> createDefault, string description)
        {
            if (!File.Exists(path))
            {
                return new LoadResult<T>(createDefault(), null);
            }

            if (TryRead(path, out T? value, out var error))
            {
                return new LoadResult<T>(value!, null);
            }

            var backup = BackupPath(path);
            if (File.Exists(backup) && TryRead(backup, out value, out _))
            {
                return new LoadResult<T>(value!,
                    $"The {description} file was damaged, so the previous saved copy was restored.\n\nDetails: {error}");
            }

            // Keep the damaged file so nothing is silently thrown away.
            var aside = Path.Combine(DataFolder,
                $"{Path.GetFileNameWithoutExtension(path)}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            try
            {
                File.Move(path, aside);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            return new LoadResult<T>(createDefault(),
                $"The {description} file could not be read and was moved to:\n{aside}\n\nDetails: {error}");
        }

        private static bool TryRead<T>(string path, out T? value, out string? error)
        {
            try
            {
                value = JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonOptions);
                error = value == null ? "The file is empty." : null;
                return value != null;
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or NotSupportedException)
            {
                value = default;
                error = ex.Message;
                return false;
            }
        }

        private static string BackupPath(string path) => path + ".bak";

        /// <summary>
        /// Writes JSON to "file.tmp", flushes it to disk, then swaps it in place of the target.
        /// The previous version is kept as "file.bak" when <paramref name="keepBackup"/> is true.
        /// </summary>
        private static void WriteJson<T>(string path, T value, bool keepBackup = true)
        {
            var folder = Path.GetDirectoryName(Path.GetFullPath(path))!;
            Directory.CreateDirectory(folder);

            var tempPath = path + ".tmp";
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, value, JsonOptions);
                stream.Flush(flushToDisk: true);
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, keepBackup ? BackupPath(path) : null, ignoreMetadataErrors: true);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch (PlatformNotSupportedException)
            {
                // File.Replace is not supported on some network/non-NTFS drives.
                File.Move(tempPath, path, overwrite: true);
            }
        }
    }
}
