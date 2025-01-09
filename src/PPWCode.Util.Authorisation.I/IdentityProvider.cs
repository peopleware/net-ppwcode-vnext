namespace PPWCode.Util.Authorisation.I;

public class IdentityProvider : IIdentityProvider
{
    private readonly IPrincipalProvider _principalProvider;

    public IdentityProvider(IPrincipalProvider principalProvider)
    {
        _principalProvider = principalProvider;
    }

    public string IdentityName
        => _principalProvider.CurrentPrincipal.Identity?.IsAuthenticated == true
               ? _principalProvider.CurrentPrincipal.Identity.Name ?? $"Authenticated - {Environment.UserName}"
               : $"Not Authenticated - {Environment.UserName}";
}
