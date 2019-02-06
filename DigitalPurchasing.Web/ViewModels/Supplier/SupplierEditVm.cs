using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.Supplier
{
    public class SupplierEditVm
    {
        public SupplierVm Supplier { get; set; }
        public List<SupplierContactPersonVm> ContactPersons { get; set; }
    }
}
