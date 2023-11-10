using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;
using RemaTe.Shared.Controllers;

namespace RemaTe.GNOME.Views.Widgets;
class EmpleadoWidget : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Button edit;
    [Gtk.Connect] private readonly Gtk.Label _name;
    [Gtk.Connect] private readonly Adw.ActionRow _email;
#pragma warning restore 649

    private EmpleadoWidget(Gtk.Builder builder, EmpleadoVO empleado, Gtk.Window? parent, bool canEdit) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) edit.SetVisible(false);
        else edit.OnClicked += (_, _) => EditUser(parent: parent, empleado);

        _name.SetLabel(empleado.ci.ToString());
        _email.SetSubtitle(empleado.email);
    }

    public EmpleadoWidget(EmpleadoVO empleado, Gtk.Window? parent, bool canEdit)
     : this(Builder.FromFile("empleado_widget.ui"), empleado, parent, canEdit) { }

    public static void EditUser(Gtk.Window? parent, EmpleadoVO empleado) {
        UserDialog editWindow = new((empleado, null), parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}