namespace DigitalPurchasing.Core.Interfaces
{
    public interface IReceivedEmailService
    {
        bool IsProcessed(uint uid);
        void MarkProcessed(uint uid, bool isProcessed);
    }
}
