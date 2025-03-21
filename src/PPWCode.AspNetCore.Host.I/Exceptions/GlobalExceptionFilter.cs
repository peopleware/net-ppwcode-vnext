using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using PPWCode.AspNetCore.Server.I.Exceptions;

namespace PPWCode.AspNetCore.Host.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IExceptionHandler[] _handlers;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IEnumerable<IExceptionHandler> handlers)
    {
        _logger = logger;
        _handlers = handlers.ToArray();
    }

    /// <inheritdoc />
    public Task OnExceptionAsync(ExceptionContext context)
    {
        bool handled = false;
        foreach (IExceptionHandler handler in _handlers)
        {
            handled |= handler.Handle(context);
            if (handled)
            {
                break;
            }
        }

        if (!handled)
        {
            _logger.LogError(context.Exception, "Unhandled exception");
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
