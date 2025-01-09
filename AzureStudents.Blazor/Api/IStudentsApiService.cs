using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Student;

namespace AzureStudents.Blazor.Api;

/// <summary>
/// Service to communicate with the student API.
/// </summary>
public interface IStudentsApiService
{
    /// <summary>
    /// Creates a student.
    /// </summary>
    /// <param name="createStudentDto">The student input. </param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    Task<ApiResponseDto<StudentDto>> CreateStudentAsync(CreateStudentDto createStudentDto);

    /// <summary>
    /// Deletes a student.
    /// </summary>
    /// <param name="studentId">The ID of the student to delete.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> that contains the result of the operation.</returns>
    Task<ApiResponseDto<StudentDto>> DeleteStudentAsync(int studentId);

    /// <summary>
    /// Gets all students.
    /// </summary>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see cref="StudentDto"/> objects if successful.</returns>
    Task<ApiResponseDto<List<StudentDto>>> GetAllStudentsAsync();

    /// <summary>
    /// Gets a student by ID.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    Task<ApiResponseDto<StudentDto>> GetStudentByIdAsync(int studentId);

    /// <summary>
    /// Edits a student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="student">The updated data for the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing a <see cref="StudentDto"/> object if successful.</returns>
    Task<ApiResponseDto<StudentDto>> UpdateStudentAsync(int studentId, UpdateStudentDto student);
}
