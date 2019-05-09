using System;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISelectedSupplierService
    {
        Task GenerateReport(Guid ownerId, Guid variantId);
    }
}
