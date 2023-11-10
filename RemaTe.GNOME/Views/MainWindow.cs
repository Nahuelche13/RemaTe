using System;
using System.Diagnostics;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.GNOME.Views.Widgets;
using RemaTe.Logic;
using RemaTe.Shared.Controllers;

namespace RemaTe.GNOME.Views;

public partial class MainWindow : Adw.ApplicationWindow {
    private readonly Adw.Application application;

#pragma warning disable 649
    [Gtk.Connect] private readonly Adw.WindowTitle _windowTitle;
    [Gtk.Connect] private readonly Gtk.Button _loginButton;
    [Gtk.Connect] private readonly Gtk.Button _logoutButton;
    [Gtk.Connect] private readonly Adw.Bin _content;
    [Gtk.Connect] private readonly Gtk.Button _lstArtButton;
    [Gtk.Connect] private readonly Gtk.Button _lstLteButton;
    [Gtk.Connect] private readonly Gtk.Button _lstRmteButton;
    [Gtk.Connect] private readonly Gtk.Button _lstUrsButton;
    [Gtk.Connect] private readonly Gtk.Button _crtAniButton;
    [Gtk.Connect] private readonly Gtk.Button _crtMaqButton;
    [Gtk.Connect] private readonly Gtk.Button _crtOtrButton;
    [Gtk.Connect] private readonly Gtk.Button _crtLteButton;
    [Gtk.Connect] private readonly Gtk.Button _crtRmteButton;
    [Gtk.Connect] private readonly Gtk.Button _crtUsrButton;
    [Gtk.Connect] private readonly Gtk.Image _logo;
#pragma warning restore 649


    public MainWindow(Adw.Application application) : this(Builder.FromFile("window.ui"), application) { }
    private MainWindow(Gtk.Builder builder, Adw.Application application) : base(builder.GetPointer("_root"), false) {
        //Window Settings
        this.application = application;

        //Build UI
        builder.Connect(this);
        SetTitle(MainController.AppInfo.ShortName);
        SetIconName("emoji-nature-symbolic");

        OnCloseRequest += OnCloseRequested;

        //Header Bar
        _windowTitle.SetTitle(MainController.AppInfo.ShortName);

        _logo.SetFromFile("Resources/logo-cyt.svg");

        //Greeting
        _content.SetChild(GreetingFrame.New());

        #region Config Actions
        //New Window Action
        var actNewWindow = Gio.SimpleAction.New("newWindow", null);
        actNewWindow.OnActivate += (sender, e) => Process.Start(new ProcessStartInfo(Environment.ProcessPath) { UseShellExecute = true });
        AddAction(actNewWindow);
        //Keyboard Shortcuts Action
        var actKeyboardShortcuts = Gio.SimpleAction.New("keyboardShortcuts", null);
        actKeyboardShortcuts.OnActivate += (_, __) => Utils.KeyboardShortcuts(this);
        AddAction(actKeyboardShortcuts);
        application.SetAccelsForAction("win.keyboardShortcuts", new string[] { "<Ctrl>question" });
        //Quit Action
        var actQuit = Gio.SimpleAction.New("quit", null);
        actQuit.OnActivate += (sender, e) => this.application.Quit();
        AddAction(actQuit);
        application.SetAccelsForAction("win.quit", new string[] { "<Ctrl>q" });
        //Help Action
        var actHelp = Gio.SimpleAction.New("help", null);
        actHelp.OnActivate += (sender, e) => Gtk.Functions.ShowUri(this, "file:/var/home/potato/Documentos/remate.html", 0);
        AddAction(actHelp);
        application.SetAccelsForAction("win.help", new string[] { "F1" });
        //About Action
        var actAbout = Gio.SimpleAction.New("about", null);
        actAbout.OnActivate += (_, __) => Utils.About(this);
        AddAction(actAbout);
        #endregion

        Usuario.I.OnAuth += () => {
            if (Usuario.I.Auth != null) {
                _loginButton.SetVisible(false);
                _logoutButton.SetVisible(true);

                // _crtArtButton.SetVisible(true);
                _crtUsrButton.SetVisible(true);
                _crtLteButton.SetVisible(true);
                _crtRmteButton.SetVisible(true);

                _lstUrsButton.SetVisible(true);

                _content.SetChild(GreetingFrame.New());
            }
            else {
                _loginButton.SetVisible(true);
                _logoutButton.SetVisible(false);

                // _crtArtButton.SetVisible(false);
                _crtUsrButton.SetVisible(false);
                _crtLteButton.SetVisible(false);
                _crtRmteButton.SetVisible(false);

                _lstUrsButton.SetVisible(false);

                _content.SetChild(GreetingFrame.New());
            }
        };

        // _crtArtButton.SetVisible(false);
        _crtLteButton.SetVisible(false);
        _crtRmteButton.SetVisible(false);
        _lstUrsButton.SetVisible(false);
        _crtUsrButton.SetVisible(false);

        _loginButton.OnClicked += LoginDialog.LogIn;
        _logoutButton.OnClicked += LoginDialog.LogOut;

        _lstArtButton.OnClicked += async (sender, e) => await Frame.ShowArticulosAsync(this, _content);
        _lstLteButton.OnClicked += async (sender, e) => await Frame.ShowLotesAsync(this, _content);
        _lstRmteButton.OnClicked += async (sender, e) => await Frame.ShowRemateAsync(this, _content);
        _lstUrsButton.OnClicked += async (sender, e) => await Frame.ShowUsersAsync(this, _content);

        _crtAniButton.OnClicked += (sender, e) => Frame.CreateAnimalAsync(this, _content);
        _crtMaqButton.OnClicked += (sender, e) => Frame.CreateMaquinariaAsync(this, _content);
        _crtOtrButton.OnClicked += (sender, e) => Frame.CreateOtroAsync(this, _content);
        _crtLteButton.OnClicked += (sender, e) => Frame.CreateLotes(this, _content);
        _crtRmteButton.OnClicked += (sender, e) => Frame.CreateRemate(this, _content);
        _crtUsrButton.OnClicked += (sender, e) => Frame.CreateUser(this, _content);
    }

    public void Startup() { application.AddWindow(this); Present(); }

    private bool OnCloseRequested(Gtk.Window sender, EventArgs e) => false;
}