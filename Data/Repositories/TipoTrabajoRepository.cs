using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class TipoTrabajoRepository
    {
        private readonly string _connectionString;

        public TipoTrabajoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TipoTrabajo? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre FROM TiposTrabajo WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new TipoTrabajo
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                };
            }
            return null;
        }

        public List<TipoTrabajo> GetAll()
        {
            var list = new List<TipoTrabajo>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre FROM TiposTrabajo";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TipoTrabajo
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                });
            }
            return list;
        }

        public int Insert(TipoTrabajo tipoTrabajo)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO TiposTrabajo (Nombre)
                VALUES (@nombre);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("@nombre", tipoTrabajo.Nombre);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Update(TipoTrabajo tipoTrabajo)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE TiposTrabajo SET
                    Nombre = @nombre
                WHERE Id = @id;
            ";
            cmd.Parameters.AddWithValue("@nombre", tipoTrabajo.Nombre);
            cmd.Parameters.AddWithValue("@id", tipoTrabajo.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM TiposTrabajo WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
