using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class ClienteDA : Util<ClienteVO> {
    public static new async Task<Errors> Create(ClienteVO cliente) {
        var cmd = DBC.I.DbConn.CreateCommand();

        cmd.CommandText = "INSERT INTO cliente (ci) VALUES ($ci);";
        cmd.Parameters.AddWithValue("$ci", cliente.ci);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        return Errors.Ok;
    }
    public static async IAsyncEnumerable<ClienteVO> ReadAllWithUsuario() {
        var command = DBC.I.DbConn.CreateCommand();
        command.CommandText = @"SELECT
                c.ci,
                u.nombre,
                u.email,
                u.telefono,
                u.departamento,
                u.localidad,
                u.calle,
                u.puerta,
                u.permisos,
                u.is_active
            FROM cliente c
            JOIN usuario u ON u.id == c.ci
        ";
        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) {
            yield return new ClienteVO() {
                ci = reader.GetInt32(0),
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                email = reader.GetString(2),
                telefono = reader.GetInt32(3),
                departamento = reader.GetInt32(4),
                localidad = reader.GetString(5),
                calle = reader.GetString(6),
                puerta = reader.GetInt32(7),
                permisos = reader.GetInt32(8),
                is_active = reader.GetBoolean(9),
            };
        }
    }
    public static async IAsyncEnumerable<ClienteVO> ReadAllWithUsuarioWhere(UsuarioVO filter) {
        var command = DBC.I.DbConn.CreateCommand();
        var sb = new StringBuilder();
        sb.AppendLine(@"SELECT
                c.ci,
                u.nombre,
                u.email,
                u.telefono,
                u.departamento,
                u.localidad,
                u.calle,
                u.puerta,
                u.permisos,
                u.is_active
            FROM cliente c
            JOIN usuario u ON u.id == c.ci
        ");

        sb.AppendLine("WHERE ");
        if (filter.id != null) sb.Append("ci=$ci AND ");
        if (filter.nombre != null) sb.Append("nombre LIKE '%$nombre%' AND ");
        if (filter.email != null) sb.Append("email=$email AND ");
        if (filter.telefono != null) sb.Append("telefono=$telefono AND ");
        if (filter.departamento != null) sb.Append("departamento=$departamento AND ");
        if (filter.localidad != null) sb.Append("localidad=$localidad AND ");
        if (filter.calle != null) sb.Append("calle=$calle AND ");
        if (filter.puerta != null) sb.Append("puerta=$puerta AND ");
        if (filter.permisos != null) sb.Append("permisos=$permisos AND ");
        if (filter.is_active != null) sb.Append("is_active=$is_active AND ");

        if (filter.id != null) command.Parameters.AddWithValue("$ci", filter.id);
        if (filter.nombre != null) command.Parameters.AddWithValue("$nombre", filter.nombre);
        if (filter.email != null) command.Parameters.AddWithValue("$email", filter.email);
        if (filter.telefono != null) command.Parameters.AddWithValue("$telefono", filter.telefono);
        if (filter.departamento != null) command.Parameters.AddWithValue("$departamento", filter.departamento);
        if (filter.localidad != null) command.Parameters.AddWithValue("$localidad", filter.localidad);
        if (filter.calle != null) command.Parameters.AddWithValue("$calle", filter.calle);
        if (filter.puerta != null) command.Parameters.AddWithValue("$puerta", filter.puerta);
        if (filter.permisos != null) command.Parameters.AddWithValue("$permisos", filter.permisos);
        if (filter.is_active != null) command.Parameters.AddWithValue("$is_active", filter.is_active);

        sb.Remove(sb.Length - 5, 5);
        command.CommandText = sb.ToString();
        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) {
            yield return new ClienteVO() {
                ci = reader.GetInt32(0),
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                email = reader.GetString(2),
                telefono = reader.GetInt32(3),
                departamento = reader.GetInt32(4),
                localidad = reader.GetString(5),
                calle = reader.GetString(6),
                puerta = reader.GetInt32(7),
                permisos = reader.GetInt32(8),
                is_active = reader.GetBoolean(9),
            };
        }
    }
}
