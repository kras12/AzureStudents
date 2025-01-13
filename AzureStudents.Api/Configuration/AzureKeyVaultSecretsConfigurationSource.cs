namespace AzureStudents.Api.Configuration;

/// <summary>
/// Configuration source for fetching secrets from Azure Key Vault. 
/// </summary>
public class AzureKeyVaultSecretsConfigurationSource : IConfigurationSource
{
    #region Fields

    /// <summary>
    /// The URI for the key vault.
    /// </summary>
	private readonly Uri _keyVaultUri;

    /// <summary>
    /// A collection of names for the secrets to load. 
    /// </summary>
	private readonly List<string> _secretNames;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="keyVaultUri">The URI for the key vault.</param>
    /// <param name="secretNames">A collection of names for the secrets to load. </param>
    public AzureKeyVaultSecretsConfigurationSource(Uri keyVaultUri, List<string> secretNames)
    {
        _keyVaultUri = keyVaultUri;
        _secretNames = secretNames;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AzureKeyVaultSecretsConfigurationProvider(_keyVaultUri, _secretNames);
    }

    #endregion
}
