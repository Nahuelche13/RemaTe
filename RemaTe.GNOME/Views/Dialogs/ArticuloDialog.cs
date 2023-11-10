using System;
using System.Collections.Generic;

using Nickvision.GirExt;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.GNOME.Helpers;
using RemaTe.Logic;

namespace RemaTe.GNOME.Views;
/*
public class ArticuloDialog : Adw.Window {
    private readonly Gtk.ShortcutController _shortcutController;

#pragma warning disable 649
    // Name Entry
    [Gtk.Connect] private readonly Adw.EntryRow _nameEntry;
    [Gtk.Connect] private readonly Adw.EntryRow _cantidadEntry;
    // Type ComboRow
    [Gtk.Connect] private readonly Adw.ComboRow _artTypeRow;

    // PreferencesGroup
    [Gtk.Connect] private readonly Adw.PreferencesGroup _animalesGroup;
    [Gtk.Connect] private readonly Adw.PreferencesGroup _maquinariaGroup;

    [Gtk.Connect] private readonly Adw.ComboRow _especieTypeRow;
    [Gtk.Connect] private readonly Adw.ComboRow _razaTypeRow;

    // Calendars
    [Gtk.Connect] private readonly Gtk.MenuButton _dateCalendarButton;
    [Gtk.Connect] private readonly Gtk.MenuButton _añoCalendarButton;
    [Gtk.Connect] private readonly Gtk.Calendar _dateCalendar;
    [Gtk.Connect] private readonly Gtk.Calendar _añoCalendar;

    [Gtk.Connect] private readonly Adw.ComboRow _marcaTypeRow;
    [Gtk.Connect] private readonly Adw.ComboRow _modeloTypeRow;

    [Gtk.Connect] private readonly Gtk.TextView _dscTextView;

    // Buttons
    [Gtk.Connect] private readonly Gtk.Button _saveButton;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;

    // Images
    [Gtk.Connect] private readonly Gtk.FlowBox _images;
    [Gtk.Connect] private readonly Gtk.Button _addImageButton;

    // Custom properties
    [Gtk.Connect] private readonly Gtk.Button _addNewPropertyButton;
    [Gtk.Connect] private readonly Adw.PreferencesGroup _customPropertiesGroup;
#pragma warning restore 649

    readonly List<string> images = new();
    readonly List<Adw.EntryRow> _customPropertyRows = new();

    public ArticuloDialog(ArticuloVO? lote, Gtk.Window parent) : this(lote, Builder.FromFile("articulo_dialog.ui"), parent) { }
    private ArticuloDialog(ArticuloVO? articulo, Gtk.Builder builder, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        builder.Connect(this);

        //Dialog Settings
        SetTransientFor(parent);
        OnCloseRequest += (sender, e) => false;

        //Shortcut Controller
        _shortcutController = Gtk.ShortcutController.New();
        _shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        _shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            Close();
            return true;
        })));
        AddController(_shortcutController);

        if (articulo != null) {
            _nameEntry.SetText(articulo.nombre);
            _dscTextView.Buffer.Text = articulo.descripcion;

            _deleteButton.SetVisible(true);
        }
        else {
            _saveButton.SetSensitive(false);
            _deleteButton.SetVisible(false);
        }

        Validate();

        _nameEntry.OnNotify += OnEntryNotify;
        // _dscTextView.OnNotify += OnEntryNotify;

        _artTypeRow.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "selected-item") {
                switch (_artTypeRow.GetSelected()) {
                    case 0:
                        _animalesGroup.SetVisible(true);
                        _maquinariaGroup.SetVisible(false);
                        break;
                    case 1:
                        _animalesGroup.SetVisible(false);
                        _maquinariaGroup.SetVisible(true);
                        break;
                    default:
                        _animalesGroup.SetVisible(false);
                        _maquinariaGroup.SetVisible(false);
                        break;
                }
            }
        };

        _especieTypeRow.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "selected-item") {
                switch (_especieTypeRow.GetSelected()) {
                    case 0:
                        _razaTypeRow.SetModel(Gtk.StringList.New(new string[] { "Hereford", "Marrón", "Dálmata", "Peluda", "Otro" }));
                        break;
                    case 1:
                        _razaTypeRow.SetModel(Gtk.StringList.New(new string[] { "Uno", "Dos", "Tres", "Cuatro", "Otro" }));
                        break;
                }
            }
        };

        _dateCalendar.OnDaySelected += (calendar, e) => _dateCalendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");
        _añoCalendar.OnDaySelected += (calendar, e) => _añoCalendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");

        _addNewPropertyButton.OnClicked += (sender, e) => {
            var _dialog = Adw.MessageDialog.New(this, "Nueva propiedad personalizada", "");
            var _group = Adw.PreferencesGroup.New();
            var _entryRow = Adw.EntryRow.New();
            _entryRow.SetTitle("Nombre de la propiedad");
            _group.Add(_entryRow);
            _dialog.SetExtraChild(_group);
            _dialog.AddResponse("cancel", "Cancelar");
            _dialog.AddResponse("suggested", "Añadir");
            _dialog.SetResponseAppearance("suggested", Adw.ResponseAppearance.Suggested);
            _dialog.SetDefaultResponse("suggested");
            _dialog.SetCloseResponse("cancel");
            _dialog.OnResponse += (s, ea) => {
                if (!string.IsNullOrEmpty(_entryRow.GetText()) && _entryRow.GetText() != "NULL") {
                    var row = Adw.EntryRow.New();
                    row.SetTitle(_entryRow.GetText());
                    var removeButton = Gtk.Button.New();
                    removeButton.SetValign(Gtk.Align.Center);
                    removeButton.SetTooltipText("Remove Custom Property");
                    removeButton.SetIconName("user-trash-symbolic");
                    removeButton.AddCssClass("flat");
                    removeButton.OnClicked += (sender, e) => {
                        _customPropertyRows.Remove(row);
                        _customPropertiesGroup.Remove(row);
                    };
                    row.AddSuffix(removeButton);
                    _customPropertyRows.Add(row);
                    _customPropertiesGroup.Add(row);
                }
                _dialog.Destroy();
            };
            _dialog.Present();
        };

        _addImageButton.OnClicked += async (sender, e) => {
            var openFileDialog = Gtk.FileDialog.New();
            openFileDialog.SetTitle("Seleccione una Imagen");
            var filters = Gio.ListStore.New(Gtk.FileFilter.GetGType());
            var filterImages = Gtk.FileFilter.New();
            filterImages.AddMimeType("image/jpeg");
            filterImages.AddMimeType("image/png");
            filterImages.AddMimeType("image/bmp");
            filterImages.AddMimeType("image/webp");
            filters.Append(filterImages);
            openFileDialog.SetFilters(filters);
            try {
                var file = await openFileDialog.OpenAsync(this);
                images.Add(file.GetPath());

                using var texture = Gdk.Texture.NewFromFilename(file.GetPath());
                using var image = Gtk.Image.NewFromPaintable(texture);
                image.SetPixelSize(196);

                var button = Gtk.Button.New();
                button.SetChild(image);

                var flowChild = Gtk.FlowBoxChild.New();
                flowChild.SetChild(button);

                _images.Append(flowChild);
                button.OnClicked += (sender, e) => {
                    images.Remove(file.GetPath());
                    _images.Remove(flowChild);
                };
            }
            catch { }
        };

        _saveButton.OnClicked += async (sender, e) => {
            if (articulo != null) {
                StringBuilder sb = new();
                sb.Append("x-type:");
                sb.AppendLine(_artTypeRow.GetSelected().ToString());

                switch (_artTypeRow.GetSelected()) {
                    case 0:
                        sb.Append("x-especie:");
                        sb.AppendLine(_especieTypeRow.GetSelected().ToString());
                        sb.Append("x-raza:");
                        sb.AppendLine(_razaTypeRow.GetSelected().ToString());
                        sb.Append("x-naciemiento:");
                        sb.AppendLine($"{_dateCalendar.Day}/{_dateCalendar.Month}/{_dateCalendar.Year}");
                        break;
                    case 1:
                        sb.Append("x-typeMaq:");
                        sb.AppendLine(_modeloTypeRow.GetSelected().ToString());
                        sb.Append("x-marca:");
                        sb.AppendLine(_marcaTypeRow.GetSelected().ToString());
                        sb.Append("x-año:");
                        sb.AppendLine($"{_añoCalendar.Day}/{_añoCalendar.Month}/{_añoCalendar.Year}");
                        break;
                    default: break;
                }

                foreach (var customPropertie in _customPropertyRows) {
                    sb.Append($"c-{customPropertie.GetTitle()}:");
                    sb.AppendLine(customPropertie.GetText());
                }

                sb.Append("x-freeform:");
                sb.AppendLine(_dscTextView.Buffer.Text);


                ArticuloVO inputArt = new(
                    id: articulo.id,
                    nombre: _nameEntry.GetText(),
                    descripcion: sb.ToString()
                );

                Errors err = await Articulo.Update(inputArt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                Errors err; int id;
                switch (_artTypeRow.GetSelected()) {
                    case 0:
                        AnimalVO animal = new() {
                            id = null,
                            nombre = _nameEntry.GetText(),
                            tipo = _especieTypeRow.GetSelected(),
                            raza = _razaTypeRow.GetSelected(),
                            nacimiento = (int)DateTime.Parse($"{_dateCalendar.Day}/{_dateCalendar.Month}/{_dateCalendar.Year}").Ticks,
                            descripcion = _dscTextView.Buffer.Text
                        };
                        (err, id) = await Animal.Create(animal);
                        if (err != Errors.Ok) {
                            Utils.ShowCommonErrorDialog(parent, err);
                        }
                        err = await Articulo.AddImages(id, images);
                        Utils.ShowCommonErrorDialog(parent, err);
                        break;
                    case 1:
                        MaquinariaVO maquinaria = new() {
                            id = null,
                            nombre = _nameEntry.GetText(),
                            marca = _marcaTypeRow.GetSelected(),
                            modelo = _modeloTypeRow.GetSelected(),
                            año = (int)DateTime.Parse($"{_añoCalendar.Day}/{_añoCalendar.Month}/{_añoCalendar.Year}").Ticks,
                            descripcion = _dscTextView.Buffer.Text
                        };
                        (err, id) = await Maquinaria.Create(maquinaria);
                        if (err != Errors.Ok) {
                            Utils.ShowCommonErrorDialog(parent, err);
                        }
                        err = await Articulo.AddImages(id, images);
                        Utils.ShowCommonErrorDialog(parent, err);
                        break;
                    case 2:
                        OtroVO otro = new() {
                            id = null,
                            nombre = _nameEntry.GetText(),
                            descripcion = _dscTextView.Buffer.Text
                        };
                        (err, id) = await Otro.Create(otro);
                        if (err != Errors.Ok) {
                            Utils.ShowCommonErrorDialog(parent, err);
                        }
                        err = await Articulo.AddImages(id, images);
                        Utils.ShowCommonErrorDialog(parent, err);
                        break;
                    default: Utils.ShowCommonErrorDialog(parent, Errors.Unknown); break;
                }
            }
            Close();
        };
        _deleteButton.OnClicked += async (sender, e) => {
            Errors err = await Articulo.Delete(articulo);
            Utils.ShowCommonErrorDialog(parent, err);

            Close();
        };
    }

    void OnEntryNotify(GObject.Object sender, NotifySignalArgs e) {
        if (e.Pspec.GetName() == "text") {
            Validate();
        }
    }
    private void Validate() {
        if (string.IsNullOrEmpty(_nameEntry.GetText())) {
            _saveButton.SetSensitive(false);
        }
        else {
            _saveButton.SetSensitive(true);
        }
    }
}
*/
public class ArticuloDialog {
    protected static GObject.SignalHandler<Gtk.Button> AddCustomProperty(List<Adw.EntryRow> customPropertyRows, Adw.PreferencesGroup customPropertiesGroup, Adw.Window window) {
        return (sender, e) => {
            var dialog = Adw.MessageDialog.New(window, "Nueva propiedad personalizada", "");
            var group = Adw.PreferencesGroup.New();
            var entryRow = Adw.EntryRow.New();
            entryRow.SetTitle("Nombre de la propiedad");
            group.Add(entryRow);
            dialog.SetExtraChild(group);
            dialog.AddResponse("cancel", "Cancelar");
            dialog.AddResponse("suggested", "Añadir");
            dialog.SetResponseAppearance("suggested", Adw.ResponseAppearance.Suggested);
            dialog.SetDefaultResponse("suggested");
            dialog.SetCloseResponse("cancel");
            dialog.OnResponse += (s, ea) => {
                if (!string.IsNullOrEmpty(entryRow.GetText()) && entryRow.GetText() != "NULL") {
                    var row = Adw.EntryRow.New();
                    row.SetTitle(entryRow.GetText());
                    var removeButton = Gtk.Button.New();
                    removeButton.SetValign(Gtk.Align.Center);
                    removeButton.SetTooltipText("Remove Custom Property");
                    removeButton.SetIconName("user-trash-symbolic");
                    removeButton.AddCssClass("flat");
                    removeButton.OnClicked += (sender, e) => {
                        customPropertyRows.Remove(row);
                        customPropertiesGroup.Remove(row);
                    };
                    row.AddSuffix(removeButton);
                    customPropertyRows.Add(row);
                    customPropertiesGroup.Add(row);
                }
                dialog.Destroy();
            };
            dialog.Present();
        };
    }

