using Xunit;
using FluentAssertions;
using ShoukatSons.Core.Helpers;

namespace ShoukatSons.Tests
{
    public class CoreTests
    {
        [Fact]
        public void PasswordHasher_Roundtrip_Works()
        {
            var hash = PasswordHasher.Hash("abc123");
            PasswordHasher.Verify("abc123", hash).Should().BeTrue();
            PasswordHasher.Verify("wrong", hash).Should().BeFalse();
        }
    }
}