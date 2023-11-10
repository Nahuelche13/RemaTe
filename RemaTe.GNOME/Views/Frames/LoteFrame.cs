using System.IO;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;

namespace RemaTe.GNOME.Views.Widgets;
class LoteFrame : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Label _title;
    [Gtk.Connect] private readonly Adw.ActionRow _basePrice;
    [Gtk.Connect] private readonly Adw.ActionRow _comision;
    [Gtk.Connect] private readonly Gtk.Button _moreInfo;
    [Gtk.Connect] private readonly Gtk.Button _edit;
    [Gtk.Connect] private readonly Adw.Carousel _carousel;
#pragma warning restore 649

    public LoteFrame(LoteRep lote, Gtk.Window? parent, bool canEdit = false, GObject.SignalHandler<Gtk.Button> onClicked = null)
         : this(Builder.FromFile("lote_frame.ui"), lote, parent, canEdit, onClicked) { }
    private LoteFrame(Gtk.Builder builder, LoteRep lote, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) _edit.SetVisible(false);
        else _edit.OnClicked += (_, _) => EditLote(parent: parent, lote);

        _title.SetLabel(lote.nombre.ToString());
        _comision.SetSubtitle(lote.comision + "%");
        _basePrice.SetSubtitle("$" + lote.precio_base);

        if (onClicked != null) _moreInfo.OnClicked += onClicked;
        else _moreInfo.SetVisible(false);

        foreach (var articulo in lote.articulos) {
            var card = Adw.Bin.New();
            var column = Gtk.Box.New(Gtk.Orientation.Horizontal, 24);
            column.SetMarginTop(12);
            column.SetMarginBottom(12);
            column.SetMarginStart(12);
            column.SetMarginEnd(12);

            var carouselBox = Gtk.Box.New(Gtk.Orientation.Vertical, 12);
            var carousel = Adw.Carousel.New();
            var carouselIndicator = Adw.CarouselIndicatorDots.New();
            carouselIndicator.SetCarousel(carousel);
            carouselBox.Append(carousel);
            carouselBox.Append(carouselIndicator);
            column.Append(carouselBox);

            foreach (var image in articulo.imagenes) {
                if (image != null && File.Exists(image)) {
                    using var texture = Gdk.Texture.NewFromFilename(image);
                    using var imgWidget = Gtk.Image.NewFromPaintable(texture);
                    imgWidget.SetPixelSize(512);
                    carousel.Append(imgWidget);
                }
            }

            var column2 = Gtk.Box.New(Gtk.Orientation.Vertical, 24);
            column.Append(column2);

            var label = Gtk.Label.New(articulo.nombre);
            label.AddCssClass("title-2");
            column2.Append(label);

            var textview = Gtk.TextView.New();
            textview.SetMarginStart(12);
            textview.SetMarginEnd(12);
            textview.SetMarginTop(12);
            textview.SetMarginBottom(12);
            textview.SetEditable(false);
            textview.SetHexpand(true);
            textview.SetWrapMode(Gtk.WrapMode.Word);
            textview.Buffer.Text = articulo.descripcion;


            var scrolled_window = Gtk.ScrolledWindow.New();
            scrolled_window.SetVexpand(true);
            scrolled_window.SetHexpand(true);

            scrolled_window.SetChild(textview);
            var frame = Gtk.Frame.New(null);
            frame.SetMarginBottom(12);
            frame.SetChild(scrolled_window);

            column2.Append(frame);

            card.SetHexpand(true);
            card.SetVexpand(true);
            card.SetChild(column);
            card.AddCssClass("card");
            _carousel.Append(card);
        }
    }

    public static void EditLote(Gtk.Window? parent, LoteVO lote) {
        var editWindow = new LoteDialog(lote, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
