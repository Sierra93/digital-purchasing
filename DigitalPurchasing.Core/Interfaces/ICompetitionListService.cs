using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompetitionListService
    {
        CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
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
