namespace AzureStudents.Shared.Dto.Authentication;

/// <summary>
/// Contains login credentials. 
/// </summary>
public class LoginDto
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="applicationId">The application ID.</param>
    public LoginDto(string applicationId)
    {
        ApplicationId = applicationId;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The ID of the application. 
    /// </summary>
    public string ApplicationId { get; set; } = "";

    #endregion
}
