using Application.Services;
using Application.UseCases;
using Domain;
using Infrastructure;
using Nelibur.ObjectMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<OrderDetailService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<GetUnitNameUseCase>();
builder.Services.AddScoped<GetUnitsUseCase>();

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
