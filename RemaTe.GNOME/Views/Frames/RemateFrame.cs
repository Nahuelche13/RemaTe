using System.Threading.Tasks;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

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

            var (sold, price) = Lote.GetSellPrice((int)lote.id);

            card.SetChild(new LoteFrame(lote, parent, onClicked: sold == Common.Enums.Errors.Ok ? (_, _) => { } : ShowSellDialog(parent, lote), buttonLabel: sold == Common.Enums.Errors.Ok ? "Vendido: $" + price : "Vender"));
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

    protected static GObject.SignalHandler<Gtk.Button> ShowSellDialog(Gtk.Window window, LoteVO lote) => (sender, e) => {
        var dialog = Adw.MessageDialog.New(window, "Vernder lote", "");
        dialog.SetModal(false);
        var group = Gtk.Box.New(Gtk.Orientation.Vertical, 24);
        var precioRow = Adw.EntryRow.New();
        precioRow.SetTitle("Precio");
        precioRow.AddCssClass("card");
        group.Append(precioRow);
        var button = Gtk.Button.New();
        var buttonContent = Adw.ButtonContent.New();
        buttonContent.SetLabel("Adjudicado a");
        buttonContent.SetIconName("avatar-default-symbolic");
        button.SetChild(buttonContent);
        button.AddCssClass("action-row");
        var tcs = new TaskCompletionSource<ClienteVO>();
        button.OnClicked += (sender, e) => {
            var dialog = new ChooseUserDialog(window, tcs);
            dialog.Show();
        };
        group.Append(button);
        dialog.SetExtraChild(group);
        dialog.AddResponse("cancel", "Cancelar");
        dialog.AddResponse("suggested", "Confirmar");
        dialog.SetResponseAppearance("suggested", Adw.ResponseAppearance.Suggested);
        dialog.SetDefaultResponse("suggested");
        dialog.SetCloseResponse("cancel");
        dialog.OnResponse += async (s, ea) => {
            if (Utils.IsNumber(precioRow.GetText()) &&
            tcs.Task.Status == TaskStatus.RanToCompletion) {
                await Lote.Sell((int)lote.id, (await tcs.Task).ci, int.Parse(precioRow.GetText()));
            }
            dialog.Destroy();
        };
        dialog.Present();
    };
}
