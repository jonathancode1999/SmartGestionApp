using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class HistorialCambioRepository
    {
        private readonly string _connectionString;

        public HistorialCambioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public HistorialCambio? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, TrabajoId, UsuarioId, CampoModificado, ValorAnterior, ValorNuevo, Fecha
                FROM HistorialCambios
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new HistorialCambio
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    UsuarioId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    CampoModificado = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ValorAnterior = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ValorNuevo = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6)
                };
            }
            return null;
        }

        public List<HistorialCambio> GetAll()
        {
            var list = new List<HistorialCambio>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, TrabajoId, UsuarioId, CampoModificado, ValorAnterior, ValorNuevo, Fecha
                FROM HistorialCambios";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new HistorialCambio
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    UsuarioId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    CampoModificado = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ValorAnterior = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ValorNuevo = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6)
                });
            }
            return list;
        }

        public int Insert(HistorialCambio historial)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO HistorialCambios (TrabajoId, UsuarioId, CampoModificado, ValorAnterior, ValorNuevo, Fecha)
                VALUES (@trabajoId, @usuarioId, @campoModificado, @valorAnterior, @valorNuevo, @fecha);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@trabajoId", (object?)historial.TrabajoId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@usuarioId", (object?)historial.UsuarioId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@campoModificado", (object?)historial.CampoModificado ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@valorAnterior", (object?)historial.ValorAnterior ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@valorNuevo", (object?)historial.ValorNuevo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", historial.Fecha);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM HistorialCambios WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}

