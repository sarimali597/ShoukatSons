using System.Text.RegularExpressions;
using ShoukatSons.Core.Interfaces;

namespace ShoukatSons.Services
{
    public class BarcodeService : IBarcodeService
    {
        // CODE128 supports full ASCII; practical check = non-empty, reasonable length
        private static readonly Regex Allowed = new Regex(@"^[\x20-\x7E]{4,32}$", RegexOptions.Compiled);
        public bool IsValidCode128(string content) => !string.IsNullOrWhiteSpace(content) && Allowed.IsMatch(content);
    }
}