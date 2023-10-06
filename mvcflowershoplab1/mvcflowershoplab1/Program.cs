using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Data;
using mvcflowershoplab1.Areas.Identity.Data;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("mvcbicyclerentalContextConnection") ?? throw new InvalidOperationException("Connection string 'mvcbicyclerentalContextConnection' not found.");

//allow xray to monitor all the backend service
AWSSDKHandler.RegisterXRayForAllServices();

builder.Services.AddDbContext<mvcbicyclerentalContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<mvcbicyclerentalUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<mvcbicyclerentalContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseXRay("MVCFlowerShopSystem");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
