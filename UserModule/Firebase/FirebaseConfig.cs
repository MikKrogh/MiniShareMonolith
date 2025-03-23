using System.Text.Json.Serialization;

namespace UserModule.Firebase;

internal class FirebaseConfig
{
    [JsonPropertyName("type"), ConfigurationKeyName("type")] public string? Type { get; set; }
    [JsonPropertyName("project_id"), ConfigurationKeyName("project_id")] public string? ProjectId { get; set; }
    [JsonPropertyName("private_key_id"), ConfigurationKeyName("private_key_id")] public string? PrivateKeyId { get; set; }
    [JsonPropertyName("private_key"), ConfigurationKeyName("private_key")] public string? PrivateKey { get; set; }
    [JsonPropertyName("client_email"), ConfigurationKeyName("client_email")] public string? ClientEmail { get; set; }
    [JsonPropertyName("client_id"), ConfigurationKeyName("client_id")] public string? ClientId { get; set; }
    [JsonPropertyName("auth_uri"), ConfigurationKeyName("auth_uri")] public string? AuthUri { get; set; }
    [JsonPropertyName("token_uri"), ConfigurationKeyName("token_uri")] public string? TokenUri { get; set; }
    [JsonPropertyName("auth_provider_x509_cert_url"), ConfigurationKeyName("auth_provider_x509_cert_url")] public string? AuthProviderX509CertUrl { get; set; }
    [JsonPropertyName("client_x509_cert_url"), ConfigurationKeyName("client_x509_cert_url")] public string? ClientX509CertUrl { get; set; }
    [JsonPropertyName("universe_domain"), ConfigurationKeyName("universe_domain")] public string? universeDomain { get; set; }

}
