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
        var cantidadActionRow = builder.GetObject("_cantidad") as Adw.ActionRow;
        var textView = builder.GetObject("_textView") as Gtk.TextView;
        var carousel = builder.GetObject("_carousel") as Adw.Carousel;

        var _propertiesFlowBox = builder.GetObject("_properties") as Gtk.FlowBox;

        if (!canEdit) edit.SetVisible(false);
        else edit.OnClicked += (_, _) => EditArticulo(parent: parent, articulo);

        titulo.SetLabel(articulo.nombre);
        cantidadActionRow.SetSubtitle(articulo.cantidad.ToString());
        textView.Buffer.Text = articulo.descripcion;

        if (articulo is AnimalVO) {
            var especieActionRow = Adw.ActionRow.New();
            especieActionRow.SetTitle("Tipo");
            especieActionRow.SetSubtitle((articulo as AnimalVO).tipo);
            especieActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(especieActionRow);

            var razaActionRow = Adw.ActionRow.New();
            razaActionRow.SetTitle("Raza");
            razaActionRow.SetSubtitle((articulo as AnimalVO).raza);
            razaActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(razaActionRow);

            var nacimientoActionRow = Adw.ActionRow.New();
            nacimientoActionRow.SetTitle("Año");
            nacimientoActionRow.SetSubtitle((articulo as AnimalVO).nacimiento.ToShortDateString());
            nacimientoActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(nacimientoActionRow);
        }
        else if (articulo is MaquinariaVO) {
            var marcaActionRow = Adw.ActionRow.New();
            marcaActionRow.SetTitle("Marca");
            marcaActionRow.SetSubtitle((articulo as MaquinariaVO).marca);
            marcaActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(marcaActionRow);

            var modeloActionRow = Adw.ActionRow.New();
            modeloActionRow.SetTitle("Modelo");
            modeloActionRow.SetSubtitle((articulo as MaquinariaVO).modelo);
            modeloActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(modeloActionRow);

            var añoActionRow = Adw.ActionRow.New();
            añoActionRow.SetTitle("Año");
            añoActionRow.SetSubtitle((articulo as MaquinariaVO).año.ToString());
            añoActionRow.SetCssClasses(new[] { "property", "card" });
            _propertiesFlowBox.Append(añoActionRow);
        }

        var (err, imagenes) = await Articulo.ReadImages(articulo);
        imagenes ??= new List<string>();

        foreach (string image in imagenes) {
            if (image != null && File.Exists(image)) {
                using var texture = Gdk.Texture.NewFromFilename(image);
                using var imgWidget = Gtk.Image.NewFromPaintable(texture);
                imgWidget.SetPixelSize(512);
                carousel.Append(imgWidget);
            }
        }

        return builder.GetObject("_root") as Adw.Bin;
    }

    public static void EditArticulo(Gtk.Window? parent, ArticuloVO articulo) {
        Adw.Window editWindow = articulo switch {
            AnimalVO => AnimalDialog.New(articulo as AnimalVO, parent),
            MaquinariaVO => MaquinariaDialog.New(articulo as MaquinariaVO, parent),
            _ => OtroDialog.New(articulo as OtroVO, parent),
        };
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
}
