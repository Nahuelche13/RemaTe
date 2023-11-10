using System;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;
using RemaTe.Shared.Controllers;

namespace RemaTe.GNOME.Views.Widgets;
class RemateWidget : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Button _edit;
    [Gtk.Connect] private readonly Gtk.Label _titulo;
    [Gtk.Connect] private readonly Adw.ActionRow _duracion;
#pragma warning restore 649

    private RemateWidget(Gtk.Builder builder, RemateVO remate, Gtk.Window? parent, bool canEdit) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) _edit.SetVisible(false);
        else _edit.OnClicked += (_, _) => EditRemateAsync(parent: parent, remate);

        _titulo.SetText(new DateTime(remate.inicio).ToShortDateString());
        _duracion.SetSubtitle($"{remate.duracion / 60} Horas {remate.duracion - (remate.duracion / 60)} Minutos");
    }

    public RemateWidget(RemateVO remate, Gtk.Window? parent, bool canEdit)
         : this(Builder.FromFile("remate_widget.ui"), remate, parent, canEdit) { }

    public static async void EditRemateAsync(Gtk.Window? parent, RemateVO remate) {
        var editWindow = new RemateDialog(remate, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
