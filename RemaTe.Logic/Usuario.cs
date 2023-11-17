using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.DataAccess;

namespace RemaTe.Logic;
public class Usuario {
    private static Usuario s_instance;
    public static Usuario I {
        get {
            s_instance ??= new Usuario();
            return s_instance;
        }
    }

    private UsuarioVO auth;

    public UsuarioVO Auth {
        get => auth;
        private set {
            auth = value;
            OnAuth?.Invoke();
        }
    }
    public event System.Action OnAuth;

    public static async Task<bool> LogIn(int ci, string pwd) {
        string? hash = await UsuarioDA.GetHash(ci);
        if (hash == null) {
            return false;
        }
        else if (Hasher.Verify(pwd, hash)) {
            I.Auth = await Helpper.GetFirstOf(UsuarioDA.ReadWithFilter(new UsuarioVO() { id = ci }));
            return true;
        }
        else {
            return false;
        }

    }

    public static void LogOut() {
        I.Auth = null;
        //OnAuth.Invoke();
        //_dbConn.Dispose();
    }

    public static async Task<Errors> Create(UsuarioVO user) {
        return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
            ? await UsuarioDA.Create(user)
            : Errors.Unauthorized;
    }

    // public static (Errors error, IAsyncEnumerable<UsuarioVO> values) ReadAll() {
    //     return I.Auth != null && I.Auth.permisos >= (byte)Permissions.Empleado
    //         ? (Errors.Ok, UsuarioDA.ReadAll())
    //         : (Errors.Unauthorized, null);
    // }
    // public static (Errors error, IAsyncEnumerable<UsuarioVO> values) Read(UsuarioVO usr) {
    //     return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
    //         ? (Errors.Ok, UsuarioDA.ReadWithFilter(usr))
    //         : (Errors.Unauthorized, null);
    // }

    public static async Task<Errors> Delete(UsuarioVO usr) {
        return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
            ? await UsuarioDA.Delete(usr)
            : Errors.Unauthorized;
    }

    public static async Task<Errors> DeleteGDPR(int id) {
        return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
            ? await UsuarioDA.DeleteGDPR(id)
            : Errors.Unauthorized;
    }
    public static async Task<Errors> Activate(int id) {
        return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
            ? await UsuarioDA.Activate(id)
            : Errors.Unauthorized;
    }

    public static async Task<Errors> Update(UsuarioVO user) {
        return I.Auth != null && I.Auth.permisos == (int)Permissions.Admin
            ? await UsuarioDA.Update(user)
            : Errors.Unauthorized;
    }
}
