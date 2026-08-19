using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

// builder.Services.AddSingleton<MyService>(); // register services here

var app = builder.Build();

IConfiguration config = app.Services.GetRequiredService<IConfiguration>();

AnsiConsole.MarkupLine("[bold green]Here's your current configuration:[/]");

var table = new Table()
    .AddColumn("[bold]Key[/]")
    .AddColumn("[bold]Value[/]")
    .AddRow("[yellow]NotSecretSetting[/]", config["NotSecretSetting"] ?? "")
    .AddRow("[yellow]SecretApiKey[/]", $"[red]{Markup.Escape(config["SecretApiKey"] ?? "")}[/]");

AnsiConsole.Write(table);
