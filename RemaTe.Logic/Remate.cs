using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class Remate {
    public static (Errors errors, IAsyncEnumerable<RemateRep> lotes) ReadAllFutureWithLote() {
        return true
            ? (Errors.Ok, RemateDA.ReadAllFutureWithLote())
            : (Errors.Unauthorized, null);
    }
    public static (Errors errors, IAsyncEnumerable<RemateRep> lotes) ReadAllPastWithLote() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos >= (byte)Permissions.Empleado
            ? (Errors.Ok, RemateDA.ReadAllPastWithLote())
            : (Errors.Unauthorized, null);
    }
    public static async Task<Errors> AddLotes(int id, List<LoteVO> lotes) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos >= (byte)Permissions.Empleado
            ? await RemateDA.AddLotes(id, lotes)
            : Errors.Unauthorized;
    }
    public static async Task<(Errors errors, int id)> Create(RemateVO remate) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos >= (byte)Permissions.Empleado
            ? await RemateDA.Create(remate)
            : (Errors.Unauthorized, -1);
    }
    public static async Task<Errors> Update(RemateVO remate) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos >= (byte)Permissions.Empleado
            ? await RemateDA.Update(remate)
            : Errors.Unauthorized;
    }
}
