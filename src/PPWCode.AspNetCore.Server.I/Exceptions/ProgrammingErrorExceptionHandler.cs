using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using ProgrammingError = PPWCode.Vernacular.Exceptions.V.ProgrammingError;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ProgrammingErrorExceptionHandler
    : BaseExceptionHandler<ProgrammingErrorExceptionHandler, ProgrammingError>
{
    public ProgrammingErrorExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        : base(problemDetailsFactory)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context)
        => StatusCodes.Status500InternalServerError;

    /// <inheritdoc />
    protected override bool LogException
        => true;
}
