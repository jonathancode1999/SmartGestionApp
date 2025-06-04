using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;

namespace SmartGestionApp.Data.Repositories
{
    public class MaterialUsadoRepository
    {
        private readonly string _connectionString;

        public MaterialUsadoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<MaterialUsado> GetByTrabajoId(int trabajoId)
        {
            var materialesUsados = new List<MaterialUsado>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, TrabajoId, MaterialId, Cantidad, PrecioUnitario
                FROM MaterialesUsados
                WHERE TrabajoId = @trabajoId
            ";
            command.Parameters.AddWithValue("@trabajoId", trabajoId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var materialUsado = new MaterialUsado
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.GetInt32(1),
                    MaterialId = reader.GetInt32(2),
                    Cantidad = (double)reader.GetDecimal(3),     // casteo explícito decimal->double
                    PrecioUnitario = (double)reader.GetDecimal(4)
                };
                materialesUsados.Add(materialUsado);
            }

            return materialesUsados;
        }

        public void Insert(MaterialUsado materialUsado)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO MaterialesUsados (TrabajoId, MaterialId, Cantidad, PrecioUnitario)
                VALUES (@trabajoId, @materialId, @cantidad, @precioUnitario);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@trabajoId", materialUsado.TrabajoId);
            command.Parameters.AddWithValue("@materialId", materialUsado.MaterialId);
            command.Parameters.AddWithValue("@cantidad", materialUsado.Cantidad);
            command.Parameters.AddWithValue("@precioUnitario", materialUsado.PrecioUnitario);

            materialUsado.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(MaterialUsado materialUsado)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE MaterialesUsados
                SET TrabajoId = @trabajoId,
                    MaterialId = @materialId,
                    Cantidad = @cantidad,
                    PrecioUnitario = @precioUnitario
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@trabajoId", materialUsado.TrabajoId);
            command.Parameters.AddWithValue("@materialId", materialUsado.MaterialId);
            command.Parameters.AddWithValue("@cantidad", materialUsado.Cantidad);
            command.Parameters.AddWithValue("@precioUnitario", materialUsado.PrecioUnitario);
            command.Parameters.AddWithValue("@id", materialUsado.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM MaterialesUsados WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
