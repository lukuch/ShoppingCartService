using Serilog;
using ShoppingCart.API.Endpoints;
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

// Application services
builder.Services.AddApplicationServices();

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

// Register endpoints
app.MapProductEndpoints();
app.MapCartEndpoints();

app.Run();
