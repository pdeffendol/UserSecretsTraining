using System.ComponentModel.DataAnnotations;

// The settings this app reads, bound from the root of configuration. appsettings.json lists every key for
// discoverability; the attributes here are what actually enforce them at startup.
public class AppSettings
{
    public string NotSecretSetting { get; set; } = "";

    [Required]
    public string SecretApiKey { get; set; } = "";
}
