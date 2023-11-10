using System.Data.SQLite;
using System.Threading.Tasks;

using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class UsuarioDA : Util<UsuarioVO> {
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
}
