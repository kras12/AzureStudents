using Microsoft.AspNetCore.Components.Authorization;

namespace AzureStudents.Blazor.Api;

/// <summary>
/// An interface for a service that provides information about the authentication state of the current user.
/// </summary>
public interface IApiUserAuthenticationStateProvider
{
    /// <summary>
    /// Gets an authentication state that describes the user. 
    /// </summary>
    /// <returns>A <see cref="Task"/> that contains the <see cref="AuthenticationState"/>.</returns>
    public Task<AuthenticationState> GetAuthenticationStateAsync();

    /// <summary>
    /// Gets the token from local storage. 
    /// </summary>
    /// <returns>A <see cref="Task"/> that contains token as a <see cref="string"/>.</returns>
    public Task<string?> GetTokenAsync();

    /// <summary>
    /// Removes the token from local storage. 
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents an async operation.</returns>
    public Task RemoveTokenAsync();

    /// <summary>
    /// Stores the token in local storage.
    /// </summary>
    /// <param name="token">The token to store.</param>
    /// <returns>A <see cref="Task"/> that represents an async operation.</returns>
    public Task SetTokenAsync(string token);
}
