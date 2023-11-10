using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class GenericL<T> {
    public static IAsyncEnumerable<object[]> Exec(string command) => DBC.Exec(command);

    public static async Task<Errors> Create(T value) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? await Util<T>.Create(value)
            : Errors.Unauthorized;
    }

    public static (Errors error, IAsyncEnumerable<T> values) Read(T value) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, Util<T>.ReadWithFilter(value))
            : (Errors.Unauthorized, null);
    }

    public static (Errors error, IAsyncEnumerable<T> values) ReadAll() {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? (Errors.Ok, Util<T>.ReadAll())
            : (Errors.Unauthorized, null);
    }

    public static async Task<Errors> Delete(T value) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? await Util<T>.Delete(value)
            : Errors.Unauthorized;
    }

    public static async Task<Errors> Update(T value) {
        return Usuario.I.Auth != null && Usuario.I.Auth.permisos == (int)Permissions.Admin
            ? await Util<T>.Update(value)
            : Errors.Unauthorized;
    }
}

// public class Empleado : GenericLogic { }
// public class Tarea : GenericLogic { }
// public class Entrada : GenericLogic { }
// public class Pago : GenericLogic { }
// public class Lote : GenericLogic { }
// public class Remate : GenericLogic { }
// public class Imagenes : GenericLogic { }
// public class Lote_remate : GenericLogic { }
// public class Usuario_cliente : GenericLogic { }
// public class Usuario_empleado : GenericLogic { }
// public class Articulo_lote : GenericLogic { }
// public class Cliente_puja : GenericLogic { }
// public class Cliente_articulo : GenericLogic { }
// public class Departamento : GenericLogic { }
// public class Localidad : GenericLogic { }
