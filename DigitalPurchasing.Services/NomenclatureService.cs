using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public interface INomenclatureService
    {
        NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
    }

    public class NomenclatureService : INomenclatureService
    {
        private readonly ApplicationDbContext _db;
        private readonly List<NomenclatureResult> _nomenclature;
        
        public NomenclatureService(ApplicationDbContext dbContext)
        {
            _db = dbContext;
#pragma warning disable IDE0021 // Use expression body for constructors
            _nomenclature = new List<NomenclatureResult>
            {
                new NomenclatureResult { Code = "00", NameRus = "номенклатура 00", NameEng = "Nomenclature 00", Id = new Guid("f2a00a8f20e443a09acd8811a132f9fa") },
                new NomenclatureResult { Code = "01", NameRus = "номенклатура 01", NameEng = "Nomenclature 01", Id = new Guid("7880d6c5f6e84a21aef1f5bb50707244") },
                new NomenclatureResult { Code = "02", NameRus = "номенклатура 02", NameEng = "Nomenclature 02", Id = new Guid("255b441c731543b1b7cd059ab06067d9") },
                new NomenclatureResult { Code = "03", NameRus = "номенклатура 03", NameEng = "Nomenclature 03", Id = new Guid("be9d931f2a534d2d8e3c6b1045585743") },
                new NomenclatureResult { Code = "04", NameRus = "номенклатура 04", NameEng = "Nomenclature 04", Id = new Guid("8e983dd556d9421e9139ba9cfc6b71b0") },
                new NomenclatureResult { Code = "05", NameRus = "номенклатура 05", NameEng = "Nomenclature 05", Id = new Guid("6152cebbeb754da69e4f2761e644c35f") },
                new NomenclatureResult { Code = "06", NameRus = "номенклатура 06", NameEng = "Nomenclature 06", Id = new Guid("fc8d24367bbb4e4ba7cb55e926a66aa9") },
                new NomenclatureResult { Code = "07", NameRus = "номенклатура 07", NameEng = "Nomenclature 07", Id = new Guid("83f57ebd435c4df28c1db9de01bb97cf") },
                new NomenclatureResult { Code = "08", NameRus = "номенклатура 08", NameEng = "Nomenclature 08", Id = new Guid("7ee0e059fbf64ca3b9fe2aea3a4a8bcd") },
                new NomenclatureResult { Code = "09", NameRus = "номенклатура 09", NameEng = "Nomenclature 09", Id = new Guid("96eda2a96c5847ff81fea03e2e9c6c96") },
                new NomenclatureResult { Code = "10", NameRus = "номенклатура 10", NameEng = "Nomenclature 10", Id = new Guid("117b71c157194566b3dbe8a08fd6e5d2") },
                new NomenclatureResult { Code = "11", NameRus = "номенклатура 11", NameEng = "Nomenclature 11", Id = new Guid("24cd17497e2b4ec0b00610ff350ddd99") },
                new NomenclatureResult { Code = "12", NameRus = "номенклатура 12", NameEng = "Nomenclature 12", Id = new Guid("fb72018442604951a9c008b398d3f39d") },
                new NomenclatureResult { Code = "13", NameRus = "номенклатура 13", NameEng = "Nomenclature 13", Id = new Guid("78d328383cb64a0cab206557f31a213c") },
                new NomenclatureResult { Code = "14", NameRus = "номенклатура 14", NameEng = "Nomenclature 14", Id = new Guid("2bbfd32e5fec415bbcd855dfd82cf820") },
                new NomenclatureResult { Code = "15", NameRus = "номенклатура 15", NameEng = "Nomenclature 15", Id = new Guid("820f8653da9c4388a47428db34b21237") },
                new NomenclatureResult { Code = "16", NameRus = "номенклатура 16", NameEng = "Nomenclature 16", Id = new Guid("36cc9bca43e04f3589f5501ede5a1804") },
                new NomenclatureResult { Code = "17", NameRus = "номенклатура 17", NameEng = "Nomenclature 17", Id = new Guid("a3faf7bdcf5c4093ad5a22dd089a5975") },
                new NomenclatureResult { Code = "18", NameRus = "номенклатура 18", NameEng = "Nomenclature 18", Id = new Guid("54bb6efc7baf46428e016681c5da4675") },
                new NomenclatureResult { Code = "19", NameRus = "номенклатура 19", NameEng = "Nomenclature 19", Id = new Guid("27854e6253a64e88aed0c439598fda6a") },
                new NomenclatureResult { Code = "20", NameRus = "номенклатура 20", NameEng = "Nomenclature 20", Id = new Guid("ebed091c34ad4d82bc84de9822e20c22") },
                new NomenclatureResult { Code = "21", NameRus = "номенклатура 21", NameEng = "Nomenclature 21", Id = new Guid("b6a1a4e0624a491c84c967feb01135e6") },
            };
#pragma warning restore IDE0021 // Use expression body for constructors
        }

        public NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            var qryTmp = _db.Nomenclatures.ToList();

            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Code";
            }

            var qry = _nomenclature.AsQueryable();
            var total = _nomenclature.Count;
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ToList();
            return new NomenclatureDataResult
            {
                Total = total,
                Data = result
            };
        }
    }

    public class NomenclatureResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string NameRus { get; set; }
        public string NameEng { get; set; }
    }

    public class NomenclatureDataResult : BaseDataResult<NomenclatureResult>
    {
    }
}
