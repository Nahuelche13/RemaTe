using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nickvision.GirExt;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class ChooseUserDialog : Adw.Window {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.FlowBox _flowbox;
    [Gtk.Connect] private readonly Gtk.Button _cancelButton;
    [Gtk.Connect] private readonly Gtk.Button _saveButton;
#pragma warning restore 649

    readonly List<ClienteVO> clientes = new();

    public ChooseUserDialog(Gtk.Window parent, TaskCompletionSource<ClienteVO> tcs) : this(Builder.FromFile("choose_articulo_dialog.ui"), parent, tcs) { }
    private ChooseUserDialog(Gtk.Builder builder, Gtk.Window parent, TaskCompletionSource<ClienteVO> tcs) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        //Dialog Settings
        SetTransientFor(parent);

        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(shortcutController);

        _flowbox.SetSelectionMode(Gtk.SelectionMode.Single);

        FillFlowBox();
        _cancelButton.OnClicked += (sender, e) => Close();
        _saveButton.OnClicked += (sender, e) => {
            var indeces = _flowbox.GetSelectedChildrenIndices();
            var selected = indeces.Select(index => clientes[index]).First();
            tcs.SetResult(selected);
            Close();
        };
        OnCloseRequest += (sender, e) => false;
    }

    async void FillFlowBox() {
        var (_, clientesDB) = Cliente.ReadAllWithUsuario();

        await foreach (var cliente in clientesDB) {
            var child = Gtk.FlowBoxChild.New();
            var label = Gtk.Label.New(cliente.nombre);
            label.AddCssClass("card");
            label.AddCssClass("title-1");
            child.SetChild(label);
            _flowbox.Append(child);
            clientes.Add(cliente);
        }
    }
}
