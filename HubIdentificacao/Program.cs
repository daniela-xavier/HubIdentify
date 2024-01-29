
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.ComponentModel;
using HubIdentificacao.src.App.Hubs;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.MAppings;
using HubIdentificacao.src.App.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddTransient<IService, ServiceIdentify>();
builder.Services.AddTransient<IIdentifyDados, Identify>();
builder.Services.AddSingleton<IIdentifyAPI, IdentifyAPI>();
builder.Services.AddRazorPages();
//builder.Services.AddHostedService<Worker>();

builder.Services.AddAutoMapper(typeof(IdentifyMapping));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
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

app.Run();
