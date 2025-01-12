using AzureStudents.Shared.Constants;
using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Authentication;
using AzureStudents.Shared.Dto.Student;
using AzureStudents.Shared.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace AzureStudents.Blazor.Api;

/// <summary>
/// Service to communicate with the students API.
/// </summary>
public class StudentsApiService : IStudentsApiService
{
    #region Constants

    /// <summary>
    /// The base address for the user authentication endpoint. 
    /// </summary>
    private const string AuthenticationBaseAddress = $"students-api/auth";

    /// <summary>
    /// The create student API endpoint address.
    /// </summary>
    private const string CreateStudentApiEndpoint = $"{StudentsApiBaseAddress}";

    /// <summary>
    /// The delete student API endpoint address.
    /// </summary>
    private const string DeleteStudentApiEndpoint = $"{StudentsApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The edit student API endpoint address.
    /// </summary>
    private const string EditStudentApiEndpoint = $"{StudentsApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The get all students API endpoint address.
    /// </summary>
    private const string GetAllStudentsApiEndpoint = $"{StudentsApiBaseAddress}";

    /// <summary>
    /// The get student by ID API endpoint address.
    /// </summary>
    private const string GetStudentByIdApiEndpoint = $"{StudentsApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The ID placeholder used in API endpoint addresses.
    /// </summary>
    private const string IdPlaceHolder = "{id}";

    /// <summary>
    /// The user login endpoint. 
    /// </summary>
    private const string LoginApiEndpoint = $"{AuthenticationBaseAddress}/login";

    /// <summary>
    /// The relative API base address.
    /// </summary>
    private const string StudentsApiBaseAddress = "students-api/students";

    #endregion

    #region Fields

    /// <summary>
    /// The injected authentication state provider.
    /// </summary>
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    /// <summary>
    /// The injected HTTP client.
    /// </summary>
    private readonly HttpClient _httpClient;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="httpClient"> The injected HTTP client.</param>
    /// <param name="authenticationStateProvider">The injected authentication state provider.</param>
    public StudentsApiService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    #endregion

    #region ApiEndpointMethods

    /// <summary>
    /// Creates a student.
    /// </summary>
    /// <param name="createStudentDto">The student input. </param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    public async Task<ApiResponseDto<StudentDto>> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        if (!await EnsureAuthenticated())
        {
            return ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, "API authentication failed.");
        }

        var response = await _httpClient.PostAsJsonAsync(CreateStudentApiEndpoint, createStudentDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<StudentDto>>();
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Deletes a student.
    /// </summary>
    /// <param name="studentId">The ID of the student to delete.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> that contains the result of the operation.</returns>
    public async Task<ApiResponseDto<StudentDto>> DeleteStudentAsync(int studentId)
    {
        if (!await EnsureAuthenticated())
        {
            return ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, "API authentication failed.");
        }

        var result = await _httpClient.DeleteFromJsonAsync<ApiResponseDto<StudentDto>>(DeleteStudentApiEndpoint.Replace(IdPlaceHolder, studentId.ToString()));
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Gets all students.
    /// </summary>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see cref="StudentDto"/> object if successful.</returns>
    public async Task<ApiResponseDto<List<StudentDto>>> GetAllStudentsAsync()
    {
        if (!await EnsureAuthenticated())
        {
            return ApiResponseDto<List<StudentDto>>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, "API authentication failed.");
        }

        var result = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<StudentDto>>>(GetAllStudentsApiEndpoint);
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Gets a student by ID.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    public async Task<ApiResponseDto<StudentDto>> GetStudentByIdAsync(int studentId)
    {
        if (!await EnsureAuthenticated())
        {
            return ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, "API authentication failed.");
        }

        var result = await _httpClient.GetFromJsonAsync<ApiResponseDto<StudentDto>>(GetStudentByIdApiEndpoint.Replace(IdPlaceHolder, studentId.ToString()));
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Edits a student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="student">The updated data for the stduent.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    public async Task<ApiResponseDto<StudentDto>> UpdateStudentAsync(int studentId, UpdateStudentDto student)
    {
        if (!await EnsureAuthenticated())
        {
            return ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, "API authentication failed.");
        }

        var response = await _httpClient.PutAsJsonAsync(EditStudentApiEndpoint.Replace(IdPlaceHolder, studentId.ToString()), student);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<StudentDto>>();
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Attempts to login.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>An <see cref="ApiValueResponseDto{T}"/> containing a <see cref="LoginUserResponseDto"/> object if successful.</returns>
    private async Task<ApiResponseDto<LoginResponseDto>> Login()
    {
        var response = await _httpClient.PostAsJsonAsync(LoginApiEndpoint, new LoginDto(ApplicationConstants.FrontEndApplicationId));
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<LoginResponseDto>>();

        if (result != null && result.Success)
        {
            await ((ApiUserAuthenticationStateProvider)_authenticationStateProvider).SetTokenAsync(result.Value!.Token);
        }

        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    #endregion

    #region OtherMethods

    /// <summary>
    /// Attempts to make sure that the application is logged in and that the authorization header is set. 
    /// </summary>
    /// <returns><see cref="Task{TResult}"/> with value true if logged in.</returns>
    private async Task<bool> EnsureAuthenticated()
    {
        string? token = await ((ApiUserAuthenticationStateProvider)_authenticationStateProvider).GetTokenAsync();

        if (token == null)
        {
            var loginResult = await Login();

            if (loginResult.Success && loginResult.Value != null)
            {
                token = loginResult.Value.Token;
            }
        }

        if (token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks the value of an object reference and throws an exception if it's null. 
    /// If it has a value the object is returned. 
    /// </summary>
    /// <param name="targetObject">The object to check.</param>
    /// <param name="exceptionMessage">An optional message to use for the exception.</param>
    /// <returns>The object if it's not null.</returns>
    /// <exception cref="Exception"></exception>
    private T EnsureNotNull<T>(T? targetObject, string? exceptionMessage = null) where T : class
    {
        if (targetObject == null)
        {
            throw new Exception(exceptionMessage);
        }

        return targetObject;
    }
    #endregion
}
