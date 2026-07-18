namespace Application.Policies
{
    /// <summary>
    /// A small, deliberately simple whole-word overlap scorer shared by
    /// <see cref="StyleSuggestionPolicy"/> and
    /// <see cref="CriteriaSuggestionPolicy"/> — both need to match one piece
    /// of free-typed text against a piece of admin-authored reference text
    /// with no exact key to join on. This is a suggestion mechanism, not a
    /// search engine: it counts distinct shared words (case-insensitive,
    /// ignoring a short list of stopwords) and nothing more elaborate, since
    /// a wrong suggestion here only ever costs the user one ignored ribbon or
    /// chip, never a wrong report.
    /// </summary>
    internal static class KeywordOverlap
    {
        private static readonly HashSet<string> Stopwords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "and", "for", "of", "on", "or", "the", "to", "with"
        };

        private static readonly char[] TrimCharacters = ['.', ',', ';', ':', '!', '?', '\'', '"', '(', ')'];

        public static int Score(string first, string second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            var firstWords = Tokenize(first);
            firstWords.IntersectWith(Tokenize(second));

            return firstWords.Count;
        }

        private static HashSet<string> Tokenize(string value)
            => value
                .Split((char[]?) null, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim(TrimCharacters))
                .Where(word => word.Length > 0 && !Stopwords.Contains(word))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
