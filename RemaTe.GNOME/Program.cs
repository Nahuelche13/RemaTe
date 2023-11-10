using System;

using RemaTe.GNOME.Views;

namespace RemaTe.GNOME;

public partial class Program {
    private readonly Adw.Application application;
    private MainWindow? mainWindow = null;

    public static int Main() => new Program().Run();

    public Program() {
        application = Adw.Application.New("uy.cyt.remate", Gio.ApplicationFlags.FlagsNone);
        application.OnActivate += OnActivate;
    }

    public int Run() => application.Run();

    private void OnActivate(Gio.Application sender, EventArgs e) {
        //Main Window
        mainWindow = new MainWindow(application);
        mainWindow.Startup();
    }
}