    protected static GObject.SignalHandler<Gtk.Button> AddImage(List<string> imagesPaths, Gtk.FlowBox images, Adw.Window window) {
        return async (sender, e) => {
            var openFileDialog = Gtk.FileDialog.New();
            openFileDialog.SetTitle("Seleccione una Imagen");
            var filters = Gio.ListStore.New(Gtk.FileFilter.GetGType());
            var filterImages = Gtk.FileFilter.New();
            filterImages.AddMimeType("image/jpeg");
            filterImages.AddMimeType("image/png");
            filterImages.AddMimeType("image/bmp");
            filterImages.AddMimeType("image/webp");
            filters.Append(filterImages);
            openFileDialog.SetFilters(filters);
            try {
                var file = await openFileDialog.OpenAsync(window);
                imagesPaths.Add(file.GetPath());

                using var texture = Gdk.Texture.NewFromFilename(file.GetPath());
                using var image = Gtk.Image.NewFromPaintable(texture);
                image.SetPixelSize(196);

                var button = Gtk.Button.New();
                button.SetChild(image);

                var flowChild = Gtk.FlowBoxChild.New();
                flowChild.SetChild(button);

                images.Append(flowChild);
                button.OnClicked += (sender, e) => {
                    imagesPaths.Remove(file.GetPath());
                    images.Remove(flowChild);
                };
            }
            catch { }
        };
    }
}

