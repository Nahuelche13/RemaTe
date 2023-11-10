using System.IO;
using System.Reflection;
using System.Xml;

// using static RemaTe.GNOME.Helpers.Gettext;

namespace RemaTe.GNOME.Helpers;

/// <summary>
/// This class is used for creating a Gtk.Builder from an embedded resource and replace
/// all translatable strings with the localized version
/// </summary>
public class Builder {
    /// <summary>
    /// Creates a Gtk.Builder from an embedded resource and replaces all translatable strings with the localized version
    /// </summary>
    /// <param name="name">The name of the embedded resource</param>
    /// <returns>Gtk.Builder</returns>
    public static Gtk.Builder FromFile(string name) {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        using var reader = new StreamReader(stream!);
        var uiContents = reader.ReadToEnd();
        var xml = new XmlDocument();
        xml.LoadXml(uiContents);
        // var elements = xml.GetElementsByTagName("*");
        // foreach (XmlElement element in elements) {
        //     if (element.HasAttribute("translatable")) {
        //         element.RemoveAttribute("translatable");
        //         if (element.HasAttribute("context")) {
        //             var context = element.GetAttribute("context");
        //             element.InnerText = P(context, element.InnerText);
        //         }
        //         else {
        //             element.InnerText = _(element.InnerText);
        //         }
        //     }
        // }
        return Gtk.Builder.NewFromString(xml.OuterXml, -1);
    }
}
