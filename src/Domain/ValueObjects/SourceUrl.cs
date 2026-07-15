using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A well-formed absolute http/https URI identifying the source of a citation
    /// used in live research.
    /// </summary>
    public sealed class SourceUrl : ValueObject
    {
        public Uri Value { get; }

        private SourceUrl(Uri value)
        {
            Value = value;
        }

        public static SourceUrl Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            var isWellFormed =
                Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

            if (!isWellFormed)
            {
                throw new ArgumentException(
                    "Source URL must be a well-formed absolute http or https URI.",
                    nameof(value));
            }

            return new SourceUrl(uri!);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
