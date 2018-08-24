using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchasingRequestService
    {
        Guid CreateFromFile(string filePath);
        PurchasingRequestDetailsResponse GetById(Guid id);
    }

    public enum PurchasingRequestType
    {
        AppropriateColumns = 0,
        ManualInput = 10
    }

    public class PurchasingRequestDetailsResponse
    {
        public Guid Id { get; set; }

        public PurchasingRequestType Type { get; set; } 

        public ExcelTable ExcelTable { get; set; }

        public bool IsUploaded => ExcelTable != null;
    }
}
