using System;
using System.Text.RegularExpressions;

using RemaTe.Common.Enums;
using RemaTe.Shared.Controllers;

partial class Utils {
    public static bool IsValidEmail(string email) {
        // // Check if the string is null or empty
        // if (string.IsNullOrEmpty(email)) return false;
        //
        // // Check if the string contains only letters, digits, and @ symbol
        // Regex regex = new("^[a-zA-Z0-9@]+$");
        // Match match = regex.Match(email);
        // if (!match.Success) return false;
        //
        // // Check if the string has at least one dot after the @ symbol
        // int indexOfAtSymbol = email.IndexOf('@');
        // if (indexOfAtSymbol == -1 || indexOfAtSymbol >= email.Length - 2) return false;
        //
        // // Check if the domain part of the email address is not too long
        // string domainPart = email[(indexOfAtSymbol + 1)..];
        // return domainPart.Length <= 64;
        return true;
    }
    public static bool IsNumber(string str) {
        return int.TryParse(str, out _);
    }

    public static void ShowCommonErrorDialog(Gtk.Window parent, Errors error) {
        var dialog = Adw.MessageDialog.New(parent, "Accion Realizada", ":)");
        dialog.AddResponse("ok", "Ok");
        dialog.SetResponseAppearance("ok", Adw.ResponseAppearance.Default);
        dialog.SetCloseResponse("ok");

        switch (error) {
            case Errors.Ok:
                dialog.SetHeading("Accion Realizada");
                dialog.SetBody(":)");
                break;
            case Errors.NotFound:
                dialog.SetHeading("No encontrado");
                dialog.SetBody(":(");
                break;
            case Errors.Unauthorized:
                dialog.SetHeading("No autorizado");
                dialog.SetBody("D:");
                break;
            default:
                dialog.SetHeading("Error desconocido");
                dialog.SetBody("@~@");
                break;
        }
        dialog.Present();
    }

    public static void KeyboardShortcuts(Gtk.Window parent) {
        Gtk.Builder builder = new("shortcuts_dialog.ui");
        Gtk.ShortcutsWindow shortcutsWindow = (Gtk.ShortcutsWindow)builder.GetObject("_root");
        shortcutsWindow.SetTransientFor(parent);
        shortcutsWindow.SetIconName("emoji-nature-symbolic");
        shortcutsWindow.Present();
    }

    public static void About(Gtk.Window parent) {
        Adw.AboutWindow dialog = Adw.AboutWindow.New();
        dialog.SetTransientFor(parent);
        dialog.SetIconName("emoji-nature-symbolic");

        dialog.SetApplicationName(MainController.AppInfo.ShortName);
        dialog.SetApplicationIcon("emoji-nature-symbolic");
        dialog.SetVersion(MainController.AppInfo.Version);

        dialog.SetComments(MainController.AppInfo.Description);
        dialog.SetLicenseType(Gtk.License.Gpl30);

        dialog.SetDeveloperName("CYT");
        dialog.SetDevelopers(new[] { "NML <contacto@cyt.uy>" });
        dialog.SetDocumenters(new[] { "YAR", "BNV", "FX" });
        dialog.SetCopyright("© CYT 2023-2023");

        dialog.AddAcknowledgementSection(null, new[] {
            "UTU https://utumaldonado.edu.uy/",
            "Denaro https://github.com/NickvisionApps/Denaro",
            "Gir.Core https://github.com/gircore/gir.core",
            "GTK https://www.gtk.org/",
            "Libadwaita https://gnome.pages.gitlab.gnome.org/libadwaita/doc/1-latest/index.html"
            });

        dialog.SetWebsite(MainController.AppInfo.GitHubRepo.ToString());
        dialog.SetIssueUrl(MainController.AppInfo.IssueTracker.ToString());
        dialog.SetSupportUrl(MainController.AppInfo.SupportUrl.ToString());
        dialog.SetReleaseNotes(MainController.AppInfo.Changelog);
        dialog.Present();
    }
}
