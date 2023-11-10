using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class RemateDialog : Adw.Window {
    private readonly Gtk.ShortcutController _shortcutController;

#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.Calendar _calendar;
    [Gtk.Connect] private readonly Gtk.SpinButton _hours;
    [Gtk.Connect] private readonly Gtk.SpinButton _minutes;

    [Gtk.Connect] private readonly Gtk.CheckButton _checkbox_1;
    [Gtk.Connect] private readonly Gtk.CheckButton _checkbox_2;
    [Gtk.Connect] private readonly Gtk.CheckButton _checkbox_3;

    [Gtk.Connect] private readonly Gtk.Button _saveButton;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;
#pragma warning restore 649

    private RemateDialog(RemateVO? remate, Gtk.Builder builder, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        if (remate != null) {
            _calendar.SelectDay(new GLib.DateTime(GLib.Internal.DateTime.NewFromUnixLocal(remate.inicio)));
            _hours.SetText((remate.duracion / 60).ToString());
            _minutes.SetText((remate.duracion - (remate.duracion / 60)).ToString());

            _deleteButton.SetVisible(true);
        }
        else {
            _deleteButton.SetVisible(false);
        }

        //Dialog Settings
        SetTransientFor(parent);

        _saveButton.OnClicked += async (sender, e) => {
            if (remate != null) {
                RemateVO inputRmt = new(
                    id: remate.id,
                    inicio: _calendar.Year * 1000 + _calendar.Month * 100 + _calendar.Day,
                    duracion: (int.Parse(_hours.GetText()) * 60) + int.Parse(_minutes.GetText()),
                    tipo: _checkbox_1.Active,
                    metodos_pago: _checkbox_2.Active
                );

                Errors err = await GenericL<RemateVO>.Update(inputRmt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                RemateVO inputRmt = new(
                    id: null,
                    inicio: _calendar.Year * 1000 + _calendar.Month * 100 + _calendar.Day,
                    duracion: (int.Parse(_hours.GetText()) * 60) + int.Parse(_minutes.GetText()),
                    tipo: _checkbox_1.Active,
                    metodos_pago: _checkbox_2.Active
                );

                Errors err = await GenericL<RemateVO>.Create(inputRmt);
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
