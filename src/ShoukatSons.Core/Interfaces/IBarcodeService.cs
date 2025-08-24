namespace ShoukatSons.Core.Interfaces
{
    public interface IBarcodeService
    {
        // Returns CODE128 content validation; UI layer will render
        bool IsValidCode128(string content);
    }
}