using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using RemaTe.Shared.Controllers;

namespace RemaTe.GNOME.Views.Widgets;
static class GreetingFrame {
    private static readonly HttpClient Client = new();
    public static Gtk.Box New() {

        var box = Gtk.Box.New(Gtk.Orientation.Horizontal, 24);
        var textBox = Gtk.Box.New(Gtk.Orientation.Vertical, 6);

        var weatherIcon = Gtk.Image.NewFromIconName(MainController.ShowSun ? "weather-clear-symbolic" : "weather-clear-night-symbolic");
        var greetingLabel = Gtk.Label.New(MainController.Greeting);
        var temperatureLabel = Gtk.Label.New("Cargando...");

        weatherIcon.SetPixelSize(48);

        greetingLabel.AddCssClass("title-1");
        temperatureLabel.AddCssClass("title-2");

        box.Append(weatherIcon);

        textBox.Append(greetingLabel);
        textBox.Append(temperatureLabel);

        box.Append(textBox);
        box.SetValign(Gtk.Align.Center);
        box.SetHalign(Gtk.Align.Center);
        try {
            Task<object?> decoded = System.Net.Http.Json.HttpClientJsonExtensions.GetFromJsonAsync(Client, "https://api.open-meteo.com/v1/forecast?latitude=-34.9&longitude=-54.95&current=temperature_2m,weather_code", typeof(Dictionary<string, dynamic>));
            _ = decoded.ContinueWith((task) => {
                try {
                    if (task.IsCompleted) {
                        var decoded = (Dictionary<string, dynamic>)task.Result;

                        JsonElement current = decoded["current"];
                        var currentDictionary = current.Deserialize<Dictionary<string, dynamic>>();

                        JsonElement weatherCode = currentDictionary["weather_code"];
                        JsonElement temperature = currentDictionary["temperature_2m"];

                        temperatureLabel.SetText(temperature + "°C");

                        switch (weatherCode.GetInt32(), MainController.ShowSun) {
                            case (0, true): weatherIcon.SetFromIconName("weather-clear-symbolic"); break;
                            case (0, false): weatherIcon.SetFromIconName("weather-clear-night-symbolic"); break;
                            case ( < 3, true): weatherIcon.SetFromIconName("weather-few-clouds-symbolic"); break;
                            case ( < 3, false): weatherIcon.SetFromIconName("weather-few-clouds-night-symbolic"); break;
                            case ( < 48, _): weatherIcon.SetFromIconName("weather-fog-symbolic"); break;
                            case ( < 67, _): weatherIcon.SetFromIconName("weather-showers-symbolic"); break;
                            case ( < 77, _): weatherIcon.SetFromIconName("weather-snow-symbolic"); break;
                            case ( < 101, _): weatherIcon.SetFromIconName("weather-storm-symbolic"); break;
                            default: weatherIcon.SetFromIconName("weather-clear-symbolic"); break;
                        }
                    }
                }
                catch (System.Exception) {
                    temperatureLabel.SetVisible(false);
                }
            });
        }
        catch { }

        return box;
    }
}
