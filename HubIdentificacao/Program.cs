
using HubIdentificacao.src.App.Hubs;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.MAppings;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddTransient<IService, ServiceIdentify>();
builder.Services.AddTransient<IIdentifyAPI, IdentifyAPI>();
builder.Services.AddTransient<IIdentifyDados, Identify>();


builder.Services.AddScoped<Data>();
builder.Services.AddRazorPages();
//builder.Services.AddHostedService<Worker>();

builder.Services.AddAutoMapper(typeof(IdentifyMapping));

//builder.Services.AddControllers().AddJsonOptions (options =>{
 //   options.JsonSerializerOptions.IncludeFields = true;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()){
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<IdentifyHub>("/identifyClient");
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.File("log.txt",outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

Log.Information("Ah, there you are!");

app.Run();
