using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class Articulo {
    public static async Task<(Errors, Type)> FindDecendant(int id) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, await ArticuloDA.FindDecendant(id))
            : (Errors.Unauthorized, null);
    }

    public static async Task<Errors> RemoveImage(int id, string image) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await ArticuloDA.RemoveImage(id, image)
            : Errors.Unauthorized;
    }

    public static async Task<Errors> AddImages(int id, List<string> images) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await ArticuloDA.AddImages(id, images)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors error, List<string> values)> ReadImages(ArticuloVO articulo) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, await ArticuloDA.ReadImagesOf(articulo))
            : (Errors.Unauthorized, null);
    }
}

public class Maquinaria {
    public static async Task<(Errors error, int id)> Create(MaquinariaVO maquinaria) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.Create(maquinaria)
            : (Errors.Unauthorized, -1);
    }
    public static (Errors error, IAsyncEnumerable<MaquinariaVO> values) ReadAll() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, MaquinariaDA.ReadAll())
            : (Errors.Unauthorized, null);
    }
    public static (Errors error, IAsyncEnumerable<MaquinariaVO> values) ReadNotLoted() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, MaquinariaDA.ReadNotLoted())
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> Update(MaquinariaVO maquinaria) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.Update(maquinaria)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Delete(MaquinariaVO maquinaria) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.Delete((int)maquinaria.id)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.AddProperties(id, properties)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.ReadProperties(id)
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await MaquinariaDA.RemoveProperties(id, properties)
            : Errors.Unauthorized;
    }
}

public class Animal {
    public static async Task<(Errors error, int id)> Create(AnimalVO animal) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.Create(animal)
            : (Errors.Unauthorized, -1);
    }
    public static (Errors error, IAsyncEnumerable<AnimalVO> values) ReadAll() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, AnimalDA.ReadAll())
            : (Errors.Unauthorized, null);
    }
    public static (Errors error, IAsyncEnumerable<AnimalVO> values) ReadNotLoted() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, AnimalDA.ReadNotLoted())
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> Update(AnimalVO animal) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.Update(animal)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Delete(AnimalVO animal) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.Delete((int)animal.id)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.ReadProperties(id)
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.AddProperties(id, properties)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await AnimalDA.RemoveProperties(id, properties)
            : Errors.Unauthorized;
    }
}

public class Otro {
    public static async Task<(Errors error, int id)> Create(OtroVO otro) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.Create(otro)
            : (Errors.Unauthorized, -1);
    }
    public static (Errors error, IAsyncEnumerable<OtroVO> values) ReadAll() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, OtroDA.ReadAll())
            : (Errors.Unauthorized, null);
    }
    public static (Errors error, IAsyncEnumerable<OtroVO> values) ReadNotLoted() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? (Errors.Ok, OtroDA.ReadNotLoted())
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> Update(OtroVO otro) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.Update(otro)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Delete(OtroVO otro) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.Delete((int)otro.id)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.ReadProperties(id)
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.AddProperties(id, properties)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await OtroDA.RemoveProperties(id, properties)
            : Errors.Unauthorized;
    }
}
