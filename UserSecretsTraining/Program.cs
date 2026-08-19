using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

// builder.Services.AddSingleton<MyService>(); // register services here

var app = builder.Build();

IConfiguration config = app.Services.GetRequiredService<IConfiguration>();
IConfigurationRoot configRoot = (IConfigurationRoot)config;

AnsiConsole.MarkupLine("[bold green]Here's your current configuration:[/]");

var table = new Table()
    .AddColumn("[bold]Key[/]")
    .AddColumn("[bold]Value[/]")
    .AddColumn("[bold]Source[/]")
    .AddRow("[yellow]NotSecretSetting[/]", config["NotSecretSetting"] ?? "", Markup.Escape(GetSource("NotSecretSetting")))
    .AddRow("[yellow]SecretApiKey[/]", $"[red]{Markup.Escape(config["SecretApiKey"] ?? "")}[/]", Markup.Escape(GetSource("SecretApiKey")));

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
