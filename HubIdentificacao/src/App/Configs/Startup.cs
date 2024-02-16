using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using HubIdentificacao.src.App.Hubs;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.MAppings;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;
using HubIdentificacao.src.App.Validators;

namespace HubIdentificacao.src.App.Configs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração do serviço de logging
            services.AddLogging(loggingBuilder =>
            {
                // Configuração do logger Serilog
                var logger = LoggerConfig.CreateLogger(Configuration);
                loggingBuilder.AddSerilog(logger);
            });

            // Add services to the container.
            services.AddControllersWithViews();
            services.AddSignalR();

            services.AddTransient<IService, ServiceIdentify>();
            services.AddTransient<IIdentifyAPI, IdentifyAPI>();
            services.AddTransient<IIdentifyDados, Identify>();

            services.AddScoped<Data>();
            services.AddScoped<DataTransformData>();
            services.AddRazorPages();
            //builder.Services.AddHostedService<Worker>();

            services.AddAutoMapper(typeof(IdentifyMapping));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configuração do pipeline de requisição HTTP
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Middleware para tratamento de erros em produção
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<IdentifyHub>("/identifyClient");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

            });
            
            Log.Information("Configuração do aplicativo concluída.");
        }
    }
}