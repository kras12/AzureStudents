using AzureStudents.Data.DatabaseContexts;
using AzureStudents.Data.Entities;
using AzureStudents.Data.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AzureStudents.Data.Repositories;

/// <summary>
/// Repository for students. 
/// </summary>
public class StudentRepository : IStudentRepository
{
    #region Fields

    /// <summary>
    /// The injected database context.
    /// </summary>
    private readonly ApplicationDbContext _databaseContext;

    #endregion

    #region Constructors

    /// <summary>
    /// A constructor.
    /// </summary>
    /// <param name="databaseContext">The injected database context.</param>
    public StudentRepository(ApplicationDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a student.
    /// </summary>
    /// <param name="student">The student to add.</param>
    /// <returns>A <see cref="Task"/> object containing the added student.</returns>
    public async Task<Student> AddAsync(Student student)
    {
        _databaseContext.Students.Add(student);
        await _databaseContext.SaveChangesAsync();
        return student;
    }

    /// <summary>
    /// Returns true if there is any students in the database.
    /// </summary>
    /// <returns>A <see cref="Task"/> object containing true if there is any students in the database.</returns>
    public Task<bool> AnyAsync()
    {
        return _databaseContext.Students.AnyAsync();
    }

    /// <summary>
    /// Deletes a student from the database.
    /// </summary>
    /// <param name="id">The ID of the student to delete.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteAsync(int id)
    {
        try
        {
            var student = _databaseContext.Students.SingleOrDefault(x => x.Id == id);

            if (student == null)
            {
                throw new EntityNotFoundException($"The student with ID '{id}' was not found.");
            }

            _databaseContext.Students.Remove(student);
            await _databaseContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EntityNotFoundException($"The student with ID '{id}' was not found.");
        }
    }

    /// <summary>
    /// Returns all students in the database.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}{T}"/> that contains a collection of <see cref="Student"/>.</returns>
    public async Task<List<Student>> GetAll()
    {
        return await _databaseContext.Students.ToListAsync();
    }

    /// <summary>
    /// Attempts to fetch a student by ID.
    /// </summary>
    /// <remarks>Returned entities will not be tracked by EF Core.</remarks>
    /// <param name="id">The ID of the student.</param>
    /// <returns>A <see cref="Task"/> object containing the student if found or null if not found.</returns>
    public Task<Student?> GetByIdAsync(int id)
    {
        return _databaseContext.Students.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
    }

    /// <summary>
    /// Checks whether a student with the specified ID exists. 
    /// </summary>
    /// <param name="id">The ID for the student.</param>
    /// <returns>A <see cref="Task"/> object containing true if there was a matching student.</returns>
    public Task<bool> StudentExists(int id)
    {
        return _databaseContext.Students.AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Updates a student.
    /// </summary>
    /// <param name="student">The student to update.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async Task UpdateAsync(Student student)
    {
        _databaseContext.ChangeTracker.Clear();
        _databaseContext.Students.Update(student);
        int numberUpdated = await _databaseContext.SaveChangesAsync();

        if (numberUpdated == 0)
        {
            throw new EntityNotFoundException($"The student with ID '{student.Id}' was not found.");
        }
    }

    #endregion
}
