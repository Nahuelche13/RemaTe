using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class RemateDialog : Adw.Window {
    private readonly Gtk.ShortcutController _shortcutController;

#pragma warning disable 649
    [Gtk.Connect] private readonly Adw.EntryRow _nombre;
    [Gtk.Connect] private readonly Adw.EntryRow _rematador;

    [Gtk.Connect] private readonly Gtk.Button _calendarButton;
    [Gtk.Connect] private readonly Gtk.Calendar _calendar;
    [Gtk.Connect] private readonly Gtk.SpinButton _horaEntry;
    [Gtk.Connect] private readonly Gtk.SpinButton _minutoEntry;
    [Gtk.Connect] private readonly Gtk.SpinButton _horasEntry;
    [Gtk.Connect] private readonly Gtk.SpinButton _minutosEntry;

    [Gtk.Connect] private readonly Gtk.Switch _switch;

    [Gtk.Connect] private readonly Gtk.Button _saveButton;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;

    [Gtk.Connect] private readonly Gtk.FlowBox _lotes;
    [Gtk.Connect] private readonly Gtk.Button _addLoteButton;
#pragma warning restore 649

    private readonly List<LoteVO> lotesSelected = new();
    private RemateDialog(RemateVO? remate, Gtk.Builder builder, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (remate != null) {
            var inicioDate = remate.inicio;
            _calendar.Day = inicioDate.Day;
            _calendar.Month = inicioDate.Month;
            _calendar.Year = inicioDate.Year;

            _calendarButton.SetLabel($"{inicioDate.Day}/{inicioDate.Month}/{inicioDate.Year}");

            _horaEntry.SetValue(inicioDate.Hour);
            _minutoEntry.SetValue(inicioDate.Minute);

            _horasEntry.SetValue(float.Floor(remate.duracion / 60));
            _minutosEntry.SetValue(remate.duracion - (remate.duracion / 60));

            _deleteButton.SetVisible(true);
        }
        else {
            _deleteButton.SetVisible(false);
        }

        //Dialog Settings
        SetTransientFor(parent);

        _calendar.OnDaySelected += (calendar, e) => _calendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");

        _addLoteButton.OnClicked += async (sender, e) => {
            TaskCompletionSource<List<LoteVO>?> tcs = new();
            var dialog = new ChooseLoteDialog(this, tcs);
            dialog.Present();

            List<LoteVO>? lotes = await tcs.Task;
            if (lotes != null) {
                foreach (LoteVO lote in lotes) {
                    lotesSelected.Add(lote);

                    var child = Gtk.FlowBoxChild.New();
                    var button = Gtk.Button.NewWithLabel(lote.nombre);
                    button.AddCssClass("card");
                    button.AddCssClass("title-1");
                    button.OnClicked += (sender, e) => {
                        lotesSelected.Remove(lote);
                        _lotes.Remove(child);
                    };
                    child.SetChild(button);
                    _lotes.Append(child);
                }
            }
        };

        _saveButton.OnClicked += async (sender, e) => {
            var inicio = new DateTime(_calendar.Year, _calendar.Month, _calendar.Day, (int)_horaEntry.GetValue(), (int)_minutoEntry.GetValue(), 0);

            RemateVO inputRmt = new() {
                nombre = _nombre.GetText(),
                rematador = _rematador.GetText(),
                inicio = inicio,
                duracion = (int)((_horasEntry.GetValue() * 60) + _minutosEntry.GetValue()),
                tipo = 0,
                metodos_pago = _switch.Active ? 1 : 0
            };

            if (remate != null) {
                inputRmt.id = remate.id;
                Errors err = await GenericL<RemateVO>.Update(inputRmt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                inputRmt.id = null;

                var (err, id) = await Remate.Create(inputRmt);
                if (err != Errors.Ok) {
                    Utils.ShowCommonErrorDialog(parent, err);
                    return;
                }
                err = await Remate.AddLotes(id, lotesSelected);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            Close();
        };
        _deleteButton.OnClicked += async (sender, e) => {
            Errors err = await GenericL<RemateVO>.Delete(remate);
            Utils.ShowCommonErrorDialog(parent, err);

            Close();
        };

        OnCloseRequest += (sender, e) => false;

        //Shortcut Controller
        _shortcutController = Gtk.ShortcutController.New();
        _shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        _shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(_shortcutController);
    }

    public RemateDialog(RemateVO? remate, Gtk.Window parent) : this(remate, Builder.FromFile("remate_dialog.ui"), parent) { }
}
