using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUploadedDocumentService
    {
        DeleteResultVm Delete(Guid id);
    }
}
