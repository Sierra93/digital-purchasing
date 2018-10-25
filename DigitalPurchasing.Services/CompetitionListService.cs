using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class CompetitionListService : ICompetitionListService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;

        public CompetitionListService(ApplicationDbContext db, ICounterService counterService)
        {
            _db = db;
            _counterService = counterService;
        }

        public CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.CompetitionLists.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults
                .Skip((page-1)*perPage)
                .Take(perPage)
                .ProjectToType<CompetitionListIndexDataItem>()
                .ToList();

            return new CompetitionListIndexData
            {
                Data = result,
                Total = total
            };
        }

        public Guid GetId(Guid qrId)
        {
            var quotationRequest = _db.CompetitionLists.FirstOrDefault(q => q.QuotationRequestId == qrId);
            if (quotationRequest != null)
            {
                return quotationRequest.Id;
            }

            return Create(qrId);
        }

        private Guid Create(Guid qrId)
        {
            var entity = new CompetitionList
            {
                PublicId = _counterService.GetCLNextId(),
                QuotationRequestId = qrId
            };
            var entry = _db.CompetitionLists.Add(entity);
            _db.SaveChanges();
            return entry.Entity.Id;
        }
    }
}
