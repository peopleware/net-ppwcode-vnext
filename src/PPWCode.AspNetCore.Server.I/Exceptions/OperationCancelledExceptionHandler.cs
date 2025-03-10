using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class OperationCancelledExceptionHandler
    : BaseExceptionHandler<OperationCancelledExceptionHandler, OperationCanceledException>
{
    public OperationCancelledExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        : base(problemDetailsFactory)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context)
        => 499; // client closed
}
