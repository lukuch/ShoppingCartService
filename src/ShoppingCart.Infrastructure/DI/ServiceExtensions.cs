using FluentValidation;
using JasperFx;
using JasperFx.Events;
using Mapster;
using Marten;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Npgsql;
using Polly;
using Refit;
using Serilog;
using ShoppingCart.Application.Behaviors;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Entities;
using ShoppingCart.Infrastructure.External;
using ShoppingCart.Infrastructure.Marten;
using ShoppingCart.Infrastructure.Redis;
using StackExchange.Redis;

namespace ShoppingCart.Infrastructure.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR for CQRS
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.DependencyInjection).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(Application.DependencyInjection).Assembly);

        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Mapster configuration
        ConfigureMapsterMappings();

        return services;
    }

    private static void ConfigureMapsterMappings()
    {
        TypeAdapterConfig<Cart, CartDto>.NewConfig()
            .Map(dest => dest.UserId, src => src.Id);

        TypeAdapterConfig<Cart, CartSummaryDto>.NewConfig()
            .Map(dest => dest.UserId, src => src.Id);
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Ensure database exists (like EF Core's EnsureCreated)
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        EnsureDatabaseExists(connectionString!);

        // Marten
        services.AddMarten(options =>
        {
            options.Connection(connectionString!);
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Events.StreamIdentity = StreamIdentity.AsString;
        })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();

        // Redis
        var redisConnectionString = configuration.GetConnectionString("Redis");
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString!));
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Refit for external API
        var productApiBaseUrl = configuration["ProductApi:BaseUrl"] ?? "https://fakeapi.platzi.com";
        services.AddRefitClient<IProductApi>()
            .AddPolicyHandler(
                Polly.Extensions.Http.HttpPolicyExtensions
                    .HandleTransientHttpError() // 5xx, 408, HttpRequestException
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            )
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(productApiBaseUrl));

        services.AddScoped<IProductService, ProductApiClient>();

        // Repository
        services.AddScoped<ICartRepository, CartRepository>();

        return services;
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        try
        {
            // Extract database name from connection string
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;
            
            // Connect to postgres database to check/create target database
            builder.Database = "postgres";
            
            using var connection = new NpgsqlConnection(builder.ToString());
            connection.Open();

            // Check if database exists
            using var checkCommand = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @databaseName", 
                connection);
            checkCommand.Parameters.AddWithValue("databaseName", databaseName ?? "shoppingcart");
            
            var exists = checkCommand.ExecuteScalar();
            
            if (exists == null)
            {
                // Create database
                using var createCommand = new NpgsqlCommand(
                    $"CREATE DATABASE \"{databaseName ?? "shoppingcart"}\"", 
                    connection);
                createCommand.ExecuteNonQuery();
                Log.Information("Database '{DatabaseName}' created successfully", databaseName);
            }
            else
            {
                Log.Information("Database '{DatabaseName}' already exists", databaseName);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database initialization failed");
            throw;
        }
    }
}
