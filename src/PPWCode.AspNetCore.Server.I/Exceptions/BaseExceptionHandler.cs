﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

public abstract class BaseExceptionHandler<THandler, TException> : IExceptionHandler
    where THandler : IExceptionHandler
    where TException : Exception
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    protected BaseExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    protected virtual bool LogException
        => false;

    /// <inheritdoc />
    public bool Handle(ExceptionContext context)
    {
        if (CanHandle(context))
        {
            if (LogException)
            {
                ILogger<THandler> logger = CreateLogger<THandler>(context);
                LogContext(logger, context);
            }

            int? statusCode = GetStatusCode(context);
            if (statusCode is not null)
            {
                ProblemDetails problemDetail =
                    _problemDetailsFactory
                        .CreateProblemDetails(
                            context.HttpContext,
                            statusCode: statusCode.Value);
                EnrichProblemDetails(context, context.Exception as TException, problemDetail);
                context.Result = new ObjectResult(problemDetail);
                return true;
            }

            IActionResult? actionResult = CreateActionResult(context);
            if (actionResult is not null)
            {
                context.Result = actionResult;
                return true;
            }
        }

        return false;
    }

    private ILogger<T> CreateLogger<T>(ExceptionContext context)
        where T : IExceptionHandler
    {
        HttpContext httpContext = context.HttpContext;
        ILoggerFactory factory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        return factory.CreateLogger<T>();
    }

    protected virtual bool CanHandle(ExceptionContext context)
        => context.Exception is TException;

    protected virtual void LogContext(ILogger<THandler> logger, ExceptionContext context)
        => logger.LogError(context.Exception, "Handled exception");

    protected virtual IActionResult? CreateActionResult(ExceptionContext context)
        => null;

    protected virtual int? GetStatusCode(ExceptionContext context)
        => null;

    protected virtual void EnrichProblemDetails(
        ExceptionContext context,
        TException? contextException,
        ProblemDetails problemDetail)
    {
    }
}
