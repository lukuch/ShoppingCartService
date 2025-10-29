using MediatR;
using Serilog;

namespace ShoppingCart.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid().ToString();

        Log.Information("Processing {RequestType} with {CorrelationId}", requestName, correlationId);

        try
        {
            var response = await next();
            Log.Information("Completed {RequestType} with {CorrelationId}", requestName, correlationId);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing {RequestType} with {CorrelationId}", requestName, correlationId);
            throw;
        }
    }
}
