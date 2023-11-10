using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class UserDialog : Adw.Window {
#pragma warning disable 649, IDE1006
    [Gtk.Connect] private readonly Adw.EntryRow ciEntry;
    [Gtk.Connect] private readonly Adw.PasswordEntryRow pwdEntry;
    [Gtk.Connect] private readonly Adw.EntryRow nameEntry;
    [Gtk.Connect] private readonly Adw.EntryRow apellidoEntry;
    [Gtk.Connect] private readonly Adw.EntryRow emailClienteEntry;
    [Gtk.Connect] private readonly Adw.EntryRow telEntry;
    [Gtk.Connect] private readonly Gtk.DropDown dptoCombo;
    [Gtk.Connect] private readonly Adw.EntryRow locEntry;
    [Gtk.Connect] private readonly Adw.EntryRow calleEntry;
    [Gtk.Connect] private readonly Adw.EntryRow puertaEntry;

    [Gtk.Connect] private readonly Gtk.DropDown typeCombo;

    [Gtk.Connect] private readonly Gtk.Button _saveButton;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;
#pragma warning restore 649, IDE1006

    public UserDialog((EmpleadoVO? empleado, ClienteVO? cliente)? values, Gtk.Window parent) : this(values, Builder.FromFile("user_dialog.ui"), parent) { }
    private UserDialog((EmpleadoVO? empleado, ClienteVO? cliente)? values, Gtk.Builder builder, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);
        //Dialog Settings
        SetTransientFor(parent);
        OnCloseRequest += (sender, e) => false;
        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(shortcutController);

        if (values != null) {
            (EmpleadoVO? empleado, ClienteVO? cliente) = values.Value;
            UsuarioVO usuario = empleado == null ? cliente : empleado;

            ciEntry.SetEditable(false);
            ciEntry.Sensitive = false;
            typeCombo.Sensitive = false;

            ciEntry.SetText(usuario.id.ToString());
            nameEntry.SetText(usuario.nombre);
            apellidoEntry.SetText(usuario.nombre);
            emailClienteEntry.SetText(usuario.email);
            telEntry.SetText(usuario.telefono.ToString());
            dptoCombo.SetSelected((uint)usuario.departamento);
            locEntry.SetText(usuario.localidad.ToString());
            calleEntry.SetText(usuario.calle);
            puertaEntry.SetText(usuario.puerta.ToString());

            typeCombo.Selected = (uint)usuario.permisos;

            _deleteButton.SetVisible(true);
        }
        else {
            _deleteButton.SetVisible(false);
        }

        _saveButton.OnClicked += async (sender, e) => {
            if (!Utils.IsNumber(ciEntry.GetText())
            || !Utils.IsNumber(telEntry.GetText())
            || !Utils.IsNumber(puertaEntry.GetText())) {
                return;
            }

            UsuarioVO usuario = new() {
                id = int.Parse(ciEntry.GetText()),
                hash_pwd = values == null ? Hasher.Hash(pwdEntry.GetText()) : null,
                nombre = nameEntry.GetText(),
                email = emailClienteEntry.GetText(),
                telefono = int.Parse(telEntry.GetText()),
                departamento = (int)dptoCombo.GetSelected(),
                localidad = locEntry.GetText(),
                calle = calleEntry.GetText(),
                puerta = int.Parse(puertaEntry.GetText()),
                permisos = (int)typeCombo.GetSelected()
            };

            if (values != null) {
                Errors err = await Usuario.Update(usuario);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                Errors err = await Usuario.Create(usuario);
                if (typeCombo.GetSelected() < 3) {
                    _ = Cliente.Create(new ClienteVO() { ci = usuario.id });
                }
                else {
                    _ = Empleado.Create(new EmpleadoVO() { ci = usuario.id });
                }
                Utils.ShowCommonErrorDialog(parent, err);
            }
            Close();

        };
        _deleteButton.OnClicked += async (sender, e) => {
            Errors rspns = Errors.Ok;
            if (values.Value.empleado != null) {
                rspns = await Usuario.Delete(values.Value.empleado);
                await GenericL<EmpleadoVO>.Delete(values.Value.empleado);
            }
            if (values.Value.cliente != null) {
                rspns = await Usuario.Delete(values.Value.cliente);
                await GenericL<ClienteVO>.Delete(values.Value.cliente);
            }
            Utils.ShowCommonErrorDialog(parent, rspns);
            Close();
        };

        ciEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                if (ciEntry.GetText().Length != 8 || !Utils.IsNumber(ciEntry.GetText())) {
                    ciEntry.AddCssClass("error");
                }
                else { ciEntry.RemoveCssClass("error"); }
            }
        };

        emailClienteEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                if (!Utils.IsValidEmail(emailClienteEntry.GetText())) {
                    emailClienteEntry.AddCssClass("error");
                }
                else { emailClienteEntry.RemoveCssClass("error"); }
            }
        };
        puertaEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                if (!Utils.IsNumber(puertaEntry.GetText())) {
                    puertaEntry.AddCssClass("error");
                }
                else { puertaEntry.RemoveCssClass("error"); }
            }
        };
        telEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                if (!Utils.IsNumber(telEntry.GetText())) {
                    telEntry.AddCssClass("error");
                }
                else { telEntry.RemoveCssClass("error"); }
            }
        };
    }
}
