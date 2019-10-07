using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.ViewModels.CompetitionList
{
    public class SendPriceReductionRequestsVm
    {
        public class Item
        {
            public Guid SupplierOfferId { get; set; }
            public Guid ItemId { get; set; }
            public decimal TargetPrice { get; set; }
        }

        public List<Item> Items { get; set; }
    }
}
