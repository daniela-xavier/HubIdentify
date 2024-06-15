using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using HubIdentificacao.src.App.Hubs;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.MAppings;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Configs;
using HubIdentificacao.src.App.Controllers;
using Microsoft.AspNetCore.Http;

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
            // Carregar configurações do arquivo appsettings.json
            var allowedOrigins = Configuration.GetSection("AllowedOrigins").Get<string[]>();

            // Registrar o serviço AppSettings para injeção de dependência
            services.AddSingleton<AppSettings>();


            // Configuração do serviço de logging
            services.AddLogging(loggingBuilder =>
            {
                // Configuração do logger Serilog
                var serviceProvider = services.BuildServiceProvider();
                var appSettings = serviceProvider.GetService<AppSettings>();
                var logger = new LoggerConfig(appSettings).CreateLogger(Configuration);
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
            services.AddScoped<DataAutorization>();
            
            services.AddRazorPages();

            services.AddAutoMapper(typeof(IdentifyMapping));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string[]>())
                        .AllowAnyMethod()
                        //.WithHeaders("Authorization", "Content-Type", "x-itau-apikey", "x-itau-visual-correlationID")
                        .WithHeaders(Configuration.GetSection("Headers").Get<string[]>())
                        );
            });

            

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

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            // Adicionar middleware ValidateNegotiate
            app.UseMiddleware<ValidateNegotiate>();

            // Adicionar middleware DataAutorization
            //app.UseMiddleware<DataAutorization>();


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
