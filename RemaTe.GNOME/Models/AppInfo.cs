using System;

using RemaTe.Common.Models;

namespace RemaTe.Shared.Models;

/// <summary>
/// A model for the information about the application
/// </summary>
public record class AppInfo {
    /// <summary>
    /// Gets the singleton object
    /// </summary>
    internal static AppInfo Current { get; } = new();

    /// <summary>
    /// The id of the application
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// The name of the application
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The short name of the application
    /// </summary>
    public string ShortName { get; set; }
    /// <summary>
    /// The description of the application
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// The running version of the application
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// The changelog for the running version of the application
    /// </summary>
    public string Changelog { get; set; }
    /// <summary>
    /// The GitHub repo for the application
    /// </summary>
    public Uri GitHubRepo { get; set; }
    /// <summary>
    /// The issue tracker url for the application
    /// </summary>
    public Uri IssueTracker { get; set; }
    /// <summary>
    /// The support url for the application
    /// </summary>
    public Uri SupportUrl { get; set; }

    /// <summary>
    /// Constructs an AppInfo
    /// </summary>
    internal AppInfo() {
        ID = "uy.cyt.remate";
        Name = "CYT RemaTe";
        ShortName = "RemaTe";
        Description = "Administra tu empresa de remates";
        Version = "2023.11.0";
        Changelog = "<ul><li>Pasaron cosas...</li></ul>";
        GitHubRepo = new Uri("https://github.com/Nahuelche13/RemaTe");
        IssueTracker = new Uri("https://github.com/Nahuelche13/RemaTe/issues/new");
        SupportUrl = new Uri("https://github.com/Nahuelche13/RemaTe/discussions");
    }

    /// <summary>
    /// Gets whether or not the application version is a development version or not
    /// </summary>
    /// <returns>True for development version, else false</returns>
    public bool GetIsDevelVersion() => Version.Contains('-');

    public static string[] Departamentos = {
        "Treinta y tres",
        "Tacuarembo",
        "Soriano",
        "San Jose",
        "Rocha",
        "Rivera",
        "Rio Negro",
        "Paysandu",
        "Montevideo",
        "Maldonado",
        "Lavalleja",
        "Florida",
        "Flores",
        "Durazno",
        "Colonia",
        "Cerro",
        "Canelones",
        "Artigas",
        "Salto",
    };

    public static string[] EspeciesAnimales = {
        "Vacas",
        "Toros",
        "Obejas"
    };
}
