using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ApiUsageErrorExceptionHandler
    : BaseExceptionHandler<ApiUsageErrorExceptionHandler, ApiUsageError>
{
    public ApiUsageErrorExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        : base(problemDetailsFactory)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context)
        => StatusCodes.Status400BadRequest;
}
