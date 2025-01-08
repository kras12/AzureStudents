namespace AzureStudents.Shared.Dto.Api;

/// <summary>
/// An API error class that contains error information. 
/// </summary>
public class ApiErrorDto
{
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public ApiErrorDto()
    {

    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="errorType">The type of the error.</param>
    /// <param name="errorMessage">The error message.</param>
    public ApiErrorDto(string errorType, string errorMessage)
    {
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The error message.
    /// </summary>
    public string ErrorMessage { get; set; } = "";

    /// <summary>
    /// The type of the error.
    /// </summary>
    public string ErrorType { get; set; } = "";

    #endregion
}
