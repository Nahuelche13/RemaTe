using System;
using System.Threading.Tasks;

using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class LoginDialog : Adw.Window {
    private readonly Gtk.ShortcutController shortcutController;

#pragma warning disable 649
    [Gtk.Connect] private readonly Adw.EntryRow _emailEntry;
    [Gtk.Connect] private readonly Adw.PasswordEntryRow _passwordEntry;
    [Gtk.Connect] private readonly Gtk.Button _loginButton;
#pragma warning restore 649

    private LoginDialog(Gtk.Builder builder, Gtk.Window parent, TaskCompletionSource<Tuple<int, string>?> tcs) : base(builder.GetPointer("_root"), false) {
        var setPassword = false;
        builder.Connect(this);
        //Dialog Settings
        SetTransientFor(parent);
        _emailEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                ValidateInput();
            }
        };
        _passwordEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                ValidateInput();
            }
        };
        _loginButton.SetSensitive(false);
        _loginButton.OnClicked += (sender, e) => {
            setPassword = true;
            Close();
        };
        OnCloseRequest += (sender, e) => {
            tcs.SetResult(setPassword ? new(int.Parse(_emailEntry.GetText()), _passwordEntry.GetText()) : null);
            return false;
        };
        //Shortcut Controller
        shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(shortcutController);
    }

    private LoginDialog(Gtk.Window parent, TaskCompletionSource<Tuple<int, string>?> tcs) : this(Builder.FromFile("password_dialog.ui"), parent, tcs) { }

    private void ValidateInput() {
        int _;
        bool validCI = int.TryParse(_emailEntry.GetText(), out _);
        bool validPassword = !string.IsNullOrEmpty(_passwordEntry.GetText());

        if (!validCI) {
            _emailEntry.AddCssClass("error");
        }
        else {
            _emailEntry.RemoveCssClass("error");
        }
        if (!validCI || !validPassword) {
            _loginButton.SetSensitive(false);
        }
        else {
            _loginButton.SetSensitive(true);
        }
    }

    public static async void LogIn(Gtk.Window parent) {
        Tuple<int, string> rsp = await AccountLoginAsync(parent);
        bool success = false;
        if (rsp != null) success = await Usuario.LogIn(rsp.Item1, rsp.Item2);
        if (rsp != null && success == false) {
            Adw.MessageDialog dialog = Adw.MessageDialog.New(parent, "Error", "Usuario o pwd erroneo");
            dialog.AddResponse("ok", "Ok");
            dialog.SetResponseAppearance("ok", Adw.ResponseAppearance.Destructive);
            dialog.SetCloseResponse("ok");
            dialog.Present();
        }
    }

    public static void LogOut() {
        Usuario.LogOut();
    }

    public static async Task<Tuple<int, string>?> AccountLoginAsync(Gtk.Window parent) {
        var tcs = new TaskCompletionSource<Tuple<int, string>?>();
        LoginDialog passwordDialog = new(parent, tcs);
        passwordDialog.SetIconName("emoji-nature-symbolic");
        passwordDialog.Present();
        return await tcs.Task;
    }
}
