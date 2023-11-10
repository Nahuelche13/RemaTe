using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;

using RemaTe.Common.Models;

using RemaTe.GNOME.Views;
using RemaTe.GNOME.Views.Widgets;
using RemaTe.Logic;

class Frame {
    public static void CreateAnimalAsync(Gtk.Window parent, Adw.Bin content) {
        // var editWindow = ArticuloDialog.New(null, parent);
        // editWindow.SetIconName("emoji-nature-symbolic");
        // editWindow.Present();
    }
    public static void CreateMaquinariaAsync(Gtk.Window parent, Adw.Bin content) {
        var editWindow = MaquinariaDialog.New(null, parent);
        editWindow.SetIconName("emoji-nature-symbolic");
        editWindow.Present();
    }
    public static void CreateOtroAsync(Gtk.Window parent, Adw.Bin content) {
        // var editWindow = OtroDialog.New(null, parent);
        // editWindow.SetIconName("emoji-nature-symbolic");
        // editWindow.Present();
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

        Gtk.FlowBox box = Gtk.FlowBox.New();
        box.SetSelectionMode(Gtk.SelectionMode.None);
        box.SetMaxChildrenPerLine(2);
        box.SetRowSpacing(24);
        box.SetColumnSpacing(24);
        box.SetMarginTop(24);
        box.SetMarginBottom(24);
        box.SetMarginStart(24);
        box.SetMarginEnd(24);

        Gtk.ScrolledWindow scroll = new();
        scroll.SetChild(box);
        content.SetChild(scroll);

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
    }
    public static async Task ShowLotesAsync(Gtk.Window parent, Adw.Bin content) {
        var (err, response) = Lote.ReadAllWithArticulo();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        content.SetChild(Gtk.Label.New("Cargando..."));

        Gtk.FlowBox box = CommonFlowBox();

        Gtk.ScrolledWindow scroll = new();
        scroll.SetChild(box);
        content.SetChild(scroll);

        await foreach (LoteRep lote in response) {
            box.Append(new LoteWidget(lote, parent, true, (sender, e) => ShowLote(lote, parent, content)));
        }
    }
    public static async Task ShowRemateAsync(Gtk.Window parent, Adw.Bin content) {
        var (err, response) = GenericL<RemateVO>.ReadAll();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        content.SetChild(Gtk.Label.New("Cargando..."));

        Gtk.FlowBox box = CommonFlowBox();

        Gtk.ScrolledWindow scroll = new();
        scroll.SetChild(box);
        content.SetChild(scroll);

        await foreach (RemateVO remate in response) {
            box.Append(new RemateWidget(remate, parent, true));
        }
    }
    public static async Task ShowUsersAsync(Gtk.Window parent, Adw.Bin content) {
        content.SetChild(Gtk.Label.New("Cargando..."));

        Errors err;
        (err, var clientes) = Cliente.ReadAllWithUsuario();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }
        (err, var empleados) = Empleado.ReadAllWithUsuario();
        if (err != Errors.Ok) {
            Utils.ShowCommonErrorDialog(parent, err);
            return;
        }

        Gtk.ScrolledWindow scroll = new() {
            PropagateNaturalHeight = true,
            HasFrame = true
        };
        content.SetChild(scroll);

        var box = new Gtk.Box {
            MarginTop = 25,
            MarginBottom = 25,
            MarginStart = 25,
            MarginEnd = 25
        };
        box.SetOrientation(Gtk.Orientation.Vertical);
        // box.SetSelectionMode(Gtk.SelectionMode.Single);
        // box.SetRowSpacing(25);
        // box.SetColumnSpacing(25);

        scroll.SetChild(box);

        var label1 = Gtk.Label.New("Administradores: ");
        label1.AddCssClass("title-1");
        var label2 = Gtk.Label.New("Empleados: ");
        label2.AddCssClass("title-1");
        var label3 = Gtk.Label.New("Vendedores: ");
        label3.AddCssClass("title-1");
        var label4 = Gtk.Label.New("Clientes: ");
        label4.AddCssClass("title-1");

        var administradoresBox = new Gtk.FlowBox {
            RowSpacing = 6,
            ColumnSpacing = 6,
            Homogeneous = true,
            MaxChildrenPerLine = 6,
            MinChildrenPerLine = 3,
            SelectionMode = Gtk.SelectionMode.None
        };
        administradoresBox.SetOrientation(Gtk.Orientation.Horizontal);
        var empleadosBox = new Gtk.FlowBox {
            RowSpacing = 6,
            ColumnSpacing = 6,
            Homogeneous = true,
            MaxChildrenPerLine = 6,
            MinChildrenPerLine = 3,
            SelectionMode = Gtk.SelectionMode.None
        };
        empleadosBox.SetOrientation(Gtk.Orientation.Horizontal);
        var vendedoresBox = new Gtk.FlowBox {
            RowSpacing = 6,
            ColumnSpacing = 6,
            Homogeneous = true,
            MaxChildrenPerLine = 6,
            MinChildrenPerLine = 3,
            SelectionMode = Gtk.SelectionMode.None
        };
        vendedoresBox.SetOrientation(Gtk.Orientation.Horizontal);
        var clientesBox = new Gtk.FlowBox {
            RowSpacing = 6,
            ColumnSpacing = 6,
            Homogeneous = true,
            MaxChildrenPerLine = 6,
            MinChildrenPerLine = 3,
            SelectionMode = Gtk.SelectionMode.None
        };
        clientesBox.SetOrientation(Gtk.Orientation.Horizontal);

        box.Append(label1);
        box.Append(administradoresBox);
        box.Append(label2);
        box.Append(empleadosBox);
        box.Append(label3);
        box.Append(vendedoresBox);
        box.Append(label4);
        box.Append(clientesBox);

        await foreach (ClienteVO cliente in clientes) {
            if (cliente.permisos == 1) {
                clientesBox.Append(new ClienteWidget(cliente, parent, true));
            }
            else {
                vendedoresBox.Append(new ClienteWidget(cliente, parent, true));
            }
        }
        await foreach (EmpleadoVO empleado in empleados) {
            if (empleado.permisos == 3) {
                empleadosBox.Append(new EmpleadoWidget(empleado, parent, true));
            }
            else {
                administradoresBox.Append(new EmpleadoWidget(empleado, parent, true));
            }
        }
    }

    public static void ShowLote(LoteRep lote, Gtk.Window parent, Adw.Bin content) {
        content.SetChild(new LoteFrame(lote, parent, true));
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