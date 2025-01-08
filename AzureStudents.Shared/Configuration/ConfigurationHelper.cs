using Microsoft.Extensions.Configuration;

namespace AzureStudents.Shared.Configuration;

/// <summary>
/// Reads configuration files from disk and provides values for various settings. 
/// </summary>
public static class ConfigurationHelper
{
    #region Fields

    /// <summary>
    /// Contains the configuration.
    /// </summary>
    private static IConfigurationRoot _configurationRoot;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    static ConfigurationHelper()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
            ?? "Development";

        _configurationRoot = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();
    }

    #endregion

    #region Methods
    
    /// <summary>
    /// Returns the connection string for the database.
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public static string GetConnectionString()
    {
        return _configurationRoot.GetConnectionString("DefaultConnectionString") 
            ?? throw new KeyNotFoundException("The connection string was not found.");
    }

    #endregion
}
