// Copyright 2025 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

public abstract class BaseExceptionHandler<THandler, TException> : IExceptionHandler
    where THandler : IExceptionHandler
    where TException : Exception
{
    protected virtual bool LogException
        => false;

    /// <inheritdoc />
    public bool Handle(ExceptionContext context)
    {
        if (CanHandle(context))
        {
            IActionResult? actionResult = CreateActionResult(context);
            if (actionResult != null)
            {
                if (LogException)
                {
                    ILogger<THandler> logger = CreateLogger<THandler>(context);
                    LogContext(logger, context);
                }

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

    protected abstract IActionResult? CreateActionResult(ExceptionContext context);
}
