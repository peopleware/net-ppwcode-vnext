using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class NotImplementedExceptionHandler
    : BaseExceptionHandler<NotImplementedExceptionHandler, NotImplementedException>
{
    public NotImplementedExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        : base(problemDetailsFactory)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context)
        => StatusCodes.Status501NotImplemented;

    /// <inheritdoc />
    protected override bool LogException
        => true;
}
