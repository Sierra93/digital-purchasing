using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DigitalPurchasing.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            using (var scope = webHost.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                DataSeeder.Seed(context);
            }
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSentry();

        public class DataSeeder
        {
            public static void Seed(ApplicationDbContext context)
            {
                if (!context.NomenclatureComparisonDatas.Any())
                {
                    var noms = (from n in context.Nomenclatures.IgnoreQueryFilters()
                                where !n.IsDeleted
                                select new
                                {
                                    NomenclatureId = n.Id,
                                    n.Name,
                                    AlternativeId = (Guid?)null
                                }).ToList();
                    noms.AddRange((from n in context.NomenclatureAlternatives.IgnoreQueryFilters()
                                   where !n.Nomenclature.IsDeleted
                                   select new
                                   {
                                       n.NomenclatureId,
                                       n.Name,
                                       AlternativeId = (Guid?)n.Id
                                   }).ToList());
                    var compDatas = new List<NomenclatureComparisonData>();
                    noms.ForEach(n =>
                    {
                        var compDataItem = GetComparisonDataByNomenclatureName(n.Name);
                        if (!string.IsNullOrWhiteSpace(compDataItem.AdjustedNomenclatureName))
                        {
                            compDataItem.NomenclatureId = n.NomenclatureId;
                            compDataItem.NomenclatureAlternativeId = n.AlternativeId;
                            compDatas.Add(compDataItem);
                        }
                    });
                    context.NomenclatureComparisonDatas.AddRange(compDatas);
                    context.SaveChanges();
                }

                if (!context.NomenclatureComparisonDataNGrams.Any())
                {
                    byte ngramLen = 3;
                    var data = (from cd in context.NomenclatureComparisonDatas
                                select new
                                {
                                    cd.Id,
                                    cd.AdjustedNomenclatureName
                                }).ToList();
                    var ngrams = (from cd in data
                                  from ngram in cd.AdjustedNomenclatureName.Ngrams(ngramLen)
                                  select new NomenclatureComparisonDataNGram
                                  {
                                      NomenclatureComparisonDataId = cd.Id,
                                      N = ngramLen,
                                      Gram = ngram,
                                  }).ToList();
                    context.NomenclatureComparisonDataNGrams.AddRange(ngrams);
                    context.SaveChanges();
                }
            }
        }

        private static NomenclatureComparisonData GetComparisonDataByNomenclatureName(string nomName)
        {
            var terms = new Services.NomenclatureComparisonService().CalculateComparisonTerms(nomName);

            return new NomenclatureComparisonData
            {
                AdjustedNomenclatureDigits = terms.AdjustedDigits,
                AdjustedNomenclatureName = terms.AdjustedName,
                NomenclatureDimensions = terms.NomDimensions
            };
        }
    }
}
