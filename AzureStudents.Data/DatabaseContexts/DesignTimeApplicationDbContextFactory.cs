using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using AzureStudents.Shared.Configuration;

namespace AzureStudents.Data.DatabaseContexts;

/// <summary>
/// A design time database context needed to support scaffolding when the database context class resides in a standalone project.
/// </summary>
public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    #region Methods

    /// <summary>
    /// Creates a new instance of a derived context.
    /// </summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(ConfigurationHelper.GetConnectionString());

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    #endregion
}
