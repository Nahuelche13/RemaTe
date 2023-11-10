using System;
using System.Collections.Generic;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;

namespace RemaTe.GNOME.Views.Widgets;
class RemateWidget : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Button _edit;
    [Gtk.Connect] private readonly Gtk.Label _titulo;
    [Gtk.Connect] private readonly Adw.ActionRow _inicio;
    [Gtk.Connect] private readonly Adw.ActionRow _duracion;
    [Gtk.Connect] private readonly Gtk.Button _moreInfo;
    [Gtk.Connect] private readonly Adw.ActionRow _rematador;
    [Gtk.Connect] private readonly Adw.Carousel _carousel;
#pragma warning restore 649

    public RemateWidget(RemateRep remate, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked = null)
         : this(Builder.FromFile("remate_widget.ui"), remate, parent, canEdit, onClicked) { }
    private RemateWidget(Gtk.Builder builder, RemateRep remate, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked = null) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) _edit.SetVisible(false);
        else _edit.OnClicked += (_, _) => EditRemate(parent: parent, remate);

        if (onClicked != null) _moreInfo.OnClicked += onClicked;
        else _moreInfo.SetVisible(false);

        _titulo.SetLabel(remate.nombre);

        var inicio = remate.inicio;
        _inicio.SetSubtitle($"{inicio.ToShortTimeString()} {inicio.ToShortDateString()}");
        _duracion.SetSubtitle($"{remate.duracion / 60} Horas {remate.duracion - (remate.duracion / 60 * 60)} Minutos");
        _rematador.SetSubtitle(remate.rematador);

        foreach (LoteRep lote in remate.lotes) {
            lote.articulos = new List<ArticuloRep>();
            _carousel.Append(new LoteWidget(lote, parent));
        }
    }

    public static void EditRemate(Gtk.Window? parent, RemateVO remate) {
        var editWindow = new RemateDialog(remate, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
