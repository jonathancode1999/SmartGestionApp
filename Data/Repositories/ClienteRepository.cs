using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class ClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Cliente? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Telefono, Direccion, Email, CreatedAt FROM Clientes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Telefono = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Direccion = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }
            return null;
        }

        public List<Cliente> GetAll()
        {
            var list = new List<Cliente>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Telefono, Direccion, Email, CreatedAt FROM Clientes";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Cliente
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Telefono = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Direccion = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }
            return list;
        }

        public int Insert(Cliente cliente)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Clientes (Nombre, Telefono, Direccion, Email, CreatedAt)
                VALUES (@nombre, @telefono, @direccion, @email, @createdAt);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)cliente.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)cliente.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)cliente.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", cliente.CreatedAt);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Update(Cliente cliente)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Clientes SET
                    Nombre = @nombre,
                    Telefono = @telefono,
                    Direccion = @direccion,
                    Email = @email
                WHERE Id = @id;
            ";
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)cliente.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)cliente.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)cliente.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", cliente.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Clientes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
