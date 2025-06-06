using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class TrabajoRepository
    {
        private readonly string _connectionString;

        public TrabajoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Trabajo? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, ClienteId, UsuarioId, EstadoId, TipoTrabajoId, Descripcion, Fecha, CreatedAt
                FROM Trabajos
                WHERE Id = @id
            ";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Trabajo
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    UsuarioId = reader.GetInt32(2),
                    EstadoId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    TipoTrabajoId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6),
                    CreatedAt = reader.GetDateTime(7)
                };
            }
            return null;
        }
        public List<Trabajo> GetByClientId(int clientId)
        {
            var list = new List<Trabajo>();

            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
		SELECT Id, ClienteId, UsuarioId, EstadoId, TipoTrabajoId, Descripcion, Fecha, CreatedAt
		FROM Trabajos
		WHERE ClienteId = @clientId
	";
            cmd.Parameters.AddWithValue("@clientId", clientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Trabajo
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    UsuarioId = reader.GetInt32(2),
                    EstadoId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    TipoTrabajoId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6),
                    CreatedAt = reader.GetDateTime(7)
                });
            }
            return list;
        }

        public List<Trabajo> GetAll()
        {
            var list = new List<Trabajo>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, ClienteId, UsuarioId, EstadoId, TipoTrabajoId, Descripcion, Fecha, CreatedAt
                FROM Trabajos
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Trabajo
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    UsuarioId = reader.GetInt32(2),
                    EstadoId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    TipoTrabajoId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6),
                    CreatedAt = reader.GetDateTime(7)
                });
            }
            return list;
        }

        public int Insert(Trabajo trabajo)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Trabajos (ClienteId, UsuarioId, EstadoId, TipoTrabajoId, Descripcion, Fecha, CreatedAt)
                VALUES (@clienteId, @usuarioId, @estadoId, @tipoTrabajoId, @descripcion, @fecha, @createdAt);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("@clienteId", trabajo.ClienteId);
            cmd.Parameters.AddWithValue("@usuarioId", trabajo.UsuarioId);
            cmd.Parameters.AddWithValue("@estadoId", trabajo.EstadoId.HasValue ? (object)trabajo.EstadoId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@tipoTrabajoId", trabajo.TipoTrabajoId.HasValue ? (object)trabajo.TipoTrabajoId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object?)trabajo.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", trabajo.Fecha);
            cmd.Parameters.AddWithValue("@createdAt", trabajo.CreatedAt);

            var id = (long)cmd.ExecuteScalar()!;
            return (int)id;
        }

        public bool Update(Trabajo trabajo)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Trabajos SET
                    ClienteId = @clienteId,
                    UsuarioId = @usuarioId,
                    EstadoId = @estadoId,
                    TipoTrabajoId = @tipoTrabajoId,
                    Descripcion = @descripcion,
                    Fecha = @fecha
                WHERE Id = @id;
            ";
            cmd.Parameters.AddWithValue("@clienteId", trabajo.ClienteId);
            cmd.Parameters.AddWithValue("@usuarioId", trabajo.UsuarioId);
            cmd.Parameters.AddWithValue("@estadoId", trabajo.EstadoId.HasValue ? (object)trabajo.EstadoId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@tipoTrabajoId", trabajo.TipoTrabajoId.HasValue ? (object)trabajo.TipoTrabajoId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object?)trabajo.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", trabajo.Fecha);
            cmd.Parameters.AddWithValue("@id", trabajo.Id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Trabajos WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public List<TrabajoViewModel> GetAllViewModel()
        {
            var list = new List<TrabajoViewModel>();

            using var con = new SqliteConnection(_connectionString);
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    t.Id,
                    c.Nombre AS ClienteNombre,
                    u.Nombre AS UsuarioNombre,
                    COALESCE(e.Nombre, '') AS EstadoNombre,
                    COALESCE(tt.Nombre, '') AS TipoTrabajoNombre,
                    t.Descripcion,
                    t.Fecha
                FROM Trabajos t
                INNER JOIN Clientes c ON t.ClienteId = c.Id
                INNER JOIN Usuarios u ON t.UsuarioId = u.Id
                LEFT JOIN EstadosTrabajo e ON t.EstadoId = e.Id
                LEFT JOIN TiposTrabajo tt ON t.TipoTrabajoId = tt.Id
                ORDER BY t.Fecha DESC;
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TrabajoViewModel
                {
                    Id = reader.GetInt32(0),
                    ClienteNombre = reader.GetString(1),
                    UsuarioNombre = reader.GetString(2),
                    EstadoNombre = reader.GetString(3),
                    TipoTrabajoNombre = reader.GetString(4),
                    Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Fecha = reader.GetDateTime(6)
                });
            }

            return list;
        }
    }
}
