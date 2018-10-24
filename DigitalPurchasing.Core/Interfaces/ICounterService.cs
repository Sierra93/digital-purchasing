namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICounterService
    {
        int GetQRNextId();
        int GetPRNextId();
        int GetCLNextId();
    }
}
