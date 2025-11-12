using AppOwnsData.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------
// 1️⃣ Add authentication and services
// -------------------------------------------

// Connects to Azure AD using settings in appsettings.json ("AzureAd" section)
// builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
//     .EnableTokenAcquisitionToCallDownstreamApi()
//     .AddInMemoryTokenCaches();

// Register your Power BI service for dependency injection
builder.Services.AddScoped<PowerBiServiceApi>();


// Require authentication globally for all MVC controllers
builder.Services.AddControllersWithViews(options =>
{
    // var policy = new AuthorizationPolicyBuilder()
    //     .RequireAuthenticatedUser()
    //     .Build();
    // options.Filters.Add(new AuthorizeFilter(policy));
})
.AddMicrosoftIdentityUI(); // adds Microsoft login/logout pages

builder.Services.AddRazorPages();


// -------------------------------------------
// 2️⃣ Build the app
// -------------------------------------------
var app = builder.Build();

// -------------------------------------------
// 3️⃣ Configure middleware pipeline
// -------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
