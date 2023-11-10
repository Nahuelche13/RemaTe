using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;

namespace RemaTe.GNOME.Views.Widgets;
class RemateFrame : Adw.Bin {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Label _title;
    [Gtk.Connect] private readonly Adw.ActionRow _inicio;
    [Gtk.Connect] private readonly Adw.ActionRow _duracion;
    [Gtk.Connect] private readonly Adw.ActionRow _rematador;
    [Gtk.Connect] private readonly Gtk.Button _edit;
    [Gtk.Connect] private readonly Gtk.FlowBox _flowBox;
#pragma warning restore 649

    public RemateFrame(RemateRep remate, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked = null)
         : this(Builder.FromFile("remate_frame.ui"), remate, parent, canEdit, onClicked) { }
    private RemateFrame(Gtk.Builder builder, RemateRep remate, Gtk.Window? parent, bool canEdit, GObject.SignalHandler<Gtk.Button> onClicked) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (!canEdit) _edit.SetVisible(false);
        else _edit.OnClicked += (_, _) => EditRemate(parent: parent, remate);

        _title.SetLabel(remate.nombre);

        var inicio = remate.inicio;
        _inicio.SetSubtitle($"{inicio.ToShortTimeString()} {inicio.ToShortDateString()}");
        _duracion.SetSubtitle($"{remate.duracion / 60} Horas {remate.duracion - (remate.duracion / 60)} Minutos");
        _rematador.SetSubtitle(remate.rematador);

        foreach (var lote in remate.lotes) {
            var card = Adw.Bin.New();
            card.SetChild(new LoteFrame(lote, parent, onClicked: ShowSellDialog(parent)));
            card.SetHexpand(true);
            card.SetVexpand(true);
            card.AddCssClass("card");
            _flowBox.Append(card);
        }
    }

    private static void EditRemate(Gtk.Window? parent, RemateVO remate) {
        var editWindow = new RemateDialog(remate, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }

    protected static GObject.SignalHandler<Gtk.Button> ShowSellDialog(Gtk.Window window) {
        return (sender, e) => {
            var dialog = Adw.MessageDialog.New(window, "Vernder lote", "");
            var group = Adw.PreferencesGroup.New();
            var precioRow = Adw.EntryRow.New();
            precioRow.SetTitle("Precio");
            group.Add(precioRow);
            var entryRow = Adw.EntryRow.New();
            entryRow.SetTitle("Adjudicado a");
            group.Add(entryRow);
            dialog.SetExtraChild(group);
            dialog.AddResponse("cancel", "Cancelar");
            dialog.AddResponse("suggested", "Confirmar");
            dialog.SetResponseAppearance("suggested", Adw.ResponseAppearance.Suggested);
            dialog.SetDefaultResponse("suggested");
            dialog.SetCloseResponse("cancel");
            dialog.OnResponse += (s, ea) => {
                if (!string.IsNullOrEmpty(entryRow.GetText()) && entryRow.GetText() != "NULL") {
                    // TODO SELL LOTE
                }
                dialog.Destroy();
            };
            dialog.Present();
        };
    }
}