public class MaquinariaDialog : ArticuloDialog {
    public static Adw.Window New(MaquinariaVO? maquinaria, Gtk.Window parent) {
        List<string> imagesPaths = new();
        List<Adw.EntryRow> customPropertyRows = new();

        #region Asign variables
        var builder = Builder.FromFile("articulo_dialog.ui");
        // Name Entry
        var nameEntry = builder.GetObject("_nameEntry") as Adw.EntryRow;
        var cantidadEntry = builder.GetObject("_cantidadEntry") as Adw.EntryRow;

        var dscTextView = builder.GetObject("_dscTextView") as Gtk.TextView;
        // PreferencesGroup
        var animalesGroup = builder.GetObject("_animalesGroup") as Adw.PreferencesGroup;
        var maquinariaGroup = builder.GetObject("_maquinariaGroup") as Adw.PreferencesGroup;

        var especieTypeRow = builder.GetObject("_especieTypeRow") as Adw.EntryRow;
        var razaTypeRow = builder.GetObject("_razaTypeRow") as Adw.EntryRow;

        var marcaTypeRow = builder.GetObject("_marcaTypeRow") as Adw.EntryRow;
        var modeloTypeRow = builder.GetObject("_modeloTypeRow") as Adw.EntryRow;
        var añoEntry = builder.GetObject("_añoEntry") as Adw.EntryRow;
        // Calendars
        var nacimientoCalendarButton = builder.GetObject("_dateCalendarButton") as Gtk.MenuButton;
        var nacimientoCalendar = builder.GetObject("_dateCalendar") as Gtk.Calendar;
        // Buttons
        var saveButton = builder.GetObject("_saveButton") as Gtk.Button;
        var deleteButton = builder.GetObject("_deleteButton") as Gtk.Button;
        // Images
        var images = builder.GetObject("_images") as Gtk.FlowBox;
        var addImageButton = builder.GetObject("_addImageButton") as Gtk.Button;
        // Custom properties
        var addNewPropertyButton = builder.GetObject("_addNewPropertyButton") as Gtk.Button;
        var customPropertiesGroup = builder.GetObject("_customPropertiesGroup") as Adw.PreferencesGroup;

        var window = builder.GetObject("_root") as Adw.Window;

        //Dialog Settings
        window.SetTransientFor(parent);
        window.OnCloseRequest += (sender, e) => false;
        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            window.Close();
            return true;
        })));
        window.AddController(shortcutController);
        #endregion

        animalesGroup.SetVisible(false);
        maquinariaGroup.SetVisible(true);

        nameEntry.OnNotify += OnEntryNotify;
        cantidadEntry.OnNotify += OnEntryNotify;
        marcaTypeRow.OnNotify += OnEntryNotify;
        modeloTypeRow.OnNotify += OnEntryNotify;

        if (maquinaria != null) {
            nameEntry.SetText(maquinaria.nombre);
            cantidadEntry.SetText(maquinaria.cantidad.ToString());

            marcaTypeRow.SetText(maquinaria.marca);
            modeloTypeRow.SetText(maquinaria.modelo);
            añoEntry.SetText(maquinaria.año.ToString());

            dscTextView.Buffer.Text = maquinaria.descripcion;

            deleteButton.SetVisible(true);
        }
        else {
            saveButton.SetSensitive(true);
            deleteButton.SetVisible(false);
        }

        /*
        Validate();
        
        nameEntry.OnNotify += OnEntryNotify;
        _dscTextView.OnNotify += OnEntryNotify;

        artTypeRow.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "selected-item") {
                switch (artTypeRow.GetSelected()) {
                    case 0:
                        animalesGroup.SetVisible(true);
                        maquinariaGroup.SetVisible(false);
                        break;
                    case 1:
                        animalesGroup.SetVisible(false);
                        maquinariaGroup.SetVisible(true);
                        break;
                    default:
                        animalesGroup.SetVisible(false);
                        maquinariaGroup.SetVisible(false);
                        break;
                }
            }
        };

        especieTypeRow.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "selected-item") {
                switch (especieTypeRow.GetSelected()) {
                    case 0:
                        razaTypeRow.SetModel(Gtk.StringList.New(new string[] { "Hereford", "Marrón", "Dálmata", "Peluda", "Otro" }));
                        break;
                    case 1:
                        razaTypeRow.SetModel(Gtk.StringList.New(new string[] { "Uno", "Dos", "Tres", "Cuatro", "Otro" }));
                        break;
                }
            }
        };
        */

        nacimientoCalendar.OnDaySelected += (calendar, e) => nacimientoCalendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");

        addNewPropertyButton.OnClicked += AddCustomProperty(customPropertyRows, customPropertiesGroup, window);
        addImageButton.OnClicked += AddImage(imagesPaths, images, window);

        saveButton.OnClicked += async (sender, e) => {
            if (string.IsNullOrEmpty(nameEntry.GetText()) ||
            !Utils.IsNumber(cantidadEntry.GetText()) ||
            string.IsNullOrEmpty(marcaTypeRow.GetText()) ||
            string.IsNullOrEmpty(modeloTypeRow.GetText()) ||
            !Utils.IsNumber(añoEntry.GetText())) { return; }

            MaquinariaVO inputArt = new() {
                id = null,
                nombre = nameEntry.GetText(),
                cantidad = int.Parse(cantidadEntry.GetText()),
                marca = marcaTypeRow.GetText(),
                modelo = modeloTypeRow.GetText(),
                año = int.Parse(añoEntry.GetText()),
                descripcion = dscTextView.Buffer.Text
            };
            if (maquinaria != null) {
                inputArt.id = maquinaria.id;

                Errors err = await Maquinaria.Update(inputArt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                inputArt.id = null;
                var (err, id) = await Maquinaria.Create(inputArt);
                if (err != Errors.Ok) {
                    Utils.ShowCommonErrorDialog(parent, err);
                }
                err = await Articulo.AddImages(id, imagesPaths);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            window.Close();
        };
        deleteButton.OnClicked += async (sender, e) => {
            Errors err = await Maquinaria.Delete(maquinaria);
            Utils.ShowCommonErrorDialog(parent, err);

            window.Close();
        };

        return window;

        void OnEntryNotify(GObject.Object sender, GObject.Object.NotifySignalArgs e) {
            if (e.Pspec.GetName() == "text") {
                Validate();
            }
        }
        void Validate() {
            if (string.IsNullOrEmpty(nameEntry.GetText()) ||
            !Utils.IsNumber(cantidadEntry.GetText()) ||
            string.IsNullOrEmpty(marcaTypeRow.GetText()) ||
            string.IsNullOrEmpty(modeloTypeRow.GetText()) ||
            !Utils.IsNumber(añoEntry.GetText())) {
                saveButton.SetSensitive(false);
            }
            else {
                saveButton.SetSensitive(true);
            }
        }
    }

    /*
    void OnEntryNotify(GObject.Object sender, NotifySignalArgs e) {
        if (e.Pspec.GetName() == "text") {
            Validate();
        }
    }
    private void Validate() {
        if (string.IsNullOrEmpty(_nameEntry.GetText())) {
            _saveButton.SetSensitive(false);
        }
        else {
            _saveButton.SetSensitive(true);
        }
    }
    */
}
public class AnimalDialog : ArticuloDialog {
    public static Adw.Window New(AnimalVO? animal, Gtk.Window parent) {
        List<string> imagesPaths = new();
        List<Adw.EntryRow> customPropertyRows = new();

        #region Asign variables
        var builder = Builder.FromFile("articulo_dialog.ui");
        // Name Entry
        var nameEntry = builder.GetObject("_nameEntry") as Adw.EntryRow;
        var cantidadEntry = builder.GetObject("_cantidadEntry") as Adw.EntryRow;

        var dscTextView = builder.GetObject("_dscTextView") as Gtk.TextView;
        // PreferencesGroup
        var animalesGroup = builder.GetObject("_animalesGroup") as Adw.PreferencesGroup;
        var maquinariaGroup = builder.GetObject("_maquinariaGroup") as Adw.PreferencesGroup;

        var especieTypeRow = builder.GetObject("_especieTypeRow") as Adw.EntryRow;
        var razaTypeRow = builder.GetObject("_razaTypeRow") as Adw.EntryRow;

        var marcaTypeRow = builder.GetObject("_marcaTypeRow") as Adw.EntryRow;
        var modeloTypeRow = builder.GetObject("_modeloTypeRow") as Adw.EntryRow;
        var añoEntry = builder.GetObject("_añoEntry") as Adw.EntryRow;
        // Calendars
        var nacimientoCalendarButton = builder.GetObject("_dateCalendarButton") as Gtk.MenuButton;
        var nacimientoCalendar = builder.GetObject("_dateCalendar") as Gtk.Calendar;
        // Buttons
        var saveButton = builder.GetObject("_saveButton") as Gtk.Button;
        var deleteButton = builder.GetObject("_deleteButton") as Gtk.Button;
        // Images
        var images = builder.GetObject("_images") as Gtk.FlowBox;
        var addImageButton = builder.GetObject("_addImageButton") as Gtk.Button;
        // Custom properties
        var addNewPropertyButton = builder.GetObject("_addNewPropertyButton") as Gtk.Button;
        var customPropertiesGroup = builder.GetObject("_customPropertiesGroup") as Adw.PreferencesGroup;

        var window = builder.GetObject("_root") as Adw.Window;

        //Dialog Settings
        window.SetTransientFor(parent);
        window.OnCloseRequest += (sender, e) => false;
        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            window.Close();
            return true;
        })));
        window.AddController(shortcutController);
        #endregion

        animalesGroup.SetVisible(true);
        maquinariaGroup.SetVisible(false);

        nameEntry.OnNotify += OnEntryNotify;
        cantidadEntry.OnNotify += OnEntryNotify;
        especieTypeRow.OnNotify += OnEntryNotify;
        razaTypeRow.OnNotify += OnEntryNotify;
        cantidadEntry.OnNotify += OnEntryNotify;

        if (animal != null) {
            nameEntry.SetText(animal.nombre);
            cantidadEntry.SetText(animal.cantidad.ToString());

            especieTypeRow.SetText(animal.tipo);
            razaTypeRow.SetText(animal.raza);

            nacimientoCalendar.Day = animal.nacimiento.Day;
            nacimientoCalendar.Month = animal.nacimiento.Month;
            nacimientoCalendar.Year = animal.nacimiento.Year;

            nacimientoCalendarButton.SetLabel($"{animal.nacimiento.Day}/{animal.nacimiento.Month}/{animal.nacimiento.Year}");

            dscTextView.Buffer.Text = animal.descripcion;

            deleteButton.SetVisible(true);
        }
        else {
            saveButton.SetSensitive(true);
            deleteButton.SetVisible(false);
        }

        nacimientoCalendar.OnDaySelected += (calendar, e) => nacimientoCalendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");

        addNewPropertyButton.OnClicked += AddCustomProperty(customPropertyRows, customPropertiesGroup, window);
        addImageButton.OnClicked += AddImage(imagesPaths, images, window);

        saveButton.OnClicked += async (sender, e) => {
            if (string.IsNullOrEmpty(nameEntry.GetText()) ||
                string.IsNullOrEmpty(especieTypeRow.GetText()) ||
                string.IsNullOrEmpty(razaTypeRow.GetText()) ||
                !Utils.IsNumber(cantidadEntry.GetText())) {
                return;
            }

            DateTime elapsedTime = new(nacimientoCalendar.Year, nacimientoCalendar.Month, nacimientoCalendar.Day);

            AnimalVO inputArt = new() {
                nombre = nameEntry.GetText(),
                cantidad = int.Parse(cantidadEntry.GetText()),
                tipo = especieTypeRow.GetText(),
                raza = razaTypeRow.GetText(),
                nacimiento = elapsedTime,
                descripcion = dscTextView.Buffer.Text
            };

            if (animal != null) {
                inputArt.id = animal.id;

                Errors err = await Animal.Update(inputArt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                inputArt.id = null;
                var (err, id) = await Animal.Create(inputArt);
                if (err != Errors.Ok) {
                    Utils.ShowCommonErrorDialog(parent, err);
                }
                err = await Articulo.AddImages(id, imagesPaths);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            window.Close();
        };
        deleteButton.OnClicked += async (sender, e) => {
            Errors err = await Animal.Delete(animal);
            Utils.ShowCommonErrorDialog(parent, err);

            window.Close();
        };

        return window;

        void OnEntryNotify(GObject.Object sender, GObject.Object.NotifySignalArgs e) {
            if (e.Pspec.GetName() == "text") {
                Validate();
            }
        }
        void Validate() {
            if (string.IsNullOrEmpty(nameEntry.GetText()) ||
                string.IsNullOrEmpty(especieTypeRow.GetText()) ||
                string.IsNullOrEmpty(razaTypeRow.GetText()) ||
                !Utils.IsNumber(cantidadEntry.GetText())) {
                saveButton.SetSensitive(false);
            }
            else {
                saveButton.SetSensitive(true);
            }
        }
    }
}
public class OtroDialog : ArticuloDialog {
    public static Adw.Window New(OtroVO? otro, Gtk.Window parent) {
        List<string> imagesPaths = new();
        List<Adw.EntryRow> customPropertyRows = new();

        #region Asign variables
        var builder = Builder.FromFile("articulo_dialog.ui");
        // Name Entry
        var nameEntry = builder.GetObject("_nameEntry") as Adw.EntryRow;
        var cantidadEntry = builder.GetObject("_cantidadEntry") as Adw.EntryRow;

        var dscTextView = builder.GetObject("_dscTextView") as Gtk.TextView;
        // PreferencesGroup
        var animalesGroup = builder.GetObject("_animalesGroup") as Adw.PreferencesGroup;
        var maquinariaGroup = builder.GetObject("_maquinariaGroup") as Adw.PreferencesGroup;

        var especieTypeRow = builder.GetObject("_especieTypeRow") as Adw.EntryRow;
        var razaTypeRow = builder.GetObject("_razaTypeRow") as Adw.EntryRow;

        var marcaTypeRow = builder.GetObject("_marcaTypeRow") as Adw.EntryRow;
        var modeloTypeRow = builder.GetObject("_modeloTypeRow") as Adw.EntryRow;
        var añoEntry = builder.GetObject("_añoEntry") as Adw.EntryRow;
        // Calendars
        var nacimientoCalendarButton = builder.GetObject("_dateCalendarButton") as Gtk.MenuButton;
        var nacimientoCalendar = builder.GetObject("_dateCalendar") as Gtk.Calendar;
        // Buttons
        var saveButton = builder.GetObject("_saveButton") as Gtk.Button;
        var deleteButton = builder.GetObject("_deleteButton") as Gtk.Button;
        // Images
        var images = builder.GetObject("_images") as Gtk.FlowBox;
        var addImageButton = builder.GetObject("_addImageButton") as Gtk.Button;
        // Custom properties
        var addNewPropertyButton = builder.GetObject("_addNewPropertyButton") as Gtk.Button;
        var customPropertiesGroup = builder.GetObject("_customPropertiesGroup") as Adw.PreferencesGroup;

        var window = builder.GetObject("_root") as Adw.Window;

        //Dialog Settings
        window.SetTransientFor(parent);
        window.OnCloseRequest += (sender, e) => false;
        //Shortcut Controller
        var shortcutController = Gtk.ShortcutController.New();
        shortcutController.SetScope(Gtk.ShortcutScope.Managed);
        shortcutController.AddShortcut(Gtk.Shortcut.New(Gtk.ShortcutTrigger.ParseString("Escape"), Gtk.CallbackAction.New((sender, e) => {
            window.Close();
            return true;
        })));
        window.AddController(shortcutController);
        #endregion

        animalesGroup.SetVisible(false);
        maquinariaGroup.SetVisible(false);

        if (otro != null) {
            nameEntry.SetText(otro.nombre);
            cantidadEntry.SetText(otro.cantidad.ToString());

            dscTextView.Buffer.Text = otro.descripcion;

            deleteButton.SetVisible(true);
        }
        else {
            saveButton.SetSensitive(true);
            deleteButton.SetVisible(false);
        }

        nacimientoCalendar.OnDaySelected += (calendar, e) => nacimientoCalendarButton.SetLabel($"{calendar.Day}/{calendar.Month}/{calendar.Year}");

        addNewPropertyButton.OnClicked += AddCustomProperty(customPropertyRows, customPropertiesGroup, window);
        addImageButton.OnClicked += AddImage(imagesPaths, images, window);

        saveButton.OnClicked += async (sender, e) => {
            if (otro != null) {
                OtroVO inputArt = new() {
                    id = otro.id,
                    nombre = nameEntry.GetText(),
                    cantidad = int.Parse(cantidadEntry.GetText()),
                    descripcion = dscTextView.Buffer.Text
                };

                Errors err = await Otro.Update(inputArt);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            else {
                OtroVO animalI = new() {
                    id = null,
                    nombre = nameEntry.GetText(),
                    cantidad = int.Parse(cantidadEntry.GetText()),
                    descripcion = dscTextView.Buffer.Text
                };
                var (err, id) = await Otro.Create(animalI);
                if (err != Errors.Ok) {
                    Utils.ShowCommonErrorDialog(parent, err);
                }
                err = await Articulo.AddImages(id, imagesPaths);
                Utils.ShowCommonErrorDialog(parent, err);
            }
            window.Close();
        };
        deleteButton.OnClicked += async (sender, e) => {
            Errors err = await Otro.Delete(otro);
            Utils.ShowCommonErrorDialog(parent, err);

            window.Close();
        };

        return window;
    }
}
