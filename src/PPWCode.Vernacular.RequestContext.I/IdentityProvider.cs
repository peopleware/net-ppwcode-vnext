namespace PPWCode.Vernacular.RequestContext.I;

public class IdentityProvider : IIdentityProvider
{
    public string IdentityName
        => Thread.CurrentPrincipal?.Identity?.IsAuthenticated == true
               ? Thread.CurrentPrincipal.Identity.Name ?? "Authenticated"
               : $"Not Authenticated - {Environment.UserName}";
}
