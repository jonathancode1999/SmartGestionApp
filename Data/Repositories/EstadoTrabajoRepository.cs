using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class EstadoTrabajoRepository
    {
        private readonly string _connectionString;

        public EstadoTrabajoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EstadoTrabajo? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre FROM EstadosTrabajo WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new EstadoTrabajo
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                };
            }
            return null;
        }

        public List<EstadoTrabajo> GetAll()
        {
            var list = new List<EstadoTrabajo>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre FROM EstadosTrabajo";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new EstadoTrabajo
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                });
            }
            return list;
        }

        public int Insert(EstadoTrabajo estado)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO EstadosTrabajo (Nombre)
                VALUES (@nombre);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@nombre", estado.Nombre);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Update(EstadoTrabajo estado)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE EstadosTrabajo SET Nombre = @nombre WHERE Id = @id";
            cmd.Parameters.AddWithValue("@nombre", estado.Nombre);
            cmd.Parameters.AddWithValue("@id", estado.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM EstadosTrabajo WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
