using Carter;
using Serilog;
using ShoppingCart.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQL")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// Application services
builder.Services.AddApplicationServices();

// Carter
builder.Services.AddCarter();

// Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// Health Checks endpoint
app.MapHealthChecks("/health");

// Register Carter modules
app.MapCarter();

app.Run();
