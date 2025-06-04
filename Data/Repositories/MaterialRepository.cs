using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;

namespace SmartGestionApp.Data.Repositories
{
    public class MaterialRepository
    {
        private readonly string _connectionString;

        public MaterialRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Material> GetAll()
        {
            var materiales = new List<Material>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, UnidadMedida, PrecioEstimado FROM Materiales";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var material = new Material
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    UnidadMedida = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PrecioEstimado = reader.GetDouble(3)
                };
                materiales.Add(material);
            }

            return materiales;
        }

        public Material? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, UnidadMedida, PrecioEstimado FROM Materiales WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Material
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    UnidadMedida = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PrecioEstimado = reader.GetDouble(3)
                };
            }

            return null;
        }

        public void Insert(Material material)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Materiales (Nombre, UnidadMedida, PrecioEstimado)
                VALUES (@nombre, @unidadMedida, @precioEstimado);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@nombre", material.Nombre);
            command.Parameters.AddWithValue("@unidadMedida", (object?)material.UnidadMedida ?? DBNull.Value);
            command.Parameters.AddWithValue("@precioEstimado", material.PrecioEstimado);

            material.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(Material material)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Materiales
                SET Nombre = @nombre,
                    UnidadMedida = @unidadMedida,
                    PrecioEstimado = @precioEstimado
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@nombre", material.Nombre);
            command.Parameters.AddWithValue("@unidadMedida", (object?)material.UnidadMedida ?? DBNull.Value);
            command.Parameters.AddWithValue("@precioEstimado", material.PrecioEstimado);
            command.Parameters.AddWithValue("@id", material.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Materiales WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
