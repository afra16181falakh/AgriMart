using AgriMartAPI.Data;
using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Create WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// --- Get the Connection String ---
string? connectionString = builder.Configuration.GetConnectionString("InputShopConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("FATAL ERROR: Connection string 'InputShopConnection' not found in appsettings.json.");
    Console.ResetColor();
    return;
}

// --- ADO.NET Database Migration ---
try
{
    string scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseScripts");
    if (Directory.Exists(scriptsPath))
    {
        DatabaseMigrator.ApplyMigrations(connectionString, scriptsPath);
        Console.WriteLine("Database migration check completed successfully.");
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"FATAL ERROR during database migration: {ex.Message}");
    Console.ResetColor();
    return;
}

// --- Add Services ---
builder.Services.AddControllers();

// ✅ Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>(); 
builder.Services.AddScoped<IOrderRepository, OrderRepository>(); 
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();     
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>(); 
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>(); 
builder.Services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();       
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();   
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();   

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Build the App ---
var app = builder.Build();

// --- Configure the HTTP request pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();