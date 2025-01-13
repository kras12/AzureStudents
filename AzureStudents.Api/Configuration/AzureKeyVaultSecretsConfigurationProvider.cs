using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AzureStudents.Api.Configuration;

/// <summary>
/// Configuration provider for fetching secrets from Azure Key Vault. 
/// </summary>
public class AzureKeyVaultSecretsConfigurationProvider : ConfigurationProvider
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
    public AzureKeyVaultSecretsConfigurationProvider(Uri keyVaultUri, List<string> secretNames)
    {
        _keyVaultUri = keyVaultUri;
        _secretNames = secretNames;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override void Load()
    {
        Data = new Dictionary<string, string?>();
        var secretClient = new SecretClient(_keyVaultUri, new DefaultAzureCredential());

        foreach (var secretName in _secretNames)
        {
            var configNameParts = secretName.Split("--");
            string finalConfigName = string.Join(":", configNameParts);
            var secret = secretClient.GetSecret(secretName);
            
            Data.Add(finalConfigName, secret.Value.Value);
        }
    }

    #endregion
}
