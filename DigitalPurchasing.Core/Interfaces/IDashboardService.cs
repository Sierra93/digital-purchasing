using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<DashboardRequestStatus>> GetRequestStatuses(Guid companyId, DateTime from, DateTime to);
        Task<DashboardTopSuppliers> GetTopSuppliers(Guid companyId, DateTime from, DateTime to);
    }

    public class DashboardRequestStatus
    {
        public string Name { get; set; }
        public decimal Qty { get; set; }
        public decimal Percentage(decimal totalQty)
            => totalQty > 0
                ? Qty / totalQty
                : 0;
    }

    public class DashboardTopSuppliers
    {
        public class TopSupplier
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public decimal Percentage(decimal totalAmount) =>
                totalAmount > 0
                    ? Amount / totalAmount
                    : 0;
        }

        public List<TopSupplier> TopSuppliers { get; set; } = new List<TopSupplier>();

        public decimal TopSuppliersAmount => TopSuppliers.Sum(q => q.Amount);
        public decimal TopSuppliersPercentage =>
            AllSuppliersAmount > 0
                ? TopSuppliersAmount / AllSuppliersAmount
                : 0;

        public decimal AllSuppliersAmount { get; set; }

        public decimal AllSuppliersCount { get; set; }
    }
}
