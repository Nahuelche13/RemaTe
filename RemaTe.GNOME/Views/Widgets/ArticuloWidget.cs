using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views.Widgets;

static class ArticuloWidget {
    public static async Task<Adw.Bin> NewAsync(this ArticuloVO articulo, Gtk.Window? parent, bool canEdit) {
        var builder = Builder.FromFile("articulo_widget.ui");
        var edit = builder.GetObject("_edit") as Gtk.Button;
        var titulo = builder.GetObject("_titulo") as Gtk.Label;
        var cantidadActionRow = builder.GetObject("_basePrice") as Adw.ActionRow;
        var textView = builder.GetObject("_textView") as Gtk.TextView;
        var carousel = builder.GetObject("_carousel") as Adw.Carousel;

        if (!canEdit) edit.SetVisible(false);
        else edit.OnClicked += (_, _) => EditArticulo(parent: parent, articulo);

        titulo.SetLabel(articulo.nombre);
        cantidadActionRow.SetSubtitle(articulo.cantidad.ToString());
        textView.Buffer.Text = articulo.descripcion;

        var (err, imagenes) = await Articulo.ReadImages(articulo);
        imagenes ??= new List<string>();

        foreach (string image in imagenes) {
            if (image != null && File.Exists(image)) {
                using var texture = Gdk.Texture.NewFromFilename(image);
                using var imgWidget = Gtk.Image.NewFromPaintable(texture);
                imgWidget.SetPixelSize(256);
                carousel.Append(imgWidget);
            }
        }

        return builder.GetObject("_root") as Adw.Bin;
    }

    public static void EditArticulo(Gtk.Window? parent, ArticuloVO articulo) {
        Adw.Window editWindow;
        if (articulo is MaquinariaVO) {
            editWindow = MaquinariaDialog.New(articulo as MaquinariaVO, parent);
            editWindow.SetIconName("emoji-nature-symbolic");
            editWindow.Present();
        }
        //TODO ADD MORE IF
    }
}
