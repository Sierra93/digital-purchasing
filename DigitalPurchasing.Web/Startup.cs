using System;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Web.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using DigitalPurchasing.Web.Core.ModelBinders;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Logging;

namespace DigitalPurchasing.Web
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Environment = environment;
            _loggerFactory = loggerFactory;
        }

        public IHostingEnvironment Environment { get; }

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

            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(5));
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // Job duplication fix https://github.com/HangfireIO/Hangfire/issues/1197
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
                //x.UseMemoryStorage();
            });
            services.AddHangfireServer();

            //services.AddHangfire();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            GlobalJobFilters.Filters.Add(new HangfireSentryAttribute());

            services.AddMemoryCache();

            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider(_loggerFactory));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddRazorPagesOptions(options =>
            {
                options.AllowAreas = true;
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });

            if (Environment.IsDevelopment())
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 5001;
                });
            }
            else
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }            

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddHttpContextAccessor();

            services.AddSingleton<IEmailService, EmailService>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<INomenclatureCategoryService, NomenclatureCategoryService>();
            services.AddScoped<IUomService, UomService>();
            services.AddScoped<INomenclatureComparisonService, NomenclatureComparisonService>();
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
            services.AddScoped<IRobotEmailService, RobotEmailService>();
            services.AddScoped<IEmailProcessor, RFQEmailProcessor>();
            services.AddScoped<IReceivedEmailService, ReceivedEmailService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IRootService, RootService>();
            services.AddScoped<ISelectedSupplierService, SelectedSupplierService>();
            services.AddScoped<IConversionRateService, ConversionRateService>();
            services.AddScoped<INomenclatureAlternativeService, NomenclatureAlternativeService>();
            services.AddScoped<IFileService, FileService>();
            services.AddMandrill();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContextAccessor = x.GetRequiredService<IActionContextAccessor>();
                var actionContext = actionContextAccessor.ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ApplicationDbContext dbContext)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(DeliveryMappings).Assembly /*DigitalPurchasing.Services*/);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            DatabaseSetup(dbContext);

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

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new HangfireDashboardAuthorizationFilter() }
            });

            RecurringJob.AddOrUpdate<EmailJobs>("check_robot_emails",
                q => q.CheckRobotEmails(),
                Cron.MinuteInterval(5));

            RecurringJob.AddOrUpdate<CompetitionListJobs>("close_expired_competition_lists",
                q => q.CloseExpired(),
                Cron.MinuteInterval(5));
        }

        private void DatabaseSetup(ApplicationDbContext dbContext)
        {
            var currentTimeout = dbContext.Database.GetCommandTimeout();
            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
            var sw = System.Diagnostics.Stopwatch.StartNew();
            dbContext.Database.Migrate();
            DataSeeder.Seed(dbContext);
            sw.Stop();
            dbContext.Database.SetCommandTimeout(currentTimeout);
        }
    }
}
