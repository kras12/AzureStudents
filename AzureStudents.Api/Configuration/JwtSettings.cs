namespace AzureStudents.Api.Configuration;

/// <summary>
/// Settings class for JWT configurations. 
/// </summary>
public class JwtSettings
{
    #region Properties

    /// <summary>
    /// The audience.
    /// </summary>
    public string Audience { get; set; } = "";

    /// <summary>
    /// The issuer.
    /// </summary>
    public string Issuer { get; set; } = "";

    /// <summary>
    /// The signing key.
    /// </summary>
    public string SigningKey { get; set; } = "";

    #endregion
}
