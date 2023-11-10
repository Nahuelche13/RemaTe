using System.Collections.Generic;

using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class ClienteDA : Util<ClienteVO> {
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
                u.permisos
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
                permisos = reader.GetInt32(8)
            };
        }
    }
}
