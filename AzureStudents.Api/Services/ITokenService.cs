namespace AzureStudents.Api.Services;

/// <summary>
/// Interface for a service that creates JWT tokens. 
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a token.
    /// </summary>
    /// <returns>A token in the form of a <see cref="string"/>.</returns>
    public string CreateToken();
}
