using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Managers;
using MyInvoicingApp.Models;

namespace MyInvoicingApp
{
    public class Startup
    {
        public Startup()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddXmlFile("appsettings.xml");
            Configuration = configurationBuilder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                //Added for TempData in Chrome
                //This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax; // By default this is set to 'Strict'.
            });

            services.AddDbContext<EFCDbContext>(options =>
            {
                var cs = Configuration["ConnectionStrings:DefaultConnection"];
                options.UseSqlServer(cs);
            }, ServiceLifetime.Transient);

            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<EFCDbContext>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(x => x.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());

            //My services
            //services.AddScoped<ApplicationInitialize>();
            services.AddScoped<IDateHelper, DateHelper>();
            services.AddScoped<IFileHelper, FileHelper>();
            services.AddScoped<IControllerNameHelper, ControllerNameHelper>();
            services.AddScoped<IBudgetManager, BudgetManager>();
            services.AddScoped<ICustomerManager, CustomerManager>();
            services.AddScoped<IInvoiceManager, InvoiceManager>();
            services.AddScoped<IAttachmentManager, AttachmentManager>();
            services.AddScoped<IDocumentNumberingManager, DocumentNumberingManager>();
            services.AddSingleton<IDocumentNumberModelManager, DocumentNumberModelManager>();
            services.AddScoped<IPdfManager, PdfManager>();
            services.AddScoped<IExcelManager, ExcelManager>();
            services.AddScoped<IDataAccessManager, DataAccessManager>();
            services.AddScoped<IModuleAccessManager, ModuleAccessManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzIxNjNAMzEzNjJlMzMyZTMwUkJlL20xOTBBMTZzdTNuQUhIRDJzYTJaNVdNTU16YlJaYUJKemtyY2FiST0=;MzIxNjRAMzEzNjJlMzMyZTMwRkZBcitoMXloUkRaTnlnV3FhTFYveVh4NHNTWk0rdEFhVG1rQ2ZSL0VqYz0=");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //protected async void AppInit(IServiceProvider services)
        //{
        //    var init = new ApplicationInitialize(services);
        //    await init.Init();
        //}
    }
}
