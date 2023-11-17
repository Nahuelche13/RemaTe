using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using RemaTe.Common.Enums;

namespace RemaTe.DataAccess;

public abstract class Util<T> {
    public static async Task<Errors> Create(T @object) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        using SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();

        // Se crean 2 nuevos strings en Heap
        // Obtenemos el nombre de la clase T,
        // lo pasamos a minusculas y removemos "vo"
        string table = typeof(T).Name.ToLower().Replace("vo", "");

        StringBuilder sb = new();
        sb.Append("INSERT INTO " + table + " (");

        // Las propiedades de T que no sean null
        var properties = typeof(T).GetFields().Where((v) => v.GetValue(@object) != null);

        // Agregamos al buffer los nombres de las propiedades
        foreach (var prop in properties) {
            sb.Append($"{prop.Name}, ");
        }

        // Removemos la ultima coma y espacio
        sb.Remove(sb.Length - 2, 2);

        // Add VALUES clause
        sb.Append(") VALUES (");

        // Agregamos al buffer los nombres de las propiedades con $ al inicio
        foreach (var prop in properties) {
            sb.Append($"${prop.Name}, ");
            cmd.Parameters.AddWithValue("$" + prop.Name, prop.GetValue(@object));
        }

        // Removemos la ultima coma y espacio
        sb.Remove(sb.Length - 2, 2);

        sb.Append(')');

        cmd.CommandText = sb.ToString();

        (int _, Errors e) = await HandleExec(cmd);
        if (e == Errors.Ok) {
            transaction.Commit();
            return Errors.Ok;
        }
        else {
            return e;
        }
    }

    public static async IAsyncEnumerable<T> ReadAll() {
        using SQLiteCommand cmd = new(DBC.I.DbConn);

        string table = typeof(T).Name.ToLower().Replace("vo", "");

        StringBuilder sb = new();
        sb.Append("SELECT ");

        var fields = typeof(T).GetFields();

        // Get column names
        foreach (var prop in fields) {
            sb.Append($"{prop.Name}, ");
        }

        // Remove trailing comma and space
        sb.Remove(sb.Length - 2, 2);

        sb.Append($" FROM {table} ORDER BY ");

        var keyFields = fields.Where((e) => e.CustomAttributes.Any((e) => e.AttributeType == typeof(KeyAttribute)));

        // Get parameters
        foreach (var prop in keyFields) {
            sb.Append($"{prop.Name},");
        }

        // Remove trailing comma and newline
        sb.Remove(sb.Length - 1, 1);
        sb.Append(" ASC");

        sb.Append(';');

        cmd.CommandText = sb.ToString();

        var reader = await cmd.ExecuteReaderAsync();
        int columns = reader.FieldCount;

        object[] args;
        while (reader.Read()) {
            args = new object[columns];
            for (int i = 0; i < columns; i++) {
                args[i] = reader.GetValue(i);
            };
            yield return (T)Activator.CreateInstance(typeof(T), args);
        }
    }

    public static async IAsyncEnumerable<T> ReadWithFilter(T @object) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        using SQLiteTransaction tansaction = DBC.I.DbConn.BeginTransaction();

        string table = typeof(T).Name.ToLower().Replace("vo", "");

        StringBuilder sb = new();
        sb.Append("SELECT ");

        var fields = typeof(T).GetFields();

        // Get column names
        foreach (var prop in fields) {
            sb.Append($"{prop.Name}, ");
        }

        // Remove trailing comma and space
        sb.Remove(sb.Length - 2, 2);

        // Add VALUES clause
        sb.Append($" FROM {table} WHERE ");

        var keyFields = fields.Where((e) => e.CustomAttributes.Any((e) => e.AttributeType == typeof(KeyAttribute)));

        // Get parameters
        foreach (var prop in keyFields) {
            sb.Append($"{prop.Name}=${prop.Name} AND ");
        }

        // Remove trailing comma and newline
        sb.Remove(sb.Length - 5, 5);

        sb.Append(';');

        cmd.CommandText = sb.ToString();

        foreach (var item in keyFields) {
            cmd.Parameters.AddWithValue("$" + item.Name, item.GetValue(@object));
        }

        var reader = await cmd.ExecuteReaderAsync();

        int columns = reader.FieldCount;

        object[] args;
        while (reader.Read()) {
            args = new object[columns];
            for (int i = 0; i < columns; i++) {
                args[i] = reader.GetValue(i);
            };
            yield return (T)Activator.CreateInstance(typeof(T), args);
        }
    }

    public static async Task<Errors> Update(T @object) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        using SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();

        string table = typeof(T).Name.ToLower().Replace("vo", "");

        StringBuilder sb = new();
        sb.Append("UPDATE " + table + " SET ");

        var properties = typeof(T).GetFields().Where((v) => v.GetValue(@object) != null);
        // Get column names
        foreach (var prop in properties) {
            sb.Append($"{prop.Name}=${prop.Name}, ");
        }
        // Remove trailing comma and space
        sb.Remove(sb.Length - 2, 2);

        // Add VALUES clause
        sb.Append(" WHERE ");

        FieldInfo[] fields = typeof(T).GetTypeInfo().GetFields();
        var keyFields = fields.Where((e) => e.CustomAttributes.Any((e) => e.AttributeType == typeof(KeyAttribute)));

        // Get parameters
        foreach (var prop in keyFields) {
            sb.Append($"{prop.Name}=${prop.Name} AND ");
        }

        // Remove trailing comma and newline
        sb.Remove(sb.Length - 5, 5);

        sb.Append(';');

        cmd.CommandText = sb.ToString();

        foreach (var item in typeof(T).GetFields()) {
            cmd.Parameters.AddWithValue("$" + item.Name, item.GetValue(@object));
        }

        (int _, Errors e) = await HandleExec(cmd);
        if (e == Errors.Ok) {
            transaction.Commit();
            return Errors.Ok;
        }
        else {
            return e;
        }
    }

    public static async Task<Errors> Delete(T @object) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        using SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();

        string table = typeof(T).Name.ToLower().Replace("vo", "");

        StringBuilder sb = new();
        sb.Append("DELETE FROM " + table + " WHERE ");

        FieldInfo[] fields = typeof(T).GetTypeInfo().GetFields();
        var keyFields = fields.Where((e) => e.CustomAttributes.Any((e) => e.AttributeType == typeof(KeyAttribute)));

        // Get parameters
        foreach (var prop in keyFields) {
            sb.Append($"{prop.Name}=${prop.Name} AND ");
            cmd.Parameters.AddWithValue("$" + prop.Name, prop.GetValue(@object));
        }

        // Remove trailing comma and newline
        sb.Remove(sb.Length - 5, 5);

        sb.Append(';');

        cmd.CommandText = sb.ToString();

        (int _, Errors e) = await HandleExec(cmd);
        if (e == Errors.Ok) {
            transaction.Commit();
            return Errors.Ok;
        }
        else {
            return e;
        }
    }

    public static async Task<(int rows, Errors e)> HandleExec(SQLiteCommand cmd) {
        try {
            return (await cmd.ExecuteNonQueryAsync(), Errors.Ok);
        }
        catch (SQLiteException e) {
            Console.WriteLine(e.ErrorCode + ": " + e.Message);
            Console.WriteLine(cmd.CommandText);
            return e.ErrorCode switch {
                (int)SQLiteErrorCode.Constraint_Unique => (0, Errors.Duplicated),
                _ => (0, Errors.Unknown),
            };
        }
    }
}
