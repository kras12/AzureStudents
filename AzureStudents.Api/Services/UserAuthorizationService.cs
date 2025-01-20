using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AzureStudents.Api.Services;

/// <summary>
/// Handles authorization for users.
/// </summary>
public class UserAuthorizationService : IUserAuthorizationService
{
    #region Fields

    /// <summary>
    /// The injected authorization service. 
    /// </summary>
    private readonly IAuthorizationService _authorizationService;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="authorizationService">The injected authorization service. </param>
    public UserAuthorizationService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    #endregion

    #region Methods    

    /// <summary>
    /// Checks whether the user is authorizated against a policy.
    /// </summary>
    /// <param name="userClaimsPrincipal">The claims principal for the user.</param>
    /// <param name="policy">The policy.</param>
    /// <returns>True if the user is authorized.</returns>
    public async Task<bool> IsUserAuthorized(ClaimsPrincipal userClaimsPrincipal, string policy)
    {
        var result = await _authorizationService.AuthorizeAsync(userClaimsPrincipal, policy);
        return result.Succeeded;
    }

    #endregion
}
