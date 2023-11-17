using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;

using RemaTe.Common.Models;

using RemaTe.GNOME.Views;
using RemaTe.GNOME.Views.Widgets;
using RemaTe.Logic;

class Frame {
    public static async void CreateAnimalAsync(Gtk.Window parent, Adw.Bin content) {
        var editWindow = await AnimalDialog.NewAsync(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static async void CreateMaquinariaAsync(Gtk.Window parent, Adw.Bin content) {
        var editWindow = await MaquinariaDialog.NewAsync(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static async void CreateOtroAsync(Gtk.Window parent, Adw.Bin content) {
        var editWindow = await OtroDialog.NewAsync(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static void CreateLotes(Gtk.Window parent, Adw.Bin content) {
        LoteDialog editWindow = new(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static void CreateRemate(Gtk.Window parent, Adw.Bin content) {
        RemateDialog editWindow = new(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static void CreateUser(Gtk.Window parent, Adw.Bin content) {
        UserDialog editWindow = new(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }

    public static async Task ShowArticulosAsync(Gtk.Window parent, Adw.Bin content) {
        content.SetChild(Gtk.Label.New("Cargando..."));

        Gtk.FlowBox box = CommonFlowBox();
        box.SetSelectionMode(Gtk.SelectionMode.None);
        box.SetMaxChildrenPerLine(2);

        (Errors _, IAsyncEnumerable<ArticuloVO> response) = Maquinaria.ReadAll();
        await foreach (ArticuloVO articulo in response) {
            box.Append(await ArticuloWidget.NewAsync(articulo, parent, true));
        }
        (_, response) = Animal.ReadAll();
        await foreach (ArticuloVO articulo in response) {
            box.Append(await ArticuloWidget.NewAsync(articulo, parent, true));
        }
        (_, response) = Otro.ReadAll();
        await foreach (ArticuloVO articulo in response) {
            box.Append(await ArticuloWidget.NewAsync(articulo, parent, true));
        }

        if (box.GetChildAtIndex(0) != null) {
            Gtk.ScrolledWindow scroll = new();
            scroll.SetChild(box);
            content.SetChild(scroll);
        }
        else {
            var statusPage = Adw.StatusPage.New();
            statusPage.SetTitle("Nada encontrado");
            statusPage.SetIconName("edit-find-symbolic");
            content.SetChild(statusPage);
        }
    }
    public static async Task ShowLotesAsync(Gtk.Window parent, Adw.Bin content) {
        var (err, response) = Lote.ReadAllWithArticulo();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        content.SetChild(Gtk.Label.New("Cargando..."));

        Gtk.FlowBox box = CommonFlowBox();

        await foreach (LoteRep lote in response) {
            box.Append(new LoteWidget(lote, parent, true, (sender, e) => ShowLote(lote, parent, content)));
        }

        if (box.GetChildAtIndex(0) != null) {
            Gtk.ScrolledWindow scroll = new();
            scroll.SetChild(box);
            content.SetChild(scroll);
        }
        else {
            var statusPage = Adw.StatusPage.New();
            statusPage.SetTitle("Nada encontrado");
            statusPage.SetIconName("edit-find-symbolic");
            content.SetChild(statusPage);
        }
    }
    public static async Task ShowFutureRemateAsync(Gtk.Window parent, Adw.Bin content) {
        var (err, response) = Remate.ReadAllFutureWithLote();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        var list = new Gtk.Box {
            MarginBottom = 24,
            MarginEnd = 24,
            MarginStart = 24,
            MarginTop = 24,
            Spacing = 24
        };
        list.SetOrientation(Gtk.Orientation.Vertical);
        list.Valign = Gtk.Align.Center;

        var spinner = Gtk.Spinner.New();
        spinner.Start();
        list.Append(spinner);
        list.Append(Gtk.Label.New("Cargando..."));

        var bin = new Adw.Bin() {
            Halign = Gtk.Align.Center,
            Valign = Gtk.Align.Center,
        };
        bin.AddCssClass("card");
        bin.SetChild(list);

        content.SetChild(bin);

        Gtk.FlowBox box = CommonFlowBox();
        box.SetSelectionMode(Gtk.SelectionMode.None);
        box.SetMaxChildrenPerLine(2);

        await foreach (RemateRep remate in response) {
            box.Append(new RemateWidget(remate, parent, true, (sender, e) => ShowRemate(remate, parent, content)));
        }

        if (box.GetChildAtIndex(0) != null) {
            Gtk.ScrolledWindow scroll = new();
            scroll.SetChild(box);
            content.SetChild(scroll);
        }
        else {
            var statusPage = Adw.StatusPage.New();
            statusPage.SetTitle("Nada encontrado");
            statusPage.SetIconName("edit-find-symbolic");
            content.SetChild(statusPage);
        }
    }
    public static async Task ShowPastRemateAsync(Gtk.Window parent, Adw.Bin content) {
        var (err, response) = Remate.ReadAllPastWithLote();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        content.SetChild(Gtk.Label.New("Cargando..."));

        Gtk.FlowBox box = CommonFlowBox();
        box.SetSelectionMode(Gtk.SelectionMode.None);
        box.SetMaxChildrenPerLine(2);

        await foreach (RemateRep remate in response) {
            box.Append(new RemateWidget(remate, parent, true, (sender, e) => ShowRemate(remate, parent, content)));
        }

        if (box.GetChildAtIndex(0) != null) {
            Gtk.ScrolledWindow scroll = new();
            scroll.SetChild(box);
            content.SetChild(scroll);
        }
        else {
            var statusPage = Adw.StatusPage.New();
            statusPage.SetTitle("Nada encontrado");
            statusPage.SetIconName("edit-find-symbolic");
            content.SetChild(statusPage);
        }
    }
    public static void ShowUsers(Gtk.Window parent, Adw.Bin content) {
        content.SetChild(Gtk.Label.New("Cargando..."));

        var builder = RemaTe.GNOME.Helpers.Builder.FromFile("usuarios_frame.ui");
        content.SetChild(builder.GetObject("_root") as Gtk.Widget);

        var clientesBin = builder.GetObject("_clientesBin") as Adw.Bin;
        var vendedoresBin = builder.GetObject("_vendedoresBin") as Adw.Bin;
        var empleadosBin = builder.GetObject("_empleadosBin") as Adw.Bin;
        var administradoresBin = builder.GetObject("_administradoresBin") as Adw.Bin;

        var ciEntry = builder.GetObject("_ciEntry") as Adw.EntryRow;
        var showInactiveSitch = builder.GetObject("_showInactiveSitch") as Gtk.Switch;
        var applyButton = builder.GetObject("_applyButton") as Gtk.Button;

        applyButton.OnClicked += (button, e) => Render();

        Render();

        async void Render() {
            var filer = new UsuarioVO() { is_active = !showInactiveSitch.GetState() };
            if (Utils.IsNumber(ciEntry.GetText())) {
                filer.id = int.Parse(ciEntry.GetText());
            }

            Errors err;
            (err, var clientes) = Cliente.ReadAllWithUsuarioWhere(filer);
            if (err != Errors.Ok) {
                Utils.ShowCommonErrorDialog(parent, err);
                return;
            }
            (err, var empleados) = Empleado.ReadAllWithUsuarioWhere(filer);
            if (err != Errors.Ok) {
                Utils.ShowCommonErrorDialog(parent, err);
                return;
            }

            var clientesBox = new Gtk.FlowBox {
                RowSpacing = 6,
                ColumnSpacing = 6,
                Homogeneous = true,
                MaxChildrenPerLine = 6,
                MinChildrenPerLine = 3,
                SelectionMode = Gtk.SelectionMode.None
            };
            var empleadosBox = new Gtk.FlowBox {
                RowSpacing = 6,
                ColumnSpacing = 6,
                Homogeneous = true,
                MaxChildrenPerLine = 6,
                MinChildrenPerLine = 3,
                SelectionMode = Gtk.SelectionMode.None
            };
            var vendedoresBox = new Gtk.FlowBox {
                RowSpacing = 6,
                ColumnSpacing = 6,
                Homogeneous = true,
                MaxChildrenPerLine = 6,
                MinChildrenPerLine = 3,
                SelectionMode = Gtk.SelectionMode.None
            };
            var administradoresBox = new Gtk.FlowBox {
                RowSpacing = 6,
                ColumnSpacing = 6,
                Homogeneous = true,
                MaxChildrenPerLine = 6,
                MinChildrenPerLine = 3,
                SelectionMode = Gtk.SelectionMode.None
            };

            clientesBin.SetChild(clientesBox);
            vendedoresBin.SetChild(vendedoresBox);
            empleadosBin.SetChild(empleadosBox);
            administradoresBin.SetChild(administradoresBox);

            await foreach (ClienteVO cliente in clientes) {
                if (cliente.permisos == 0) {
                    clientesBox.Append(new ClienteWidget(cliente, parent, true));
                }
                else {
                    vendedoresBox.Append(new ClienteWidget(cliente, parent, true));
                }
            }
            await foreach (EmpleadoVO empleado in empleados) {
                if (empleado.permisos == 2) {
                    empleadosBox.Append(new EmpleadoWidget(empleado, parent, true));
                }
                else {
                    administradoresBox.Append(new EmpleadoWidget(empleado, parent, true));
                }
            }
        }
    }

    public static void ShowLote(LoteRep lote, Gtk.Window parent, Adw.Bin content) {
        content.SetChild(new LoteFrame(lote, parent, true));
    }
    public static void ShowRemate(RemateRep remate, Gtk.Window parent, Adw.Bin content) {
        content.SetChild(new RemateFrame(remate, parent, true));
    }


    private static Gtk.FlowBox CommonFlowBox() {
        Gtk.FlowBox box = Gtk.FlowBox.New();
        box.SetSelectionMode(Gtk.SelectionMode.None);
        box.SetMaxChildrenPerLine(3);
        box.SetRowSpacing(24);
        box.SetColumnSpacing(24);
        box.SetMarginTop(24);
        box.SetMarginBottom(24);
        box.SetMarginStart(24);
        box.SetMarginEnd(24);
        return box;
    }
}
