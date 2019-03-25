using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;

        public DashboardService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<DashboardRequestStatus>> GetRequestStatuses(Guid companyId, DateTime from, DateTime to)
        {
            var result = new List<DashboardRequestStatus>();

            var requests = await _db.Roots
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == companyId && q.CreatedOn >= from && q.CreatedOn <= to)
                .ToListAsync();

            result.Add(new DashboardRequestStatus
            {
                Name = "Не обработанные заявки",
                Qty = requests.Count(q => q.Status == RootStatus.JustCreated)
            } );
            result.Add(new DashboardRequestStatus
            {
                Name = "Отправлен запрос КП",
                Qty = requests.Count(q => q.Status == RootStatus.QuotationRequestSent)
            });
            result.Add(new DashboardRequestStatus
            {
                Name = "Нужно сопоставить номенклатуру КП",
                Qty = requests.Count(q => q.Status == RootStatus.MatchingRequired)
            });
            result.Add(new DashboardRequestStatus
            {
                Name = "Нужно подтвердить выбор поставщика",
                Qty = requests.Count(q => q.Status == RootStatus.EverythingMatches)
            });
            result.Add(new DashboardRequestStatus
            {
                Name = "Закрытые заявки",
                Qty = requests.Count(q => q.Status == RootStatus.SupplierSelected)
            });
            
            return result;
        }

        public async Task<DashboardTopSuppliers> GetTopSuppliers(Guid companyId, DateTime from, DateTime to)
        {
            var result = new DashboardTopSuppliers
            {
                AllSuppliersAmount = 20000,
                AllSuppliersCount = await _db.Suppliers.IgnoreQueryFilters()
                    .Where(q => q.OwnerId == companyId)
                    .CountAsync()
            };

            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 1", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 2", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 3", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 4", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 5", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 6", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 7", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 8", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 9", Amount = 1000 });
            result.TopSuppliers.Add(new DashboardTopSuppliers.TopSupplier { Name  = "Supp 10", Amount = 1000 });

            return result;
        }
    }
}
