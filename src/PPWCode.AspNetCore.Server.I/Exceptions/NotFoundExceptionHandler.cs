using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class NotFoundExceptionHandler
    : BaseExceptionHandler<NotFoundExceptionHandler, NotFoundException>
{
    public NotFoundExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        : base(problemDetailsFactory)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context)
        => StatusCodes.Status404NotFound;
}
