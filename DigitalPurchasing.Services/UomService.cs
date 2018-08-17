using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public interface IUomService
    {
        UomDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        UomResult CreateUom(string name);
    }

    public class UomService : IUomService
    {
        private readonly ApplicationDbContext _db;

        public UomService(ApplicationDbContext db) => _db = db;

        public UomDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry =  _db.UnitsOfMeasurements.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<UomResult>().ToList();
            return new UomDataResult
            {
                Total = total,
                Data = result
            };
        }

        public UomResult CreateUom(string name)
        {
            var entry = _db.UnitsOfMeasurements.Add(new UnitsOfMeasurement {Name = name});
            _db.SaveChanges();
            return entry.Entity.Adapt<UomResult>();
        }
    }

    public class UomResultMapsterRegister : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<UnitsOfMeasurement, UomResult>().Map(d => d.IsSystem, s => !s.OwnerId.HasValue);
    }

    public class UomResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }

    public class UomDataResult : BaseDataResult<UomResult>
    {
    }
}
