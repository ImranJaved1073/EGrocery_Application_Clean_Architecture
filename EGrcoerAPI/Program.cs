using Application.Services;
using Application.UseCases;
using Domain;
using EGrocerAPI;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Nelibur.ObjectMapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.

builder.Services.AddAuthorization();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:44333",
            ValidAudience = "https://localhost:44333",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });




builder.Services.AddControllers();
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
    new ProductRepository(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(provider =>
    new CategoryRepository(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>(provider =>
    new OrderRepository(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));
builder.Services.AddScoped<IRepository<OrderDetail>>(provider => new GenericRepository<OrderDetail>(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));
builder.Services.AddScoped<IRepository<Brand>, GenericRepository<Brand>>(provider => new GenericRepository<Brand>(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));
builder.Services.AddScoped<IRepository<Unit>>(provider => new GenericRepository<Unit>(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0"));

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

TinyMapper.Bind<EGrcoerAPI.Models.Category, Category>();
TinyMapper.Bind<Category, EGrcoerAPI.Models.Category>();
TinyMapper.Bind<List<EGrcoerAPI.Models.Category>, List<Category>>();
TinyMapper.Bind<List<Category>, List<EGrcoerAPI.Models.Category>>();
TinyMapper.Bind<EGrcoerAPI.Models.Product, Product>();
TinyMapper.Bind<Product, EGrcoerAPI.Models.Product>();
TinyMapper.Bind<List<EGrcoerAPI.Models.Product>, List<Product>>();
TinyMapper.Bind<List<Product>, List<EGrcoerAPI.Models.Product>>();

TinyMapper.Bind<EGrcoerAPI.Models.SubCategoryViewModel, SubCategoryViewModel>();
TinyMapper.Bind<SubCategoryViewModel, EGrcoerAPI.Models.SubCategoryViewModel>();

TinyMapper.Bind<EGrcoerAPI.Models.Cart, Cart>();
TinyMapper.Bind<Cart, EGrcoerAPI.Models.Cart>();
TinyMapper.Bind<EGrcoerAPI.Models.CartItem, CartItem>();
TinyMapper.Bind<CartItem, EGrcoerAPI.Models.CartItem>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
