using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.Dashboard
{
    public class DashboardIndexVm
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public IEnumerable<DashboardRequestStatus> RequestStatuses { get; set; }
        public DashboardTopSuppliers Suppliers { get; set; }
    }
}
