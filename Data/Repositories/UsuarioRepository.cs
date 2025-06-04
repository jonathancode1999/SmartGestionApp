using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Usuario? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Email, PasswordHash, EsAdministrador, Activo, CreatedAt FROM Usuarios WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PasswordHash = reader.IsDBNull(3) ? null : reader.GetString(3),
                    EsAdministrador = reader.GetInt32(4) == 1,
                    Activo = reader.GetInt32(5) == 1,
                    CreatedAt = reader.GetDateTime(6)
                };
            }
            return null;
        }

        public List<Usuario> GetAll()
        {
            var list = new List<Usuario>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Email, PasswordHash, EsAdministrador, Activo, CreatedAt FROM Usuarios";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PasswordHash = reader.IsDBNull(3) ? null : reader.GetString(3),
                    EsAdministrador = reader.GetInt32(4) == 1,
                    Activo = reader.GetInt32(5) == 1,
                    CreatedAt = reader.GetDateTime(6)
                });
            }
            return list;
        }

        public int Insert(Usuario usuario)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Usuarios (Nombre, Email, PasswordHash, EsAdministrador, Activo, CreatedAt)
                VALUES (@nombre, @email, @passwordHash, @esAdmin, @activo, @createdAt);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@email", (object?)usuario.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@passwordHash", (object?)usuario.PasswordHash ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@esAdmin", usuario.EsAdministrador ? 1 : 0);
            cmd.Parameters.AddWithValue("@activo", usuario.Activo ? 1 : 0);
            cmd.Parameters.AddWithValue("@createdAt", usuario.CreatedAt);

            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public bool Update(Usuario usuario)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Usuarios SET
                    Nombre = @nombre,
                    Email = @email,
                    PasswordHash = @passwordHash,
                    EsAdministrador = @esAdmin,
                    Activo = @activo
                WHERE Id = @id;
            ";
            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@email", (object?)usuario.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@passwordHash", (object?)usuario.PasswordHash ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@esAdmin", usuario.EsAdministrador ? 1 : 0);
            cmd.Parameters.AddWithValue("@activo", usuario.Activo ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", usuario.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Usuarios WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
