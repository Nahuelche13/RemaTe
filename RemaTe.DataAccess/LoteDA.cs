using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

using RemaTe.Common.Enums;
using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class LoteDA : Util<LoteVO> {
    public static new async Task<(Errors error, int id)> Create(LoteVO lote) {
        SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();
        var cmd = new SQLiteCommand("", DBC.I.DbConn, transaction);

        cmd.CommandText = "INSERT INTO lote (nombre, precio_base, comision) VALUES ($nombre, $precio_base, $comision);";
        cmd.Parameters.AddWithValue("$nombre", lote.nombre);
        cmd.Parameters.AddWithValue("$precio_base", lote.precio_base);
        cmd.Parameters.AddWithValue("$comision", lote.comision);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        cmd.CommandText = "SELECT id FROM lote WHERE rowid = $rowid";
        cmd.Parameters.AddWithValue("$rowid", DBC.I.DbConn.LastInsertRowId);
        var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();

        reader.Read();
        int id = reader.GetInt32(0);
        reader.Close();

        transaction.Commit();
        transaction.Dispose();

        return (Errors.Ok, id);
    }

    public static async IAsyncEnumerable<LoteRep> ReadAllWithArticulo() {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT l.id, l.nombre, l.precio_base, l.comision
            FROM lote l ORDER BY l.id";

        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new LoteRep() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                precio_base = reader.GetInt32(2),
                comision = reader.GetInt32(3),
                articulos = ReadArticulosOf(reader.GetInt32(0)).ToBlockingEnumerable()
            };
        }
    }

    public static async IAsyncEnumerable<ArticuloRep> ReadArticulosOf(int id) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT a.id, a.nombre, a.descripcion
            FROM lote l
            INNER JOIN articulo_lote al   ON l.id == al.id_lote
            INNER JOIN articulo a         ON a.id == al.id_articulo
            WHERE l.id = $id";

        cmd.Parameters.AddWithValue("id", id);

        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new ArticuloRep() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                descripcion = reader.GetString(2),
                imagenes = await ArticuloDA.ReadImagesOf(reader.GetInt32(0))
            };
        }
    }

    public static async Task<Errors> AddArticulos(int id, List<ArticuloVO> articulos) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var articulo in articulos) {
            cmd.CommandText = "INSERT INTO articulo_lote (id_articulo, id_lote) VALUES ($id_articulo, $id_lote);";
            cmd.Parameters.AddWithValue("$id_lote", id);
            cmd.Parameters.AddWithValue("$id_articulo", articulo.id);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }

        return Errors.Ok;
    }
}
