using System.Reflection;
using BookHeaven.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// -------------- Sessions ----------------- //
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#region Session and cookies settings

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);//You can set Time in seconds or minutes
    options.Cookie.IsEssential = true; // Set the session cookie as essential
});
#endregion

builder.Services.AddHttpContextAccessor();
// -------------- # # # # # ----------------- //

// Initialize SQLHelper
SQLHelper.Initialize(builder.Configuration);

var app = builder.Build();

// Run your initialization code here
User defaultUser = new User(); // User(0, "default@default.com", "default", "default", false);
User.currentUser = defaultUser;

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

app.UseSession(); // <---- Sessions 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserHome}/{action=showUserHome}/{id?}");

app.Run();
