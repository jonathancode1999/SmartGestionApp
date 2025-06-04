using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;

namespace SmartGestionApp.Data.Repositories
{
    public class PresupuestoRepository
    {
        private readonly string _connectionString;

        public PresupuestoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Presupuesto> GetAll()
        {
            var presupuestos = new List<Presupuesto>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, TrabajoId, UsuarioId, FechaCreacion, Total, Observaciones
                FROM Presupuestos
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var presupuesto = new Presupuesto
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.GetInt32(1),
                    UsuarioId = reader.GetInt32(2),
                    FechaCreacion = reader.GetDateTime(3),
                    Total = (double)reader.GetDecimal(4),   // casteo explícito decimal->double
                    Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
                presupuestos.Add(presupuesto);
            }

            return presupuestos;
        }

        public Presupuesto? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, TrabajoId, UsuarioId, FechaCreacion, Total, Observaciones
                FROM Presupuestos
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Presupuesto
                {
                    Id = reader.GetInt32(0),
                    TrabajoId = reader.GetInt32(1),
                    UsuarioId = reader.GetInt32(2),
                    FechaCreacion = reader.GetDateTime(3),
                    Total = (double)reader.GetDecimal(4),
                    Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
            }

            return null;
        }

        public void Insert(Presupuesto presupuesto)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Presupuestos (TrabajoId, UsuarioId, FechaCreacion, Total, Observaciones)
                VALUES (@trabajoId, @usuarioId, @fechaCreacion, @total, @observaciones);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@trabajoId", presupuesto.TrabajoId);
            command.Parameters.AddWithValue("@usuarioId", presupuesto.UsuarioId);
            command.Parameters.AddWithValue("@fechaCreacion", presupuesto.FechaCreacion);
            command.Parameters.AddWithValue("@total", presupuesto.Total);
            command.Parameters.AddWithValue("@observaciones", (object?)presupuesto.Observaciones ?? DBNull.Value);

            presupuesto.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(Presupuesto presupuesto)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Presupuestos
                SET TrabajoId = @trabajoId,
                    UsuarioId = @usuarioId,
                    FechaCreacion = @fechaCreacion,
                    Total = @total,
                    Observaciones = @observaciones
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@trabajoId", presupuesto.TrabajoId);
            command.Parameters.AddWithValue("@usuarioId", presupuesto.UsuarioId);
            command.Parameters.AddWithValue("@fechaCreacion", presupuesto.FechaCreacion);
            command.Parameters.AddWithValue("@total", presupuesto.Total);
            command.Parameters.AddWithValue("@observaciones", (object?)presupuesto.Observaciones ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", presupuesto.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Presupuestos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
