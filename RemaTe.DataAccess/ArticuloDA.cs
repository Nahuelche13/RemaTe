using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class ArticuloDA {
    //    public static new async Task<(Errors error, int id)> Create(ArticuloVO articulo) {
    //        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();
    //        cmd.CommandText = "INSERT INTO articulo (nombre, descripcion) VALUES ($nombre, $descripcion);";
    //        cmd.Parameters.AddWithValue("$nombre", articulo.nombre);
    //        cmd.Parameters.AddWithValue("$descripcion", articulo.descripcion);
    //        await cmd.ExecuteNonQueryAsync();
    //        cmd.Parameters.Clear();
    //
    //        cmd.CommandText = "SELECT id FROM articulo WHERE rowid = $rowid";
    //        cmd.Parameters.AddWithValue("$rowid", DBC.I.DbConn.LastInsertRowId);
    //        var reader = await cmd.ExecuteReaderAsync();
    //        cmd.Parameters.Clear();
    //
    //        reader.Read();
    //        int id = reader.GetInt32(0);
    //        reader.Close();
    //        return (Errors.Ok, id);
    //    }

    public static async Task<Type> FindDecendant(int id) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = """
            SELECT animal.id_articulo as isAnimal, maquinaria.id_articulo as isMaquinaria, otro.id_articulo isOtro FROM articulo
            FULL JOIN animal ON articulo.id = animal.id_articulo
            FULL JOIN maquinaria ON articulo.id = maquinaria.id_articulo
            FULL JOIN otro ON articulo.id = otro.id_articulo
            WHERE articulo.id = $id;
        """;
        cmd.Parameters.AddWithValue("$id", id);

        var reader = await cmd.ExecuteReaderAsync();
        if (reader.Read()) {
            if (reader.GetFieldType(0) != typeof(DBNull)) {
                return typeof(AnimalVO);
            }
            else if (reader.GetFieldType(1) != typeof(DBNull)) {
                return typeof(MaquinariaVO);
            }
            else if (reader.GetFieldType(2) != typeof(DBNull)) {
                return typeof(OtroVO);
            }
            else {
                return null;
            }
        }
        else {
            return null;
        }
    }

    public static async Task<Errors> AddImages(int id, IEnumerable<string> images) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var image in images) {
            cmd.CommandText = "INSERT INTO imagenes (id_articulo, imagen) VALUES ($id_articulo, $imagen);";
            cmd.Parameters.AddWithValue("$id_articulo", id);
            cmd.Parameters.AddWithValue("$imagen", image);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }

    public static async Task<Errors> AddImage(int id, string image) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = "INSERT INTO imagenes (id_articulo, imagen) VALUES ($id_articulo, $imagen);";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        cmd.Parameters.AddWithValue("$imagen", image);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        return Errors.Ok;
    }
    public static async Task<Errors> RemoveImage(int id, string image) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = "DELETE FROM imagenes WHERE id_articulo=$id_articulo AND imagen=$imagen;";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        cmd.Parameters.AddWithValue("$imagen", image);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        return Errors.Ok;
    }

    public static async Task<List<string>> ReadImagesOf(ArticuloVO articulo) => await ReadImagesOf((int)articulo.id);
    public static async Task<List<string>> ReadImagesOf(int id) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = "SELECT imagen FROM imagenes WHERE id_articulo = $id_articulo";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        var reader = await cmd.ExecuteReaderAsync();

        List<string> images = new();
        while (reader.Read()) {
            // byte[] bytBLOB = new byte[reader.GetBytes(0, 0, null, 0, int.MaxValue) - 1];
            // reader.GetBytes(0, 0, bytBLOB, 0, bytBLOB.Length);
            images.Add(reader.GetString(0));
        }
        return images;
    }
}

