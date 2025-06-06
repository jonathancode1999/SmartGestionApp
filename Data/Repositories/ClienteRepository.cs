using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;
using System;
using System.Collections.Generic;

namespace SmartGestionApp.Data.Repositories
{
    public class ClienteRepository
    {
        private readonly string _connectionString;
        private readonly ClienteTelefonoRepository _telefonoRepo;
        private readonly ClienteEmailRepository _emailRepo;
        private readonly DireccionRepository _direccionRepo;

        public ClienteRepository(string connectionString)
        {
            _connectionString = connectionString;
            _telefonoRepo = new ClienteTelefonoRepository(connectionString);
            _emailRepo = new ClienteEmailRepository(connectionString);
            _direccionRepo = new DireccionRepository(connectionString);
        }

        public Cliente? GetById(int id)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, CreatedAt FROM Clientes WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2),
                    Telefonos = _telefonoRepo.GetByClienteId(id),
                    Emails = _emailRepo.GetByClienteId(id),
                    Direccion = _direccionRepo.GetByClienteId(id)
                };
            }
            return null;
        }

        public List<Cliente> GetAll()
        {
            var clientes = new List<Cliente>();
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, CreatedAt FROM Clientes";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var cliente = new Cliente
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2),
                };

                cliente.Telefonos = _telefonoRepo.GetByClienteId(cliente.Id);
                cliente.Emails = _emailRepo.GetByClienteId(cliente.Id);
                cliente.Direccion = _direccionRepo.GetByClienteId(cliente.Id);

                clientes.Add(cliente);
            }

            return clientes;
        }


        public int Insert(Cliente cliente)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Clientes (Nombre, CreatedAt)
                VALUES (@nombre, @createdAt);
                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@createdAt", cliente.CreatedAt);

            int clienteId = (int)(long)cmd.ExecuteScalar();

            if (cliente.Direccion != null)
            {
                cliente.Direccion.ClienteId = clienteId;
                _direccionRepo.Insert(cliente.Direccion);
            }

            foreach (var tel in cliente.Telefonos ?? new List<ClienteTelefono>())
            {
                tel.ClienteId = clienteId;
                _telefonoRepo.Insert(tel);
            }

            foreach (var email in cliente.Emails ?? new List<ClienteEmail>())
            {
                email.ClienteId = clienteId;
                _emailRepo.Insert(email);
            }

            return clienteId;
        }

        public bool Update(Cliente cliente)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Clientes SET Nombre = @nombre WHERE Id = @id";
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@id", cliente.Id);
            var rows = cmd.ExecuteNonQuery();

            _direccionRepo.UpdateOrInsert(cliente.Direccion, cliente.Id);
            _telefonoRepo.ReplaceAllForCliente(cliente.Id, cliente.Telefonos);
            _emailRepo.ReplaceAllForCliente(cliente.Id, cliente.Emails);

            return rows > 0;
        }

        public bool Delete(int id)
        {
            _telefonoRepo.DeleteAllByClienteId(id);
            _emailRepo.DeleteAllByClienteId(id);
            _direccionRepo.DeleteByClienteId(id);

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
