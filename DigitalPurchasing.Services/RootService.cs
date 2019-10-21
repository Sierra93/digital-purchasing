using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class RootService : IRootService
    {
        private readonly ApplicationDbContext _db;

        public RootService(ApplicationDbContext db) => _db = db;

        public async Task<Guid> Create(Guid ownerId, Guid prId)
        {
            var root = new Root
            {
                PurchaseRequestId = prId,
                OwnerId = ownerId
            };

            await _db.Roots.AddAsync(root);
            await _db.SaveChangesAsync();

            return root.Id;
        }

        public async Task<Guid> GetIdByPR(Guid prId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    q => q.PurchaseRequestId == prId);

            if (root == null) return Guid.Empty;

            return root.Id;
        }

        public async Task<RootDto> GetByCL(Guid competitionListId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.CompetitionListId == competitionListId);

            return root?.Adapt<RootDto>();
        }

        public async Task<RootDto> GetByQR(Guid quotationRequestId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.QuotationRequestId == quotationRequestId);

            return root?.Adapt<RootDto>();
        }

        public async Task AssignQR(Guid ownerId, Guid rootId, Guid qrId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    q => q.Id == rootId && q.OwnerId == ownerId);

            if (root == null) return;

            root.QuotationRequestId = qrId;
            await _db.SaveChangesAsync();
        }

        public async Task<Guid> GetIdByQR(Guid qrId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    q => q.QuotationRequestId == qrId);

            if (root == null) return Guid.Empty;

            return root.Id;
        }

        public async Task AssignCL(Guid ownerId, Guid rootId, Guid clId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.Id == rootId && q.OwnerId == ownerId);

            if (root == null) return;

            root.CompetitionListId = clId;
            await _db.SaveChangesAsync();
        }

        public async Task SetStatus(Guid rootId, RootStatus status)
        {
            if (rootId == Guid.Empty) return;

            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.Id == rootId);

            if (root == null) return;
            root.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task<Guid> GetIdByCL(Guid clId)
        {
            var root = await _db.Roots
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.CompetitionListId == clId);

            if (root == null) return Guid.Empty;

            return root.Id;
        }
    }
}