public class MaquinariaDA {
    public static async Task<(Errors error, int id)> Create(MaquinariaVO maquinaria) {
        SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();
        var cmd = new SQLiteCommand("", DBC.I.DbConn, transaction);

        cmd.CommandText = "INSERT INTO articulo (nombre, cantidad, descripcion) VALUES ($nombre, $cantidad, $descripcion);";
        cmd.Parameters.AddWithValue("$nombre", maquinaria.nombre);
        cmd.Parameters.AddWithValue("$cantidad", maquinaria.cantidad);
        cmd.Parameters.AddWithValue("$descripcion", maquinaria.descripcion);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        cmd.CommandText = "SELECT id FROM articulo WHERE rowid = $rowid";
        cmd.Parameters.AddWithValue("$rowid", DBC.I.DbConn.LastInsertRowId);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        reader.Read();
        int id = reader.GetInt32(0);
        reader.Close();

        cmd.CommandText = "INSERT INTO maquinaria (id_articulo, marca, modelo, año) VALUES ($id_articulo, $marca, $modelo, $año);";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        cmd.Parameters.AddWithValue("$marca", maquinaria.marca);
        cmd.Parameters.AddWithValue("$modelo", maquinaria.modelo);
        cmd.Parameters.AddWithValue("$año", maquinaria.año);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        transaction.Commit();
        transaction.Dispose();

        return (Errors.Ok, id);
    }

