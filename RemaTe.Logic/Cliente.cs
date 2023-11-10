using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;


namespace RemaTe.Logic;
public static class Cliente {
    public static async Task<Errors> Create(ClienteVO cliente) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Empleado
            ? await ClienteDA.Create(cliente)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Delete(ClienteVO cliente) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Empleado
            ? await ClienteDA.Delete(cliente)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Update(ClienteVO cliente) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Empleado
            ? await ClienteDA.Update(cliente)
            : Errors.Unauthorized;
    }
    public static (Errors error, IAsyncEnumerable<ClienteVO> values) ReadAll() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, ClienteDA.ReadAll())
            : (Errors.Unauthorized, null);
    }
    public static (Errors error, IAsyncEnumerable<ClienteVO> values) ReadAllWithUsuario() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, ClienteDA.ReadAllWithUsuario())
            : (Errors.Unauthorized, null);
    }
}
