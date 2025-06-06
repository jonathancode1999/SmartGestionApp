using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class ClienteTelefonoRepository
    {
        private readonly string _connectionString;

        public ClienteTelefonoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ClienteTelefono> GetByClienteId(int clienteId)
        {
            var list = new List<ClienteTelefono>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, ClienteId, Telefono, EsPrincipal FROM ClienteTelefonos WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ClienteTelefono
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Numero = reader.GetString(2),
                    Tipo = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return list;
        }

        public void Insert(ClienteTelefono tel)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ClienteTelefonos (ClienteId, Telefono, EsPrincipal)
                VALUES (@clienteId, @numero, @tipo)";
            cmd.Parameters.AddWithValue("@clienteId", tel.ClienteId);
            cmd.Parameters.AddWithValue("@numero", tel.Numero);
            cmd.Parameters.AddWithValue("@tipo", (object?)tel.Tipo ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void ReplaceAllForCliente(int clienteId, List<ClienteTelefono>? nuevos)
        {
            DeleteAllByClienteId(clienteId);
            foreach (var tel in nuevos ?? new List<ClienteTelefono>())
            {
                tel.ClienteId = clienteId;
                Insert(tel);
            }
        }

        public void DeleteAllByClienteId(int clienteId)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM ClienteTelefonos WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);
            cmd.ExecuteNonQuery();
        }
    }
}
