using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_Core_Mentoring_Module1.Logging
{
    public class LoggingActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        public LoggingActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LoggingActionFilter");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(nameof(OnActionExecuting));
            _logger.LogDebug($"{context.ActionDescriptor.DisplayName} {string.Join(", ", context.ActionArguments.Select(arg => $"{arg.Key}: {arg.Value}"))}");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation(nameof(OnActionExecuted));
            _logger.LogDebug($"{context.ActionDescriptor.DisplayName} {context.Exception}");
            if (context.Exception != null)
            {
                _logger.LogError($"{context.Exception}");
            }
            base.OnActionExecuted(context);
        }
    }
}
