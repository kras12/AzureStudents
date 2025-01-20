using AzureStudents.Data.DatabaseContexts;
using AzureStudents.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureStudents.Test.Tests;

/// <summary>
/// Base class for test classes.
/// </summary>
public class TestBase
{
	#region Fields

    /// <summary>
    /// The application DB context.
    /// </summary>
	protected ApplicationDbContext _applicationDbContext;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestingMemoryDb-{Guid.NewGuid()}")
            .Options;

        _applicationDbContext = new ApplicationDbContext(options);
        _applicationDbContext.Database.EnsureCreated();
    }

    #endregion

    #region HelperMethods

    /// <summary>
    /// Creates the default student. 
    /// </summary>
    /// <returns><see cref="Student"/></returns>
    protected Student CreateDefaultStudent()
    {
        return new Student()
        {
            FirstName = "Kalle",
            LastName = "Anka",
        };
    }

    #endregion
}
