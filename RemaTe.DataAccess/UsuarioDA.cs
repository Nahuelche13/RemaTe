using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

using RemaTe.Common.Enums;

using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class UsuarioDA {
    public static async Task<string?> GetHash(int ci) {
        SQLiteCommand command = DBC.I.DbConn.CreateCommand();
        command.CommandText = @"SELECT hash_pwd FROM usuario WHERE id = $id";
        command.Parameters.AddWithValue("$id", ci);

        var reader = await command.ExecuteScalarAsync();

        if (reader != null) {
            string hash_pwd = reader.ToString();
            return hash_pwd;
        }
        return null;
    }
    /*
        public static new async Task<Errors> Delete(UsuarioVO usuario) {
            var cmd = DBC.I.DbConn.CreateCommand();

            var sb = new StringBuilder();
            sb.AppendLine("UPDATE usuario SET ");
            if (usuario.hash_pwd != null) sb.AppendLine("hash_pwd=$hash_pwd");
            if (usuario.permisos != null) sb.AppendLine("permisos=$permisos");
            if (usuario.nombre != null) sb.AppendLine("nombre=$nombre");
            if (usuario.email != null) sb.AppendLine("email=$email");
            if (usuario.telefono != null) sb.AppendLine("telefono=$telefono");
            if (usuario.departamento != null) sb.AppendLine("departamento=$departamento");
            if (usuario.localidad != null) sb.AppendLine("localidad=$localidad");
            if (usuario.calle != null) sb.AppendLine("calle=$calle");
            if (usuario.puerta != null) sb.AppendLine("puerta=$puerta");
            if (usuario.is_active != null) sb.AppendLine("is_active=$is_active");
            sb.AppendLine("WHERE id = $id;");

            cmd.CommandText = sb.ToString();
            cmd.Parameters.AddWithValue("$id", usuario.id);
            if (usuario.hash_pwd != null) cmd.Parameters.AddWithValue("$hash_pwd", usuario.hash_pwd);
            if (usuario.permisos != null) cmd.Parameters.AddWithValue("$permisos", usuario.permisos);
            if (usuario.nombre != null) cmd.Parameters.AddWithValue("$nombre", usuario.nombre);
            if (usuario.email != null) cmd.Parameters.AddWithValue("$email", usuario.email);
            if (usuario.telefono != null) cmd.Parameters.AddWithValue("$telefono", usuario.telefono);
            if (usuario.departamento != null) cmd.Parameters.AddWithValue("$departamento", usuario.departamento);
            if (usuario.localidad != null) cmd.Parameters.AddWithValue("$localidad", usuario.localidad);
            if (usuario.calle != null) cmd.Parameters.AddWithValue("$calle", usuario.calle);
            if (usuario.puerta != null) cmd.Parameters.AddWithValue("$puerta", usuario.puerta);
            if (usuario.is_active != null) cmd.Parameters.AddWithValue("$is_active", usuario.is_active);

            var affected = await cmd.ExecuteNonQueryAsync();

            return affected > 0 ? Errors.Ok : Errors.NotFound;
        }
    */

    public static async Task<Errors> Create(UsuarioVO usuario) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = "INSERT INTO usuario (id, nombre, hash_pwd, permisos, email, telefono, departamento, localidad, calle, puerta, is_active) VALUES ($id, $nombre, $hash_pwd, $permisos, $email, $telefono, $departamento, $localidad, $calle, $puerta, $is_active);";

        cmd.Parameters.AddWithValue("$id", usuario.id);
        cmd.Parameters.AddWithValue("$nombre", usuario.nombre);
        cmd.Parameters.AddWithValue("$hash_pwd", usuario.hash_pwd);
        cmd.Parameters.AddWithValue("$permisos", usuario.permisos);
        cmd.Parameters.AddWithValue("$email", usuario.email);
        cmd.Parameters.AddWithValue("$telefono", usuario.telefono);
        cmd.Parameters.AddWithValue("$departamento", usuario.departamento);
        cmd.Parameters.AddWithValue("$localidad", usuario.localidad);
        cmd.Parameters.AddWithValue("$calle", usuario.calle);
        cmd.Parameters.AddWithValue("$puerta", usuario.puerta);
        cmd.Parameters.AddWithValue("$is_active", usuario.is_active ?? true);

        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        return Errors.Ok;
    }

    public static async IAsyncEnumerable<UsuarioVO> ReadWithFilter(UsuarioVO usuario) {
        var cmd = DBC.I.DbConn.CreateCommand();

        var sb = new StringBuilder();
        sb.AppendLine("""
        SELECT id, nombre, hash_pwd, permisos, email, telefono, departamento, localidad, calle, puerta, is_active
        FROM usuario
        """
        );

        sb.AppendLine("WHERE ");
        if (usuario.hash_pwd != null) sb.AppendLine("hash_pwd=$hash_pwd AND ");
        if (usuario.permisos != null) sb.AppendLine("permisos=$permisos AND ");
        if (usuario.nombre != null) sb.AppendLine("nombre=$nombre AND ");
        if (usuario.email != null) sb.AppendLine("email=$email AND ");
        if (usuario.telefono != null) sb.AppendLine("telefono=$telefono AND ");
        if (usuario.departamento != null) sb.AppendLine("departamento=$departamento AND ");
        if (usuario.localidad != null) sb.AppendLine("localidad=$localidad AND ");
        if (usuario.calle != null) sb.AppendLine("calle=$calle AND ");
        if (usuario.puerta != null) sb.AppendLine("puerta=$puerta AND ");
        if (usuario.is_active != null) sb.AppendLine("is_active=$is_active AND ");

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddWithValue("$id", usuario.id);

        if (usuario.hash_pwd != null) cmd.Parameters.AddWithValue("$hash_pwd", usuario.hash_pwd);
        if (usuario.permisos != null) cmd.Parameters.AddWithValue("$permisos", usuario.permisos);
        if (usuario.nombre != null) cmd.Parameters.AddWithValue("$nombre", usuario.nombre);
        if (usuario.email != null) cmd.Parameters.AddWithValue("$email", usuario.email);
        if (usuario.telefono != null) cmd.Parameters.AddWithValue("$telefono", usuario.telefono);
        if (usuario.departamento != null) cmd.Parameters.AddWithValue("$departamento", usuario.departamento);
        if (usuario.localidad != null) cmd.Parameters.AddWithValue("$localidad", usuario.localidad);
        if (usuario.calle != null) cmd.Parameters.AddWithValue("$calle", usuario.calle);
        if (usuario.puerta != null) cmd.Parameters.AddWithValue("$puerta", usuario.puerta);
        if (usuario.is_active != null) cmd.Parameters.AddWithValue("$is_active", usuario.is_active);


        sb.Remove(sb.Length - 5, 5);
        cmd.CommandText = sb.ToString();
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync()) {
            yield return new UsuarioVO() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                hash_pwd = reader.GetString(2),
                permisos = reader.GetInt32(3),
                email = reader.GetString(4),
                telefono = reader.GetInt32(5),
                departamento = reader.GetInt32(6),
                localidad = reader.GetString(7),
                calle = reader.GetString(8),
                puerta = reader.GetInt32(9),
                is_active = reader.GetBoolean(10),
            };
        }
    }

    public static async Task<Errors> Update(UsuarioVO usuario) {
        var cmd = DBC.I.DbConn.CreateCommand();

        var sb = new StringBuilder();
        sb.AppendLine("UPDATE usuario SET ");
        if (usuario.hash_pwd != null) sb.AppendLine("hash_pwd=$hash_pwd, ");
        if (usuario.permisos != null) sb.AppendLine("permisos=$permisos, ");
        if (usuario.nombre != null) sb.AppendLine("nombre=$nombre, ");
        if (usuario.email != null) sb.AppendLine("email=$email, ");
        if (usuario.telefono != null) sb.AppendLine("telefono=$telefono, ");
        if (usuario.departamento != null) sb.AppendLine("departamento=$departamento, ");
        if (usuario.localidad != null) sb.AppendLine("localidad=$localidad, ");
        if (usuario.calle != null) sb.AppendLine("calle=$calle, ");
        if (usuario.puerta != null) sb.AppendLine("puerta=$puerta, ");
        if (usuario.is_active != null) sb.AppendLine("is_active=$is_active, ");
        sb.Remove(sb.Length - 3, 3);
        sb.AppendLine(" WHERE id = $id;");

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddWithValue("$id", usuario.id);

        if (usuario.hash_pwd != null) cmd.Parameters.AddWithValue("$hash_pwd", usuario.hash_pwd);
        if (usuario.permisos != null) cmd.Parameters.AddWithValue("$permisos", usuario.permisos);
        if (usuario.nombre != null) cmd.Parameters.AddWithValue("$nombre", usuario.nombre);
        if (usuario.email != null) cmd.Parameters.AddWithValue("$email", usuario.email);
        if (usuario.telefono != null) cmd.Parameters.AddWithValue("$telefono", usuario.telefono);
        if (usuario.departamento != null) cmd.Parameters.AddWithValue("$departamento", usuario.departamento);
        if (usuario.localidad != null) cmd.Parameters.AddWithValue("$localidad", usuario.localidad);
        if (usuario.calle != null) cmd.Parameters.AddWithValue("$calle", usuario.calle);
        if (usuario.puerta != null) cmd.Parameters.AddWithValue("$puerta", usuario.puerta);
        if (usuario.is_active != null) cmd.Parameters.AddWithValue("$is_active", usuario.is_active);

        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }


    public static async Task<Errors> Delete(UsuarioVO usuario) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            UPDATE usuario SET is_active=0
            WHERE id = $id;
        """;

        cmd.Parameters.AddWithValue("$id", usuario.id);

        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }
    public static async Task<Errors> Activate(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            UPDATE usuario SET is_active=1
            WHERE id = $id;
        """;

        cmd.Parameters.AddWithValue("$id", id);

        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }

    public static async Task<Errors> DeleteGDPR(int id) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = """
            DELETE FROM usuario WHERE id = $id;
        """;
        cmd.Parameters.AddWithValue("id", id);
        var affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0 ? Errors.Ok : Errors.NotFound;
    }
}
