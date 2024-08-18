using Microsoft.EntityFrameworkCore;
using Rehleti.Areas.Dashboard.Controllers;
using Rehleti.EmailService;
using Rehleti.Models;
using SendGrid.Helpers.Mail;
using Stripe;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ModelContext>(options=>
options.UseOracle(builder.Configuration.GetConnectionString("Rehleti")));

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); });

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();