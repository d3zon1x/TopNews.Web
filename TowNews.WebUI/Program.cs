using Microsoft.AspNetCore.Identity;
using TopNews.Core.Entities.User;
using TopNews.Infrastructure;
using TopNews.Infrastructure.Context;
using TopNews.Infrastructure.Initializators;
using TopNews.Infrastructure.Initializers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Create conection string 
string connStr = builder.Configuration.GetConnectionString("DefaultConnection");
//Database context
builder.Services.AddDbContext(connStr);

// Add InfrastructureServices
builder.Services.AddInfrasctructureServices();

//builder.Services.AddDefaultIdentity<AppUser>().AddRoles<IdentityRole>()
//            .AddEntityFrameworkStores<AppDbContext>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//await UsersAndRolesInitializers.SeedUserAndRole(app);
//UserDbInitializer.SeedUsers(userManager);

app.Run();
