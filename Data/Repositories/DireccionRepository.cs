using Microsoft.Data.Sqlite;
using SmartGestionApp.Models;

namespace SmartGestionApp.Data.Repositories
{
    public class DireccionRepository
    {
        private readonly string _connectionString;

        public DireccionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Direccion? GetByClienteId(int clienteId)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, ClienteId, Pais, Provincia, Ciudad, Direccion, CodigoPostal
                FROM ClienteDirecciones WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Direccion
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Pais = reader.GetString(2),
                    Provincia = reader.GetString(3),
                    Ciudad = reader.GetString(4),
                    Calle = reader.GetString(5),
                    CodigoPostal = reader.GetString(6)
                };
            }
            return null;
        }

        public void Insert(Direccion direccion)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ClienteDirecciones (ClienteId, Pais, Provincia, Ciudad, Direccion, CodigoPostal)
                VALUES (@clienteId, @pais, @provincia, @ciudad, @calle, @codigoPostal)";
            cmd.Parameters.AddWithValue("@clienteId", direccion.ClienteId);
            cmd.Parameters.AddWithValue("@pais", direccion.Pais);
            cmd.Parameters.AddWithValue("@provincia", direccion.Provincia);
            cmd.Parameters.AddWithValue("@ciudad", direccion.Ciudad);
            cmd.Parameters.AddWithValue("@calle", direccion.Calle);
            cmd.Parameters.AddWithValue("@codigoPostal", direccion.CodigoPostal);
            cmd.ExecuteNonQuery();
        }

        public void UpdateOrInsert(Direccion? direccion, int clienteId)
        {
            DeleteByClienteId(clienteId);
            if (direccion != null)
            {
                direccion.ClienteId = clienteId;
                Insert(direccion);
            }
        }

        public void DeleteByClienteId(int clienteId)
        {
            using var con = new SqliteConnection(_connectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM ClienteDirecciones WHERE ClienteId = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId);
            cmd.ExecuteNonQuery();
        }
    }
}
