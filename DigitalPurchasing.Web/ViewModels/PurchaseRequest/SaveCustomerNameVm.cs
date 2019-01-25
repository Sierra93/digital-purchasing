using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.ViewModels.PurchasingRequest
{
    public class SaveCustomerNameVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
