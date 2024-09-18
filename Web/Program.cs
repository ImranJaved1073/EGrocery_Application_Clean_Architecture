using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Data;
using Infrastructure;
using Application.Services;
using Application.UseCases;
using Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    policy.RequireClaim(ClaimTypes.Email, "admin@gmail.com"));

});
//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("AdminPolicy", policy =>
//    policy.RequireClaim(ClaimTypes.Email, "abcd@gmail.com"));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<OrderDetailService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<GetUnitNameUseCase>();
builder.Services.AddScoped<GetUnitsUseCase>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();

builder.Services.AddTransient<IProductRepository, ProductRepository>(provider =>
    new ProductRepository(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(provider =>
    new CategoryRepository(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>(provider =>
    new OrderRepository(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.AddScoped<IRepository<OrderDetail>>(provider => new GenericRepository<OrderDetail>(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.AddScoped<IRepository<Brand>, GenericRepository<Brand>>(provider => new GenericRepository<Brand>(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
builder.Services.AddScoped<IRepository<Unit>>(provider => new GenericRepository<Unit>(@"Server=tcp:quran-sql-db-server.database.windows.net,1433;Initial Catalog=GroceryDb;Persist Security Info=False;User ID=imranjavedlogin;Password=Bsef21m033;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    //app.UseStatusCodePages();
    app.UseStatusCodePagesWithRedirects("/ErrorPages/{0}");
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();
app.MapHub<OrderNotificationHub>("/orderNotificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
