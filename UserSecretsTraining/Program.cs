using Azure;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

IConfigurationRoot configRoot = builder.Configuration;

var keyVaultUrl = configRoot["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    try
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }
    catch (Exception ex) when (ex is AuthenticationFailedException or RequestFailedException or AggregateException)
    {
        // Network failures arrive as an AggregateException after the SDK's retries; show the first underlying error
        var reason = ex is AggregateException agg ? agg.InnerExceptions[0] : ex;
        AnsiConsole.MarkupLine($"[bold yellow]Couldn't load secrets from Key Vault at {Markup.Escape(keyVaultUrl)}.[/]");
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(reason.Message.Split('\n')[0].Trim())}[/]");
        AnsiConsole.MarkupLine("[yellow]Make sure you're signed in (az login or Visual Studio) and have access to the vault. Continuing without it.[/]");
        AnsiConsole.WriteLine();
    }
}

AnsiConsole.MarkupLine("[bold green]Here's your current configuration:[/]");

var table = new Table()
    .AddColumn("[bold]Key[/]")
    .AddColumn("[bold]Value[/]")
    .AddColumn("[bold]Source[/]")
    .AddRow("[yellow]NotSecretSetting[/]", configRoot["NotSecretSetting"] ?? "", Markup.Escape(GetSource("NotSecretSetting")))
    .AddRow("[yellow]SecretApiKey[/]", $"[red]{Markup.Escape(configRoot["SecretApiKey"] ?? "")}[/]", Markup.Escape(GetSource("SecretApiKey")));

AnsiConsole.Write(table);

string GetSource(string key)
{
    foreach (var provider in configRoot.Providers.Reverse())
    {
        if (provider.TryGet(key, out _))
            return provider.ToString() ?? "Unknown";
    }
    return "Not found";
}
