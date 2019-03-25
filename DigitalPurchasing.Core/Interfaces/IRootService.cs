using System;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IRootService
    {
        Task<Guid> Create(Guid ownerId, Guid prId);
        Task<Guid> GetIdByPR(Guid prId);
        Task<Guid> GetIdByQR(Guid qrId);
        Task<Guid> GetIdByCL(Guid competitionListId);
        Task AssignQR(Guid ownerId, Guid rootId, Guid qrId);
        Task AssignCL(Guid ownerId, Guid rootId, Guid clId);
        Task SetStatus(Guid rootId, RootStatus status);
        
    }
}
