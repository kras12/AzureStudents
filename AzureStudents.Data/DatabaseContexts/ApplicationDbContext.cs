using Microsoft.EntityFrameworkCore;
using AzureStudents.Data.Entities;

namespace AzureStudents.Data.DatabaseContexts;

/// <summary>
/// The database context for the application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    #region Constructors

    /// <summary>
    /// A constructor.
    /// </summary>
    /// <param name="options">The options to use.</param>
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    #endregion

    #region Properties

    /// <summary>
    /// The database set for students.
    /// </summary>
    public DbSet<Student> Students { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Configures the options.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: true);
        }
    }

    /// <summary>
    /// Configures the database model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
    }

    #endregion
}