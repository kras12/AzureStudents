using AzureStudents.Data.DatabaseContexts;
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
}
