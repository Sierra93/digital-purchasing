using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                                    n.Id,
                                    n.Name
                                }).ToList();
                    var compDatas = new List<NomenclatureComparisonData>();
                    noms.ForEach(n =>
                    {
                        var copmDataItem = GetComparisonDataByNomenclatureName(n.Name);
                        copmDataItem.NomenclatureId = n.Id;
                        compDatas.Add(copmDataItem);
                    });
                    context.NomenclatureComparisonDatas.AddRange(compDatas);
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
