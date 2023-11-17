using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class Empleado {
    public static async Task<Errors> Create(EmpleadoVO empleado) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos > (byte)Permissions.Empleado
            ? await EmpleadoDA.Create(empleado)
            : Errors.Unauthorized;
    }
    public static (Errors error, IAsyncEnumerable<EmpleadoVO> values) ReadAllWithUsuario() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, EmpleadoDA.ReadAllWithUsuario())
            : (Errors.Unauthorized, null);
    }
    public static (Errors error, IAsyncEnumerable<EmpleadoVO> values) ReadAllWithUsuarioWhere(UsuarioVO filer) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, EmpleadoDA.ReadAllWithUsuarioWhere(filer))
            : (Errors.Unauthorized, null);
    }
}
