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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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



builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderDetail>, GenericRepository<OrderDetail>>();
builder.Services.AddScoped<IRepository<Brand>, GenericRepository<Brand>>();
builder.Services.AddScoped<IRepository<Unit>, GenericRepository<Unit>>();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<OrderDetailService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<GetUnitNameUseCase>();
builder.Services.AddScoped<GetUnitsUseCase>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();

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
