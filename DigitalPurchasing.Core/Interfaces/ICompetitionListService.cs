using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompetitionListService
    {
        CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Guid GetId(Guid qrId);
        CompetitionListVm GetById(Guid id);
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

    public class CompetitionListVm
    {
        public class SupplierOffer
        {
            public Guid Id { get; set; }
            public int PublicId { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }

        public IEnumerable<SupplierOffer> SupplierOffers { get; set; }
    }
}
