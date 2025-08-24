using Xunit;
using FluentAssertions;
using ShoukatSons.Services;

namespace ShoukatSons.Tests
{
    public class ServiceTests
    {
        [Fact]
        public void Barcode_Validation_Works()
        {
            var svc = new BarcodeService();
            svc.IsValidCode128("ABC123").Should().BeTrue();
            svc.IsValidCode128("").Should().BeFalse();
        }
    }
}