    public static async IAsyncEnumerable<MaquinariaVO> ReadAll() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.marca, m.modelo, m.año, a.descripcion
            FROM maquinaria m
            INNER JOIN articulo a ON a.id=m.id_articulo;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new MaquinariaVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                marca = reader.GetString(3),
                modelo = reader.GetString(4),
                año = reader.GetInt32(5),
                descripcion = reader.GetString(6),
            };
        }
    }

    public static async IAsyncEnumerable<MaquinariaVO> ReadNotLoted() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.marca, m.modelo, m.año, a.descripcion
            FROM maquinaria m
            INNER JOIN articulo a ON a.id=m.id_articulo
            LEFT JOIN pertenece i ON i.id_articulo=m.id_articulo
            WHERE i.id_articulo IS NULL;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new MaquinariaVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                marca = reader.GetString(3),
                modelo = reader.GetString(4),
                año = reader.GetInt32(5),
                descripcion = reader.GetString(6),
            };
        }
    }

    public static async Task<(Errors e, MaquinariaVO maquinaria)> Read(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.marca, m.modelo, m.año, a.descripcion
            FROM maquinaria m
            INNER JOIN articulo a ON a.id=m.id_articulo
            WHERE m.id_articulo=$id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var reader = await cmd.ExecuteReaderAsync();

        if (reader.Read()) {
            return (Errors.Ok, new MaquinariaVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                marca = reader.GetString(3),
                modelo = reader.GetString(4),
                año = reader.GetInt32(5),
                descripcion = reader.GetString(6),
            });
        }
        else {
            return (Errors.NotFound, null);
        }
    }

    public static async Task<Errors> Update(MaquinariaVO maquinaria) {
        var cmd = DBC.I.DbConn.CreateCommand();

        var sb = new StringBuilder();
        sb.AppendLine("UPDATE maquinaria SET ");
        if (maquinaria.marca != null) sb.AppendLine("marca=$marca, ");
        if (maquinaria.modelo != null) sb.AppendLine("modelo=$modelo, ");
        if (maquinaria.año != null) sb.AppendLine("año=$año");
        sb.AppendLine("WHERE id_articulo = $id;");

        sb.AppendLine("UPDATE articulo SET ");
        if (maquinaria.nombre != null) sb.AppendLine("nombre=$nombre, ");
        if (maquinaria.cantidad != null) sb.AppendLine("cantidad=$cantidad, ");
        if (maquinaria.descripcion != null) sb.AppendLine("descripcion=$descripcion");
        sb.AppendLine("WHERE id = $id;");

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddWithValue("$id", maquinaria.id);
        if (maquinaria.marca != null) cmd.Parameters.AddWithValue("$marca", maquinaria.marca);
        if (maquinaria.modelo != null) cmd.Parameters.AddWithValue("$modelo", maquinaria.modelo);
        if (maquinaria.año != null) cmd.Parameters.AddWithValue("$año", maquinaria.año);
        if (maquinaria.nombre != null) cmd.Parameters.AddWithValue("$nombre", maquinaria.nombre);
        if (maquinaria.cantidad != null) cmd.Parameters.AddWithValue("$cantidad", maquinaria.cantidad);
        if (maquinaria.descripcion != null) cmd.Parameters.AddWithValue("$descripcion", maquinaria.descripcion);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<Errors> Delete(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            DELETE FROM maquinaria WHERE id_articulo = $id;
            DELETE FROM articulo WHERE id = $id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
        SELECT propiedades.nombre, posee.valor
        FROM posee
        JOIN propiedades ON propiedades.nombre = posee.nombre_propiedad
        WHERE posee.id_maquinaria = $id_maquinaria
        """;
        cmd.Parameters.AddWithValue("$id_maquinaria", id);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        var properties = new Dictionary<string, string>();
        while (reader.Read()) {
            properties[reader.GetString(0)] = reader.GetString(1);
        }

        return (Errors.Ok, properties);
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "INSERT INTO propiedades (nombre) VALUES ($nombre);";
            cmd.Parameters.AddWithValue("$nombre", propertie.Key);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();

            cmd.CommandText = "INSERT INTO posee (id_maquinaria, nombre_propiedad, valor) VALUES ($id_maquinaria, $nombre_propiedad, $valor);";
            cmd.Parameters.AddWithValue("$id_maquinaria", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie.Key);
            cmd.Parameters.AddWithValue("$valor", propertie.Value);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "DELETE FROM posee WHERE id_maquinaria=$id_maquinaria AND nombre_propiedad=$nombre_propiedad);";
            cmd.Parameters.AddWithValue("$id_maquinaria", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
}

public class AnimalDA {
    public static async Task<(Errors error, int id)> Create(AnimalVO animal) {
        SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();
        var cmd = new SQLiteCommand("", DBC.I.DbConn, transaction);

        cmd.CommandText = "INSERT INTO articulo (nombre, cantidad, descripcion) VALUES ($nombre, $cantidad, $descripcion);";
        cmd.Parameters.AddWithValue("$nombre", animal.nombre);
        cmd.Parameters.AddWithValue("$cantidad", animal.cantidad);
        cmd.Parameters.AddWithValue("$descripcion", animal.descripcion);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        cmd.CommandText = "SELECT id FROM articulo WHERE rowid = $rowid";
        cmd.Parameters.AddWithValue("$rowid", DBC.I.DbConn.LastInsertRowId);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        reader.Read();
        int id = reader.GetInt32(0);
        reader.Close();

        cmd.CommandText = "INSERT INTO animal (id_articulo, tipo, raza, nacimiento) VALUES ($id_articulo, $tipo, $raza, $nacimiento);";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        cmd.Parameters.AddWithValue("$tipo", animal.tipo);
        cmd.Parameters.AddWithValue("$raza", animal.raza);
        cmd.Parameters.AddWithValue("$nacimiento", animal.nacimiento);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        transaction.Commit();
        transaction.Dispose();

        return (Errors.Ok, id);
    }

    public static async IAsyncEnumerable<AnimalVO> ReadAll() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.tipo, m.raza, m.nacimiento, a.descripcion
            FROM animal m
            INNER JOIN articulo a ON a.id=m.id_articulo;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new AnimalVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                tipo = reader.GetInt32(3),
                raza = reader.GetString(4),
                nacimiento = reader.GetDateTime(5),
                descripcion = reader.GetString(6),
            };
        }
    }

    public static async IAsyncEnumerable<AnimalVO> ReadNotLoted() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.tipo, m.raza, m.nacimiento, a.descripcion
            FROM animal m
            INNER JOIN articulo a ON a.id=m.id_articulo
            LEFT JOIN pertenece p ON p.id_articulo=m.id_articulo
            WHERE p.id_articulo IS NULL;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new AnimalVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                tipo = reader.GetInt32(3),
                raza = reader.GetString(4),
                nacimiento = reader.GetDateTime(5),
                descripcion = reader.GetString(6),
            };
        }
    }

    public static async Task<(Errors e, AnimalVO animal)> Read(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, m.tipo, m.raza, m.nacimiento, a.descripcion
            FROM animal m
            INNER JOIN articulo a ON a.id=m.id_articulo
            WHERE m.id_articulo=$id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var reader = await cmd.ExecuteReaderAsync();

        if (reader.Read()) {
            return (Errors.Ok, new AnimalVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                tipo = reader.GetInt32(3),
                raza = reader.GetString(4),
                nacimiento = reader.GetDateTime(5),
                descripcion = reader.GetString(6),
            });
        }
        else {
            return (Errors.NotFound, null);
        }
    }

    public static async Task<Errors> Update(AnimalVO animal) {
        var cmd = DBC.I.DbConn.CreateCommand();

        var sb = new StringBuilder();
        sb.AppendLine("UPDATE animal SET ");
        if (animal.tipo != null) sb.AppendLine("tipo=$tipo");
        if (animal.raza != null) sb.AppendLine("raza=$raza");
        if (animal.nacimiento != null) sb.AppendLine("nacimiento=$nacimiento");
        sb.AppendLine("WHERE id_articulo = $id;");

        sb.AppendLine("UPDATE articulo SET ");
        if (animal.nombre != null) sb.AppendLine("nombre=$nombre");
        if (animal.cantidad != null) sb.AppendLine("cantidad=$cantidad");
        if (animal.descripcion != null) sb.AppendLine("descripcion=$descripcion");
        sb.AppendLine("WHERE id_ = $id;");

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddWithValue("id", animal.id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<Errors> Delete(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            DELETE FROM animal WHERE id_articulo = $id;
            DELETE FROM articulo WHERE id = $id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
        SELECT nombre, valor
        FROM atesora
        JOIN propiedades ON propiedades.nombre = atesora.nombre_propiedad
        WHERE atesora.id_animal = $id_animal
        """;
        cmd.Parameters.AddWithValue("$id_animal", id);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        var properties = new Dictionary<string, string>();
        while (reader.Read()) {
            properties[reader.GetString(0)] = reader.GetString(1);
        }

        return (Errors.Ok, properties);
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "INSERT INTO propiedades (nombre) VALUES ($nombre);";
            cmd.Parameters.AddWithValue("$nombre", propertie.Key);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();

            cmd.CommandText = "INSERT INTO atesora (id_animal, nombre_propiedad, valor) VALUES ($id_animal, $nombre_propiedad, $valor);";
            cmd.Parameters.AddWithValue("$id_animal", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie.Key);
            cmd.Parameters.AddWithValue("$valor", propertie.Value);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "DELETE FROM atesora WHERE id_animal=$id_animal AND nombre_propiedad=$nombre_propiedad);";
            cmd.Parameters.AddWithValue("$id_animal", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
}

public class OtroDA {
    public static async Task<(Errors error, int id)> Create(OtroVO otro) {
        SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();
        var cmd = new SQLiteCommand("", DBC.I.DbConn, transaction);

        cmd.CommandText = "INSERT INTO articulo (nombre, cantidad, descripcion) VALUES ($nombre, $cantidad, $descripcion);";
        cmd.Parameters.AddWithValue("$nombre", otro.nombre);
        cmd.Parameters.AddWithValue("$cantidad", otro.cantidad);
        cmd.Parameters.AddWithValue("$descripcion", otro.descripcion);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        cmd.CommandText = "SELECT id FROM articulo WHERE rowid = $rowid";
        cmd.Parameters.AddWithValue("$rowid", DBC.I.DbConn.LastInsertRowId);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        reader.Read();
        int id = reader.GetInt32(0);
        reader.Close();

        cmd.CommandText = "INSERT INTO otro (id_articulo) VALUES ($id_articulo);";
        cmd.Parameters.AddWithValue("$id_articulo", id);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        transaction.Commit();
        transaction.Dispose();

        return (Errors.Ok, id);
    }

    public static async IAsyncEnumerable<OtroVO> ReadAll() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, a.descripcion
            FROM otro m
            INNER JOIN articulo a ON a.id=m.id_articulo;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new OtroVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                descripcion = reader.GetString(3),
            };
        }
    }

    public static async IAsyncEnumerable<OtroVO> ReadNotLoted() {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, a.descripcion
            FROM otro m
            INNER JOIN articulo a ON a.id=m.id_articulo
            LEFT JOIN pertenece i ON i.id_articulo=m.id_articulo
            WHERE i.id_articulo IS NULL;
        """;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new OtroVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                descripcion = reader.GetString(3),
            };
        }
    }

    public static async Task<(Errors e, OtroVO otro)> Read(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            SELECT a.id, a.nombre, a.cantidad, a.descripcion
            FROM otro m
            INNER JOIN articulo a ON a.id=m.id_articulo
            WHERE m.id_articulo=$id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var reader = await cmd.ExecuteReaderAsync();

        if (reader.Read()) {
            return (Errors.Ok, new OtroVO() {
                id = reader.GetInt32(0),
                id_articulo = reader.GetInt32(0),
                nombre = reader.GetString(1),
                cantidad = reader.GetInt32(2),
                descripcion = reader.GetString(3),
            });
        }
        else {
            return (Errors.NotFound, null);
        }
    }

    public static async Task<Errors> Update(OtroVO otro) {
        var cmd = DBC.I.DbConn.CreateCommand();

        var sb = new StringBuilder();
        sb.AppendLine("UPDATE articulo SET ");
        if (otro.nombre != null) sb.AppendLine("nombre=$nombre");
        if (otro.cantidad != null) sb.AppendLine("otro=$otro");
        if (otro.descripcion != null) sb.AppendLine("descripcion=$descripcion");
        sb.AppendLine("WHERE id_ = $id;");

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddWithValue("id", otro.id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<Errors> Delete(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            DELETE FROM otro WHERE id_articulo = $id;
            DELETE FROM articulo WHERE id = $id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }


    public static async Task<(Errors error, IDictionary<string, string> properties)> ReadProperties(int id) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
        SELECT nombre, valor
        FROM tiene
        JOIN propiedades ON propiedades.nombre = tiene.nombre_propiedad
        WHERE tiene.id_otro = $id_otro
        """;
        cmd.Parameters.AddWithValue("$id_otro", id);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        var properties = new Dictionary<string, string>();
        while (reader.Read()) {
            properties[reader.GetString(0)] = reader.GetString(1);
        }

        return (Errors.Ok, properties);
    }
    public static async Task<Errors> AddProperties(int id, IDictionary<string, string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "INSERT INTO propiedades (nombre) VALUES ($nombre);";
            cmd.Parameters.AddWithValue("$nombre", propertie.Key);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();

            cmd.CommandText = "INSERT INTO tiene (id_otro, nombre_propiedad, valor) VALUES ($id_otro, $nombre_propiedad, $valor);";
            cmd.Parameters.AddWithValue("$id_otro", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie.Key);
            cmd.Parameters.AddWithValue("$valor", propertie.Value);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
    public static async Task<Errors> RemoveProperties(int id, IEnumerable<string> properties) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var propertie in properties) {
            cmd.CommandText = "DELETE FROM posee WHERE id_maquinaria=$id_maquinaria AND nombre_propiedad=$nombre_propiedad);";
            cmd.Parameters.AddWithValue("$id_maquinaria", id);
            cmd.Parameters.AddWithValue("$nombre_propiedad", propertie);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
}
