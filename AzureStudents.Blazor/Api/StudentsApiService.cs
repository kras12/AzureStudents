using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Student;
using System.Net.Http.Json;

namespace AzureStudents.Blazor.Api;

/// <summary>
/// Service to communicate with the students API.
/// </summary>
public class StudentsApiService : IStudentsApiService
{
    #region Constants

    /// <summary>
    /// The relative API base address.
    /// </summary>
    private const string ApiBaseAddress = "students-api/students";

    /// <summary>
    /// The create student API endpoint address.
    /// </summary>
    private const string CreateStudentApiEndpoint = $"{ApiBaseAddress}";

    /// <summary>
    /// The delete student API endpoint address.
    /// </summary>
    private const string DeleteStudentApiEndpoint = $"{ApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The edit student API endpoint address.
    /// </summary>
    private const string EditStudentApiEndpoint = $"{ApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The get all students API endpoint address.
    /// </summary>
    private const string GetAllStudentsApiEndpoint = $"{ApiBaseAddress}";

    /// <summary>
    /// The get student by ID API endpoint address.
    /// </summary>
    private const string GetStudentByIdApiEndpoint = $"{ApiBaseAddress}/{IdPlaceHolder}";

    /// <summary>
    /// The ID placeholder used in API endpoint addresses.
    /// </summary>
    private const string IdPlaceHolder = "{id}";

    #endregion

    #region Fields

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
    public StudentsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        var result = await _httpClient.DeleteFromJsonAsync<ApiResponseDto<StudentDto>>(DeleteStudentApiEndpoint.Replace(IdPlaceHolder, studentId.ToString()));
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    /// <summary>
    /// Gets all students.
    /// </summary>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see cref="StudentDto"/> object if successful.</returns>
    public async Task<ApiResponseDto<List<StudentDto>>> GetAllStudentsAsync()
    {
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
        var response = await _httpClient.PutAsJsonAsync(EditStudentApiEndpoint.Replace(IdPlaceHolder, studentId.ToString()), student);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<StudentDto>>();
        return EnsureNotNull(result, "Failed to serialize the API response.");
    }

    #endregion

    #region OtherMethods

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
