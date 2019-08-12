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
        Task<RootDto> GetByCL(Guid competitionListId);
        Task AssignQR(Guid ownerId, Guid rootId, Guid qrId);
        Task AssignCL(Guid ownerId, Guid rootId, Guid clId);
        Task SetStatus(Guid rootId, RootStatus status);
    }

    public class RootDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public RootStatus Status { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public Guid? QuotationRequestId { get; set; }
        public Guid? CompetitionListId { get; set; }
    }
}
