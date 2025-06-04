using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class AdjuntoRepository
    {
        private readonly string _connectionString;

        public AdjuntoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Adjunto? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, TrabajoId, RutaArchivo, Tipo, Descripcion, Fecha 
                FROM Adjuntos 
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Adjunto
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.GetInt32(1),
                    RutaArchivo = reader.GetString(2),
                    Tipo = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Fecha = reader.GetDateTime(5)
                };
            }
            return null;
        }

        public List<Adjunto> GetAll()
        {
            var list = new List<Adjunto>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, TrabajoId, RutaArchivo, Tipo, Descripcion, Fecha 
                FROM Adjuntos";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Adjunto
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.GetInt32(1),
                    RutaArchivo = reader.GetString(2),
                    Tipo = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Fecha = reader.GetDateTime(5)
                });
            }
            return list;
        }

        public int Insert(Adjunto adjunto)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Adjuntos (TrabajoId, RutaArchivo, Tipo, Descripcion, Fecha)
                VALUES (@trabajoId, @rutaArchivo, @tipo, @descripcion, @fecha);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@trabajoId", adjunto.TrabajoId);
            cmd.Parameters.AddWithValue("@rutaArchivo", adjunto.RutaArchivo);
            cmd.Parameters.AddWithValue("@tipo", (object?)adjunto.Tipo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object?)adjunto.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", adjunto.Fecha);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Update(Adjunto adjunto)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Adjuntos SET
                    TrabajoId = @trabajoId,
                    RutaArchivo = @rutaArchivo,
                    Tipo = @tipo,
                    Descripcion = @descripcion,
                    Fecha = @fecha
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@trabajoId", adjunto.TrabajoId);
            cmd.Parameters.AddWithValue("@rutaArchivo", adjunto.RutaArchivo);
            cmd.Parameters.AddWithValue("@tipo", (object?)adjunto.Tipo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object?)adjunto.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", adjunto.Fecha);
            cmd.Parameters.AddWithValue("@id", adjunto.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Adjuntos WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
