using CatalogService.API.GrpcServices;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Repositories;
using CatalogService.Infrastructure.Services;
using CatalogService.Infrastructure.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Logging estructurado en JSON
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions { Indented = false };
});

// MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();  // HU-10

// Services
builder.Services.AddScoped<IImageValidator, ImageValidator>();
builder.Services.AddScoped<IAuditService, AuditService>();              // HU-10
builder.Services.AddScoped<IImportService, ImportService>();            // HU-11
builder.Services.AddScoped<IEventPublisher, EventPublisher>();          // RabbitMQ

// JWT Configuration
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtService, JwtService>();

// Health Checks (RNF-02, RNF-08)
builder.Services.AddHealthChecks();

// gRPC (HU-09)
builder.Services.AddGrpc();

// CORS Configuration for Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable static files for image uploads
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowFrontend");

// Health Check Endpoint (RNF-02, RNF-08)
app.MapHealthChecks("/health");

// gRPC Mapping
app.MapGrpcService<CatalogGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a gRPC client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseAuthorization();
app.MapControllers();
app.Run();