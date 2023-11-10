using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Nickvision.GirExt;

using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.GNOME.Views.Widgets;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;

public class ChooseArticuloDialog : Adw.Window {
#pragma warning disable 649
    [Gtk.Connect] private readonly Gtk.FlowBox _flowbox;
    [Gtk.Connect] private readonly Gtk.Button _cancelButton;
    [Gtk.Connect] private readonly Gtk.Button _saveButton;
#pragma warning restore 649

    List<ArticuloVO> articulos = new();

    public ChooseArticuloDialog(Gtk.Window parent, TaskCompletionSource<List<ArticuloVO>?> tcs) : this(Builder.FromFile("choose_articulo_dialog.ui"), parent, tcs) { }
    private ChooseArticuloDialog(Gtk.Builder builder, Gtk.Window parent, TaskCompletionSource<List<ArticuloVO>?> tcs) : base(builder.GetPointer("_root"), false) {
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

        FillFlowBox(tcs);
        _cancelButton.OnClicked += (sender, e) => Close();
        _saveButton.OnClicked += (sender, e) => {
            var indeces = _flowbox.GetSelectedChildrenIndices();
            var selected = indeces.Select(index => articulos[index]).ToList();
            tcs.SetResult(selected);
            Close();
        };
        OnCloseRequest += (sender, e) => false;
    }

    async void FillFlowBox(TaskCompletionSource<List<ArticuloVO>> tcs) {
        var (_, animales) = Animal.ReadAll();
        var (_, maquinarias) = Maquinaria.ReadAll();
        var (_, otros) = Otro.ReadAll();

        await foreach (var item in animales) {
            var child = Gtk.FlowBoxChild.New();
            var button = Gtk.Label.New(item.nombre);
            button.AddCssClass("card");
            button.AddCssClass("title-1");
            child.SetChild(button);
            _flowbox.Append(child);
            articulos.Add(item);
        }
        await foreach (var item in maquinarias) {
            var child = Gtk.FlowBoxChild.New();
            var button = Gtk.Label.New(item.nombre);
            button.AddCssClass("card");
            button.AddCssClass("title-1");
            child.SetChild(button);
            _flowbox.Append(child);
            articulos.Add(item);
        }
        await foreach (var item in otros) {
            var child = Gtk.FlowBoxChild.New();
            var button = Gtk.Label.New(item.nombre);
            button.AddCssClass("card");
            button.AddCssClass("title-1");
            child.SetChild(button);
            _flowbox.Append(child);
            articulos.Add(item);
        }
    }
}
