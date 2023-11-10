using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

using RemaTe.Common.Enums;

using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class RemateDA : Util<RemateVO> {
    public static new async Task<(Errors error, int id)> Create(RemateVO remate) {
        SQLiteTransaction transaction = DBC.I.DbConn.BeginTransaction();
        var cmd = new SQLiteCommand("", DBC.I.DbConn, transaction);

        cmd.CommandText = "INSERT INTO remate (nombre, rematador, inicio, duracion, tipo, metodos_pago) VALUES ($nombre, $rematador, $inicio, $duracion, $tipo, $metodos_pago);";
        cmd.Parameters.AddWithValue("$nombre", remate.nombre);
        cmd.Parameters.AddWithValue("$rematador", remate.rematador);
        cmd.Parameters.AddWithValue("$inicio", remate.inicio);
        cmd.Parameters.AddWithValue("$duracion", remate.duracion);
        cmd.Parameters.AddWithValue("$tipo", remate.tipo);
        cmd.Parameters.AddWithValue("$metodos_pago", remate.metodos_pago);
        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();

        cmd.CommandText = "SELECT id FROM remate WHERE rowid = $rowid";
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
    /*
    public static async Task<RemateVO> Read(int id) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT r.id, r.inicio, r.duracion, r.tipo, r.metodos_pago
            FROM remate r
            ORDER BY r.duracion";

        var reader = await cmd.ExecuteReaderAsync();
        return new RemateVO() {
            id = reader.GetInt32(0),
            inicio = reader.GetDateTime(1),
            duracion = reader.GetInt32(2),
            tipo = reader.GetInt32(3),
            metodos_pago = reader.GetInt32(4)
        };
    }
    */
    public static async IAsyncEnumerable<LoteRep> ReadLotesOf(int id) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT l.id, l.nombre, l.precio_base, l.comision
            FROM integra i
            JOIN lote l           ON l.id == i.id_lote
            WHERE i.id_remate == $id_remate";

        cmd.Parameters.AddWithValue("id_remate", id);
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new LoteRep() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                precio_base = reader.GetInt32(2),
                comision = reader.GetInt32(3),
                articulos = LoteDA.ReadArticulosOf(reader.GetInt32(0)).ToBlockingEnumerable()
            };
        }
    }

    public static async IAsyncEnumerable<RemateRep> ReadAllFutureWithLote() {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT r.id, r.nombre, r.rematador, r.inicio, r.duracion, r.tipo, r.metodos_pago
            FROM remate r
            WHERE r.inicio >= date('now', 'localtime')
            ORDER BY r.inicio";

        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new RemateRep() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                rematador = reader.GetString(2),
                inicio = reader.GetDateTime(3),
                duracion = reader.GetInt32(4),
                tipo = reader.GetInt32(5),
                metodos_pago = reader.GetInt32(6),
                lotes = RemateDA.ReadLotesOf(reader.GetInt32(0)).ToBlockingEnumerable()
            };
        }
    }
    public static async IAsyncEnumerable<RemateRep> ReadAllPastWithLote() {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT r.id, r.nombre, r.rematador, r.inicio, r.duracion, r.tipo, r.metodos_pago
            FROM remate r
            WHERE r.inicio < date('now', 'localtime')
            ORDER BY r.inicio";

        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read()) {
            yield return new RemateRep() {
                id = reader.GetInt32(0),
                nombre = reader.GetString(1),
                rematador = reader.GetString(2),
                inicio = reader.GetDateTime(3),
                duracion = reader.GetInt32(4),
                tipo = reader.GetInt32(5),
                metodos_pago = reader.GetInt32(6),
                lotes = RemateDA.ReadLotesOf(reader.GetInt32(0)).ToBlockingEnumerable()
            };
        }
    }

    public static async Task<Errors> AddLotes(int id, List<LoteVO> lotes) {
        SQLiteCommand cmd = DBC.I.DbConn.CreateCommand();

        foreach (var lote in lotes) {
            cmd.CommandText = "INSERT INTO integra (id_lote, id_remate) VALUES ($id_lote, $id_remate);";
            cmd.Parameters.AddWithValue("$id_remate", id);
            cmd.Parameters.AddWithValue("$id_lote", lote.id);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
        }
        return Errors.Ok;
    }
}