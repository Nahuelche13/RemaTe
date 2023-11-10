using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.GNOME.Views.Widgets;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class LoteDialog : Adw.Window {

#pragma warning disable 649
    [Gtk.Connect] private readonly Adw.EntryRow _idEntry;
    [Gtk.Connect] private readonly Adw.EntryRow _minEntry;
    [Gtk.Connect] private readonly Adw.EntryRow _comisionEntry;
    [Gtk.Connect] private readonly Gtk.Button _applyButton;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;

    [Gtk.Connect] private readonly Gtk.FlowBox _articulos;
    [Gtk.Connect] private readonly Gtk.Button _addArticuloButton;
#pragma warning restore 649

    private readonly List<ArticuloVO> articulosSelected = new();

    public LoteDialog(LoteVO? lote, Gtk.Window parent) : this(lote, Builder.FromFile("lote_dialog.ui"), parent) { }
    private LoteDialog(LoteVO? lote, Gtk.Builder builder, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (lote != null) {
            _idEntry.SetText(lote.id.ToString());
            _minEntry.SetText(lote.precio_base.ToString());
            _comisionEntry.SetText(lote.comision.ToString());

            _applyButton.SetSensitive(false);
            _deleteButton.SetVisible(true);
        }
        else {
            _applyButton.SetSensitive(false);
            _deleteButton.SetVisible(false);
        }

        //Dialog Settings
        SetTransientFor(parent);

        _idEntry.OnNotify += OnEntryNotify;
        _minEntry.OnNotify += OnEntryNotify;
        _comisionEntry.OnNotify += OnEntryNotify;

        _applyButton.OnClicked += async (sender, e) => {
            if (string.IsNullOrEmpty(_idEntry.GetText()) ||
                !Utils.IsNumber(_minEntry.GetText()) ||
                !Utils.IsNumber(_comisionEntry.GetText())) {
                return;
            }
            LoteVO inputLte = new() {
                id = null,
                nombre = _idEntry.GetText(),
                precio_base = int.Parse(_minEntry.GetText()),
                comision = int.Parse(_comisionEntry.GetText())
            };
            if (lote != null) {
                inputLte.id = lote.id;
                Errors err = await Lote.Update(inputLte);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                inputLte.id = null;

                var (err, id) = await Lote.Create(inputLte);
                if (err != Errors.Ok) {
                    Utils.ShowCommonErrorDialog(parent, err);
                    return;
                }
                err = await Lote.AddArticulos(id, articulosSelected);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            Close();
        };

        _deleteButton.OnClicked += async (sender, e) => {
            Errors err = await Lote.Delete(lote);
            Utils.ShowCommonErrorDialog(parent, err);

            Close();
        };
        OnCloseRequest += (sender, e) => false;

        _addArticuloButton.OnClicked += async (sender, e) => {
            TaskCompletionSource<List<ArticuloVO>?> tcs = new();
            var dialog = new ChooseArticuloDialog(this, tcs);
            dialog.Present();

            List<ArticuloVO>? articulos = await tcs.Task;
            if (articulos != null) {
                foreach (ArticuloVO articulo in articulos) {
                    articulosSelected.Add(articulo);

                    var child = Gtk.FlowBoxChild.New();
                    var button = Gtk.Button.NewWithLabel(articulo.nombre);
                    button.AddCssClass("card");
                    button.AddCssClass("title-1");
                    button.OnClicked += (sender, e) => {
                        articulosSelected.Remove(articulo);
                        _articulos.Remove(child);
                    };
                    child.SetChild(button);
                    _articulos.Append(child);
                }
            }
        };

        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(shortcutController);
    }

    void OnEntryNotify(GObject.Object sender, NotifySignalArgs e) {
        if (e.Pspec.GetName() == "text") {
            Validate();
        }
    }
    private void Validate() {
        if (string.IsNullOrEmpty(_idEntry.GetText()) ||
            !Utils.IsNumber(_minEntry.GetText()) ||
            !Utils.IsNumber(_comisionEntry.GetText())
        ) {
            _applyButton.SetSensitive(false);
        }
        else {
            _applyButton.SetSensitive(true);
        }
    }
}
