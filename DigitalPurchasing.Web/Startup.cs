using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using Microsoft.AspNetCore.Localization;

namespace DigitalPurchasing.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddRazorPagesOptions(options =>
            {
                options.AllowAreas = true;
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddHttpContextAccessor();

            services.AddSingleton<IEmailService, EmailService>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();

            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<INomenclatureCategoryService, NomenclatureCategoryService>();
            services.AddScoped<IUomService, UomService>();
            services.AddScoped<INomenclatureService, NomenclatureService>();
            services.AddScoped<IDictionaryService, DictionaryService>();
            services.AddScoped<IExcelRequestReader, ExcelRequestReader>();
            services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
            services.AddScoped<IQuotationRequestService, QuotationRequestService>();
            services.AddScoped<ICompetitionListService, CompetitionListService>();
            services.AddScoped<ICounterService, CounterService>();
            services.AddScoped<IColumnNameService, ColumnNameService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<ISupplierOfferService, SupplierOfferService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUploadedDocumentService, UploadedDocumentService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IAnalysisService, AnalysisService>();
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(DeliveryMappings).Assembly /*DigitalPurchasing.Services*/);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                dbContext.Database.Migrate();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            const string defaultCulture = "ru-RU";

            var supportedCultures = new[]
            {
                new CultureInfo(defaultCulture),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
