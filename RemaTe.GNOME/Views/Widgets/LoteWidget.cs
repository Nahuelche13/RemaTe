using System.IO;
using System.Linq;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;

namespace RemaTe.GNOME.Views.Widgets;
class LoteWidget : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Label _title;
    [Gtk.Connect] private readonly Adw.ActionRow _basePrice;
    [Gtk.Connect] private readonly Adw.ActionRow _comision;
    [Gtk.Connect] private readonly Gtk.Button _moreInfo;
    [Gtk.Connect] private readonly Gtk.Button _edit;
    [Gtk.Connect] private readonly Adw.Carousel _carousel;
#pragma warning restore 649

    public LoteWidget(LoteRep lote, Gtk.Window? parent, bool canEdit = false, GObject.SignalHandler<Gtk.Button> onClicked = null)
         : this(Builder.FromFile("lote_widget.ui"), lote, parent, canEdit, onClicked) { }
    private LoteWidget(Gtk.Builder builder, LoteRep lote, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) _edit.SetVisible(false);
        else _edit.OnClicked += (_, _) => EditLote(parent: parent, lote);

        _title.SetLabel(lote.nombre);
        _comision.SetSubtitle(lote.comision + "%");
        _basePrice.SetSubtitle("$" + lote.precio_base);

        if (onClicked != null) _moreInfo.OnClicked += onClicked;
        else _moreInfo.SetVisible(false);

        foreach (var articulo in lote.articulos) {
            var child = Adw.Bin.New();
            var column = Gtk.Box.New(Gtk.Orientation.Vertical, 24);
            column.SetMarginTop(12);
            column.SetMarginBottom(12);
            column.SetMarginStart(12);
            column.SetMarginEnd(12);

            if (articulo.imagenes.Count() > 0) {
                if (File.Exists(articulo.imagenes.ElementAt(0))) {
                    using var texture = Gdk.Texture.NewFromFilename(articulo.imagenes.ElementAt(0));
                    using var imgWidget = Gtk.Image.NewFromPaintable(texture);
                    imgWidget.SetPixelSize(256);
                    column.Append(imgWidget);
                }
            }

            var label = Gtk.Label.New(articulo.nombre);
            column.Append(label);

            child.SetChild(column);
            child.AddCssClass("card");
            _carousel.Append(child);
        }
    }

    public static void EditLote(Gtk.Window? parent, LoteVO lote) {
        var editWindow = new LoteDialog(lote, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
