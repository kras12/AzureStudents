namespace AzureStudents.Shared.Dto.Authentication;

/// <summary>
/// Contains the response data for a successful login attempt.
/// </summary>
public class LoginResponseDto
{
    #region Constructors

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="applicationId">
    /// </param>
    /// <param name="token">The JWT token.</param>
    public LoginResponseDto(string applicationId, string token)
    {
        if (string.IsNullOrEmpty(applicationId))
        {
            throw new ArgumentException("Application ID can't be empty");
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("The JWT token can't be empty");
        }

        ApplicationId = applicationId;
        Token = token;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The application ID.
    /// </summary>
    public string ApplicationId { get; set; } = "";

    /// <summary>
    /// The JWT token.
    /// </summary>
    public string Token { get; set; } = "";

    #endregion
}
