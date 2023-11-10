using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Shared.Models;

namespace RemaTe.GNOME.Views.Widgets;
class ClienteWidget : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Button edit;
    [Gtk.Connect] private readonly Gtk.Label _name;
    [Gtk.Connect] private readonly Adw.ActionRow _ci;
    [Gtk.Connect] private readonly Adw.ActionRow _tel;
    [Gtk.Connect] private readonly Adw.ActionRow _email;
    [Gtk.Connect] private readonly Adw.ActionRow _dpto;
    [Gtk.Connect] private readonly Adw.ActionRow _loc;
    [Gtk.Connect] private readonly Adw.ActionRow _calle;
    [Gtk.Connect] private readonly Adw.ActionRow _puerta;
#pragma warning restore 649

    private ClienteWidget(Gtk.Builder builder, ClienteVO cliente, Gtk.Window? parent, bool canEdit) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) edit.SetVisible(false);
        else edit.OnClicked += (_, _) => EditCliente(parent: parent, cliente);

        _name.SetLabel(cliente.nombre);
        _ci.SetSubtitle(cliente.ci.ToString());
        _tel.SetSubtitle(cliente.telefono.ToString());
        _email.SetSubtitle(cliente.email.ToString());
        _dpto.SetSubtitle(AppInfo.Departamentos[cliente.departamento]);
        _loc.SetSubtitle(cliente.localidad.ToString());
        _calle.SetSubtitle(cliente.calle.ToString());
        _puerta.SetSubtitle(cliente.puerta.ToString());
    }

    public ClienteWidget(ClienteVO cliente, Gtk.Window? parent, bool canEdit)
     : this(Builder.FromFile("cliente_widget.ui"), cliente, parent, canEdit) { }

    public static void EditCliente(Gtk.Window? parent, ClienteVO cliente) {
        UserDialog editWindow = new((null, cliente), parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
