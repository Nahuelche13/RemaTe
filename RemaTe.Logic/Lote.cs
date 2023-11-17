using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class Lote {
    public static (Errors errors, IAsyncEnumerable<LoteRep> lotes) ReadAllWithArticulo() {
        return true
            ? (Errors.Ok, LoteDA.ReadAllWithArticulo())
            : (Errors.Unauthorized, null);
    }

    public static (Errors errors, IAsyncEnumerable<LoteRep> lotes) ReadAllWithArticuloNotLoted() {
        return true
            ? (Errors.Ok, LoteDA.ReadAllWithArticuloNotLoted())
            : (Errors.Unauthorized, null);
    }

    public static async Task<Errors> AddArticulos(int id, List<ArticuloVO> articulos) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Empleado
            ? await LoteDA.AddArticulos(id, articulos)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors error, int id)> Create(LoteVO lote) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await LoteDA.Create(lote)
            : (Errors.Unauthorized, -1);
    }
    public static (Errors error, int monto) GetSellPrice(int id) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? LoteDA.GetSellPrice(id)
            : (Errors.Unauthorized, -1);
    }
    public static async Task<Errors> Sell(int id, int buyer, int price) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await LoteDA.Sell(id, buyer, price)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Update(LoteVO lote) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await LoteDA.Update(lote)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Delete(LoteVO articulo) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Vendedor
            ? await LoteDA.Delete(articulo)
            : Errors.Unauthorized;
    }
}
