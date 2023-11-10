using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;
using RemaTe.Logic;

namespace RemaTe.Test;
public class Test {
    public static async Task Main() {
        await Usuario.LogIn(0, "root"); Usuario.LogOut();

        IAsyncEnumerable<object[]> enumerator = GenericL<Test>.Exec(@"
            SELECT r.id, r.inicio, r.duracion, r.tipo, r.metodos_pago
            FROM remate r
            JOIN lote_remate lr ON r.id == lr.id_remate
            JOIN lote l           ON l.id == lr.id_lote
            ORDER BY r.duracion"
        );
        await foreach (var item in enumerator) {
            foreach (var arts in item) {
                Console.Write(arts + " | ");
            }
            Console.WriteLine("");
        }

        await Usuario.LogIn(0, "root");
        var (error, values) = Lote.ReadAllWithArticulo();
        if (error != Errors.Ok) {
            Console.WriteLine(error);
            return;
        }
        //----------------------------------------------------------------//
        // Console.WriteLine("Before User tests");
        // printMem();
        // await TestUsuariosAsync();
        // 
        // Console.WriteLine("After Artiulos tests");
        // printMem();
        // await TestArtiulosAsync();
        // 
        // Console.WriteLine("After Cliente tests");
        // printMem();
        // await TestClienteAsync();
        // 
        // Console.WriteLine("After Articulo tests");
        // printMem();
        //----------------------------------------------------------------//
    }

    static async Task TestUsuariosAsync() {
        bool loged = await Usuario.LogIn(0, "root");
        if (!loged) throw new Exception("");

        string hash = Hasher.Hash("contra");

        UsuarioVO user1 = new() { id = 1, hash_pwd = hash, permisos = (int)Permissions.Empleado };
        await Usuario.Create(user1);

        UsuarioVO user2 = new() { id = 2, hash_pwd = hash, permisos = (int)Permissions.Empleado };
        await Usuario.Create(user2);
        user2.permisos = (int)Permissions.Admin;
        await Usuario.Update(user2);

        await Usuario.Delete(user1);

        Usuario.LogOut();

        bool loged1 = await Usuario.LogIn(2, "contra");
        if (!loged1) throw new Exception("");
        await Usuario.Delete(user2);
        Usuario.LogOut();
    }

    static async Task TestArtiulosAsync() {
        await Usuario.LogIn(0, "root");

        ArticuloVO articulo = new() { id = 1, nombre = "Juanes", precio_base = 600, descripcion = "Larga descripcion", comision = 6 };
        var err = await Articulo.Create(articulo);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        ArticuloVO articulo2 = new() { id = 1, nombre = "Caballos" };
        err = await Articulo.Update(articulo2);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        (err, var arts) = Articulo.ReadAll();
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        await foreach (var item in arts) {
            Console.WriteLine("Nombre: " + item.nombre);
            if (item.nombre == "Caballos") {
                break;
            }
            else {
                throw new Exception(item.nombre);
            }
        }

        err = await Articulo.Delete(articulo);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        (err, arts) = Articulo.ReadAll();
        await foreach (var item in arts) {
            throw new Exception(item.nombre);
        }

        Usuario.LogOut();
    }

    static async Task TestClienteAsync() {
        await Usuario.LogIn(0, "root");

        var cliente = new ClienteVO() { ci = 0, nombre = "Nombre", email = "Email", telefono = 0, departamento = 0, localidad = 0, calle = " ", puerta = 0 };
        var err = await Cliente.Create(cliente);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        cliente.nombre = "Waldemar";
        err = await Cliente.Update(cliente);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        (err, var arts) = Cliente.ReadAll();
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }
        await foreach (var item in arts) {
            Console.WriteLine("Nombre: " + item.nombre);
            if (item.nombre == "Waldemar") {
                break;
            }
            else {
                throw new Exception(item.nombre);
            }
        }

        err = await Cliente.Delete(cliente);
        if (err != Errors.Ok) { throw new Exception(err.ToString()); }

        (err, arts) = Cliente.ReadAll();
        await foreach (var item in arts) {
            throw new Exception(item.nombre);
        }

        Usuario.LogOut();
    }

    static void printMem() {
        Console.WriteLine("Reserved memory: " + Environment.WorkingSet / 1024 + "Kb total memory");
        // GC.Collect();
        Console.WriteLine("Memory allocated: " + GC.GetTotalMemory(false) / 1024 + " Kb managed memory");
        Console.WriteLine("Memory allocated: " + GC.GetTotalMemory(true) / 1024 + " Kb managed memory after clean");
    }

    static async void PrintEnumeratorAsync<T>(IAsyncEnumerator<T> enumerator) {
        printHeaders(enumerator.Current);
        while (await enumerator.MoveNextAsync()) {
            printValues(enumerator.Current);
        }

        static void printHeaders(T _) {
            FieldInfo[] members = typeof(T).GetFields();
            foreach (FieldInfo item in members) {
                Console.Write(item.Name + "\t");
            }
            Console.WriteLine();
        }
        static void printValues(T obj) {
            FieldInfo[] members = typeof(T).GetFields();
            foreach (FieldInfo item in members) {
                Console.Write(item.GetValue(obj) + "\t");
            }
            Console.WriteLine();
        }
    }
}
