using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PPWCode.AspNetCore.Host.I.RouteConstraints;

public class EnumRouteConstraint<TEnum> : IRouteConstraint
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out object? value) || value is null)
        {
            return false;
        }

        return Enum.TryParse<TEnum>(value.ToString(), true, out _);
    }
}
