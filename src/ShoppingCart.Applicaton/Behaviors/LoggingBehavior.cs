using MediatR;
using Serilog;

namespace ShoppingCart.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        Log.Information("Handling {RequestName}", requestName);

        try
        {
            var response = await next();
            Log.Information("Completed {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}
