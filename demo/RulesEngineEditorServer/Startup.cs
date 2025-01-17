// Copyright (c) Alex Reich.
// Licensed under the CC BY 4.0 License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RulesEngineEditorServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RulesEngineEditor.Services;
using RulesEngineEditor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using RulesEngineEditorServer.Authentication;

namespace RulesEngineEditorServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            //services.AddDbContext<RulesEngineEditorDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RulesEngineEditorDB")));
            //services.AddDbContextFactory<RulesEngineEditorDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RulesEngineEditorDB")), ServiceLifetime.Transient);

            #region 数据库

            #region Sqlite
            //services.AddDbContextFactory<RulesEngineEditorDbContext>(options => {
            //    var folder = Environment.SpecialFolder.LocalApplicationData;
            //    var path = Environment.GetFolderPath(folder);
            //    var DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}RulesEngineDemo.db";
            //    options.UseSqlite($"Data Source={DbPath}", op => { });
            //});
            #endregion

            #region MySql
            services.AddDbContextFactory<RulesEngineEditorDbContext>(options => {
                options.UseMySql(
                     Configuration.GetConnectionString("RulesEngineEditorContext"),
                     ServerVersion.AutoDetect(Configuration.GetConnectionString("RulesEngineEditorContext")),
                     x => {
                         x.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                     });
            });
            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<RulesEngineEditorDbContext>(); 
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
            #endregion

            services.AddScoped<DbContext, RulesEngineEditorDbContext>(); //表明MyDbContext是DbContext的具体实现 
            #endregion

            services.AddRulesEngineEditor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
