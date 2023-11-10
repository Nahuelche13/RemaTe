using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

using RemaTe.Common.Models;

namespace RemaTe.DataAccess;

public class RemateDA : Util<RemateVO> {
    public static async Task<RemateVO> Read(int id) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT r.id, r.inicio, r.duracion, r.tipo, r.metodos_pago
            FROM remate r
            ORDER BY r.duracion";

        var reader = await cmd.ExecuteReaderAsync();
        return new RemateVO() {
            id = reader.GetInt32(0),
            inicio = reader.GetInt32(1),
            duracion = reader.GetInt32(2),
            tipo = reader.GetInt32(3),
            metodos_pago = reader.GetInt32(4)
        };
    }
    public static async Task<List<(int fecha, LoteVO lotes)>> ReadLotesOf(int id) {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT *
            FROM lote_remate lr
            JOIN lote l           ON l.id == lr.id_lote
            WHERE lr.id_remate == $id_remate";

        cmd.Parameters.AddWithValue("id_remate", id);
        var reader = await cmd.ExecuteReaderAsync();

        var lote = new List<(int fecha, LoteVO lotes)>() { };
        while (reader.Read()) {
            lote.Add((
                fecha: reader.GetInt32(0),
                lotes: new LoteVO() {
                    id = reader.GetInt32(1),
                    precio_base = reader.GetInt32(2),
                    comision = reader.GetInt32(3)
                }
            ));
        }

        return lote;
    }
    public static async IAsyncEnumerable<RemateRep> ReadAllWithLote() {
        using SQLiteCommand cmd = new(DBC.I.DbConn);
        cmd.CommandText = @"
            SELECT r.id, r.inicio, r.duracion, r.tipo, r.metodos_pago, l.id, l.nombre, l.precio_base, l.comision
            FROM remate r
            JOIN lote_remate lr ON r.id == lr.id_remate
            JOIN lote l           ON l.id == lr.id_lote
            ORDER BY r.duracion";

        var reader = await cmd.ExecuteReaderAsync();

        int? id = null;
        RemateRep? remate = null;

        while (reader.Read()) {
            if (id != reader.GetInt32(0)) {
                if (remate != null) {
                    yield return remate;
                }
                id = reader.GetInt32(0);
                remate = new RemateRep() {
                    id = reader.GetInt32(0),
                    inicio = reader.GetInt32(1),
                    duracion = reader.GetInt32(2),
                    tipo = reader.GetInt32(3),
                    metodos_pago = reader.GetInt32(4),
                    lotes = new List<LoteRep>()
                };
            }
            remate?.lotes.Append(
                    new LoteRep() {
                        id = reader.GetInt32(5),
                        nombre = reader.GetString(6),
                        precio_base = reader.GetInt32(7),
                        comision = reader.GetInt32(8),
                        articulos = LoteDA.ReadArticulosOf(reader.GetInt32(5)).ToBlockingEnumerable()
                    }
                );
        }
        if (remate != null) {
            yield return remate;
        }
    }
}
