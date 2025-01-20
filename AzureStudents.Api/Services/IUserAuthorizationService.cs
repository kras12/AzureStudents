using System.Security.Claims;

namespace AzureStudents.Api.Services;

/// <summary>
/// Interface for a service that handles authorization for users.
/// </summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// Checks whether the user is authorizated against a policy.
    /// </summary>
    /// <param name="userClaimsPrincipal">The claims principal for the user.</param>
    /// <param name="policy">The policy.</param>
    /// <returns>True if the user is authorized.</returns>
    public Task<bool> IsUserAuthorized(ClaimsPrincipal userClaimsPrincipal, string policy);
}