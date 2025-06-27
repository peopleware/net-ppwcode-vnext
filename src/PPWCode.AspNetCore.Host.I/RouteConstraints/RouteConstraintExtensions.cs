using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace PPWCode.AspNetCore.Host.I.RouteConstraints;

public static class RouteConstraintExtensions
{
    public static IServiceCollection AddEnumConstraint<TEnum>(this IServiceCollection services, string name)
        where TEnum : struct, Enum
    {
        services.Configure<RouteOptions>(options => { options.ConstraintMap[name] = typeof(EnumRouteConstraint<TEnum>); });
        return services;
    }
}
