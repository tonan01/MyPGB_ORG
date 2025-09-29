using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PGB.BuildingBlocks.Application.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        #region Fields
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly int _slowRequestThresholdMs;
        #endregion

        #region Constructor
        public PerformanceBehavior(
           ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
           int slowRequestThresholdMs = 500)
        {
            _logger = logger;
            _slowRequestThresholdMs = slowRequestThresholdMs;
        }
        #endregion

        #region Handle Method
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > _slowRequestThresholdMs)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning(
                    "Slow request detected: {RequestName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                    requestName,
                    elapsedMs,
                    _slowRequestThresholdMs);
            }

            return response;
        } 
        #endregion
    }
}