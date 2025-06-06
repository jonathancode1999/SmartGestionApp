using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class ClienteEmailRepository
    {
        private readonly string _connectionString;

        public ClienteEmailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ClienteEmail> GetByClienteId(int clienteId)
        {
            var list = new List<ClienteEmail>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, ClienteId, Email, EsPrincipal FROM ClienteEmails WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ClienteEmail
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Email = reader.GetString(2),
                    Observacion = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return list;
        }

        public void Insert(ClienteEmail email)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ClienteEmails (ClienteId, Email, EsPrincipal)
                VALUES (@clienteId, @email, @obs)";
            cmd.Parameters.AddWithValue("@clienteId", email.ClienteId);
            cmd.Parameters.AddWithValue("@email", email.Email);
            cmd.Parameters.AddWithValue("@obs", (object?)email.Observacion ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void ReplaceAllForCliente(int clienteId, List<ClienteEmail>? nuevos)
        {
            DeleteAllByClienteId(clienteId);
            foreach (var email in nuevos ?? new List<ClienteEmail>())
            {
                email.ClienteId = clienteId;
                Insert(email);
            }
        }

        public void DeleteAllByClienteId(int clienteId)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM ClienteEmails WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);
            cmd.ExecuteNonQuery();
        }
    }
}
