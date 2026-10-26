using DailyQuote.Models;

namespace DailyQuote.Services
{
    /// <summary>
    /// Chooses the next unread entry, either in Excel order or at random.
    /// Read state is looked up through a callback, so the navigator never caches it.
    /// </summary>
    public sealed class EntryNavigator
    {
        private readonly IReadOnlyList<Entry> _entries;
        private readonly Func<string, bool> _isRead;
        private readonly Dictionary<string, int> _indexByRef;
        private readonly Random _random;

        /// <param name="entries">All entries, in Excel order.</param>
        /// <param name="isRead">Returns true if the Ref has been read.</param>
        /// <param name="random">Optional random source (useful for tests).</param>
        public EntryNavigator(IReadOnlyList<Entry> entries, Func<string, bool> isRead, Random? random = null)
        {
            _entries = entries;
            _isRead = isRead;
            _random = random ?? Random.Shared;
            _indexByRef = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < entries.Count; i++)
            {
                _indexByRef.TryAdd(entries[i].Ref, i);
            }
        }

        public OrderMode Mode { get; set; } = OrderMode.Sequential;

        /// <summary>Number of entries in the loaded file.</summary>
        public int TotalCount => _entries.Count;

        /// <summary>Number of entries in the loaded file that have been read.</summary>
        public int ReadCount => _entries.Count(e => _isRead(e.Ref));

        /// <summary>Number of entries in the loaded file that are still unread.</summary>
        public int UnreadCount => TotalCount - ReadCount;

        /// <summary>
        /// Returns the next unread entry after <paramref name="current"/>, or the first one when
        /// <paramref name="current"/> is null. The current entry is only returned again if it is
        /// the last unread entry. Returns null when everything has been read.
        /// </summary>
        public Entry? GetNext(Entry? current)
        {
            return Mode == OrderMode.Random ? GetRandom(current) : GetSequential(current);
        }

        private Entry? GetSequential(Entry? current)
        {
            int count = _entries.Count;
            if (count == 0)
            {
                return null;
            }

            int start = 0;
            if (current != null && _indexByRef.TryGetValue(current.Ref, out var index))
            {
                start = index + 1;
            }

            // Walk forward and wrap around, so skipped entries come back after the end.
            for (int step = 0; step < count; step++)
            {
                var candidate = _entries[(start + step) % count];
                if (!_isRead(candidate.Ref))
                {
                    return candidate;
                }
            }
            return null;
        }

        private Entry? GetRandom(Entry? current)
        {
            var unread = _entries.Where(e => !_isRead(e.Ref)).ToList();
            if (unread.Count == 0)
            {
                return null;
            }
            if (unread.Count > 1 && current != null)
            {
                unread.RemoveAll(e => string.Equals(e.Ref, current.Ref, StringComparison.OrdinalIgnoreCase));
            }
            return unread[_random.Next(unread.Count)];
        }
    }
}
