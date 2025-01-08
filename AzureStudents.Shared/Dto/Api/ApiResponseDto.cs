using AzureStudents.Shared.Enums;
using System.Text.Json.Serialization;

namespace AzureStudents.Shared.Dto.Api;

/// <summary>
/// Generic API response class. 
/// </summary>
public class ApiResponseDto<T> where T : class
{
    #region Constructors

    /// <summary>
    /// A constructor intended for JSON deserializers.
    /// </summary>
    [JsonConstructor]
    private ApiResponseDto()
    {

    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="success">True if the operation was successful.</param>
    private ApiResponseDto(bool success)
    {
        Success = success;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="success">True if the operation was successful.</param>
    /// <param name="value">The value of the response.</param>
    private ApiResponseDto(bool success, T? value)
    {
        Success = success;
        Value = value;
    }

    /// <summary>
    /// Constructor for an error response.
    /// </summary>
    /// <param name="errors">A collection of errors.</param>
    private ApiResponseDto(List<ApiErrorDto> errors)
    {
        Errors = errors;
        Success = false;
    }

    /// <summary>
    /// Constructor for an error response.
    /// </summary>
    /// <param name="errorType">The type of error.</param>
    /// <param name="errorMessage">The error message.</param>
    private ApiResponseDto(string errorType, string errorMessage)
        : this(new List<ApiErrorDto>() { new ApiErrorDto(errorType, errorMessage) })
    {

    }

    #endregion

    #region Properties

    /// <summary>
    /// A collection of errors. 
    /// </summary>
    [JsonInclude]
    public List<ApiErrorDto> Errors { get; private set; } = new();

    /// <summary>
    /// True if operation was successful.
    /// </summary>
    [JsonInclude]
    public bool Success { get; private set; }

    /// <summary>
    /// The value for a successful response.
    /// </summary>
    [JsonInclude]
    public T? Value { get; private set; }

    #endregion

    #region FactoryMethods

    /// <summary>
    /// Creates an error response.
    /// </summary>
    /// <param name="errorType">The type of error.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns><see cref="ApiResponseDto"/> containing the supplied errors.</returns>
    public static ApiResponseDto<T> CreateErrorResponse(ApiErrorMessageTypes errorType, string errorMessage)
    {
        return CreateErrorResponse(new List<ApiErrorDto> { new ApiErrorDto(errorType.ToString(), errorMessage) });
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    /// <param name="errors">A collection of errors having labels and descriptions.</param>
    /// <returns><see cref="ApiResponseDto"/> containing the supplied errors.</returns>
    public static ApiResponseDto<T> CreateErrorResponse(List<ApiErrorDto> errors)
    {
        return new ApiResponseDto<T>(errors);
    }

    /// <summary>
    /// Creates a successful response having a value of type <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the response value</typeparam>
    /// <param name="value">The response value.</param>
    /// <returns><see cref="ApiResponseDto"/> containing the supplied response value.</returns>
    public static ApiResponseDto<T> CreateSuccessfulResponse(T? value = null)
    {
        return new ApiResponseDto<T>(success: true, value);
    }

    #endregion
}
