using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Shared.Models;

namespace RemaTe.GNOME.Views.Widgets;
class EmpleadoWidget : Adw.Bin {
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

    private EmpleadoWidget(Gtk.Builder builder, EmpleadoVO empleado, Gtk.Window? parent, bool canEdit) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) edit.SetVisible(false);
        else edit.OnClicked += (_, _) => EditUser(parent: parent, empleado);

        _name.SetLabel(empleado.nombre);
        _ci.SetSubtitle(empleado.ci.ToString());
        _tel.SetSubtitle(empleado.telefono.ToString());
        _email.SetSubtitle(empleado.email.ToString());
        _dpto.SetSubtitle(AppInfo.Departamentos[(int)empleado.departamento]);
        _loc.SetSubtitle(empleado.localidad.ToString());
        _calle.SetSubtitle(empleado.calle.ToString());
        _puerta.SetSubtitle(empleado.puerta.ToString());
    }

    public EmpleadoWidget(EmpleadoVO empleado, Gtk.Window? parent, bool canEdit)
     : this(Builder.FromFile("cliente_widget.ui"), empleado, parent, canEdit) { }

    public static void EditUser(Gtk.Window? parent, EmpleadoVO empleado) {
        UserDialog editWindow = new((empleado, null), parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
