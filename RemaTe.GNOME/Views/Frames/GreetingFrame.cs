using RemaTe.Shared.Controllers;

namespace RemaTe.GNOME.Views.Widgets;
static class GreetingFrame {
    public static Adw.ButtonContent New() {
        Adw.ButtonContent greeting = new();
        greeting.SetLabel(MainController.Greeting);
        greeting.SetIconName(MainController.ShowSun ? "weather-clear-symbolic" : "weather-clear-night-symbolic");
        Gtk.Image image = (Gtk.Image)greeting.GetFirstChild();
        image.SetPixelSize(48);
        image.SetMarginEnd(6);
        Gtk.Label label = (Gtk.Label)greeting.GetLastChild();
        label.AddCssClass("title-1");
        label.AddCssClass("greeting-title");
        greeting.SetHalign(Gtk.Align.Center);
        greeting.SetMarginTop(24);
        greeting.SetMarginBottom(14);
        return greeting;
    }
}
