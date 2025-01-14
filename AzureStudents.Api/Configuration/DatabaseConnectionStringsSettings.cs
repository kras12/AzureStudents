namespace AzureStudents.Api.Configuration;

/// <summary>
/// Settings class for database connection strings configurations.
/// </summary>
public class DatabaseConnectionStringsSettings
{
	#region Properties

	/// <summary>
	/// The connection string for the database. 
	/// </summary>
	public string AzureStudentsDatabaseConnectionString { get; set; } = "";

    #endregion
}
