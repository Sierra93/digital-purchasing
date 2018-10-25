using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompetitionListService
    {
        CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Guid GetId(Guid qrId);
    }

    public class CompetitionListIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CompetitionListIndexData : BaseDataResponse<CompetitionListIndexDataItem>
    {

    }
}
