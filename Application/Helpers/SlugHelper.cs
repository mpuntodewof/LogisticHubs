using System.Text.RegularExpressions;

namespace Application.Helpers
{
    public static partial class SlugHelper
    {
        public static string Generate(string name)
        {
            var slug = name.ToLowerInvariant().Trim();
            slug = NonAlphanumericRegex().Replace(slug, "-");
            slug = MultipleDashRegex().Replace(slug, "-");
            slug = slug.Trim('-');
            return slug;
        }

        [GeneratedRegex(@"[^a-z0-9\s-]")]
        private static partial Regex NonAlphanumericRegex();

        [GeneratedRegex(@"[\s-]+")]
        private static partial Regex MultipleDashRegex();
    }
}
