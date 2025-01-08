using AzureStudents.Data.Entities;

namespace AzureStudents.Data.Repositories;

/// <summary>
/// Interface for student repository.
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Adds a student.
    /// </summary>
    /// <param name="student">The student to add.</param>
    /// <returns>A <see cref="Task"/> object containing the added student.</returns>
    Task<Student> AddAsync(Student student);

    /// <summary>
    /// Returns true if there are any students in the database.
    /// </summary>
    /// <returns>A <see cref="Task"/> object containing true if there are any students in the database.</returns>
    Task<bool> AnyAsync();

    /// <summary>
    /// Deletes a student from the database.
    /// </summary>
    /// <param name="id">The ID of the student to delete.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"></exception>
    Task DeleteAsync(int id);

    /// <summary>
    /// Returns all students in the database.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}{T}"/> that contains a collection of <see cref="Student"/>.</returns>
    public Task<List<Student>> GetAll();

    /// <summary>
    /// Attempts to fetch a student by ID.
    /// </summary>
    /// <remarks>Returned entities will not be tracked by EF Core.</remarks>
    /// <param name="id">The ID of the student.</param>
    /// <returns>A <see cref="Task"/> object containing the student if found or null if not found.</returns>
    Task<Student?> GetByIdAsync(int id);

    /// <summary>
    /// Checks whether a student with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the student.</param>
    /// <returns>A <see cref="Task"/> object containing true if there was a matching student.</returns>
    Task<bool> StudentExists(int id);

    /// <summary>
    /// Updates a student.
    /// </summary>
    /// <param name="student">The student to update.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    Task UpdateAsync(Student student);
}
