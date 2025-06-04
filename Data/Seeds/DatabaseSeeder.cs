using Microsoft.Data.Sqlite;
using System;

namespace SmartGestionApp.Data.Seeds
{
    public static class DatabaseSeeder
    {
        public static void Seed()
        {
            using var connection = DatabaseManager.GetConnection();
            connection.Open();

            // Se crea una transacción para que todo sea atómico
            using var transaction = connection.BeginTransaction();

            // Insertar EstadosTrabajo si no existen
            var insertEstadosCmd = connection.CreateCommand();
            insertEstadosCmd.Transaction = transaction;
            insertEstadosCmd.CommandText = @"
                INSERT INTO EstadosTrabajo (Nombre)
                SELECT 'Pendiente' WHERE NOT EXISTS (SELECT 1 FROM EstadosTrabajo WHERE Nombre = 'Pendiente');
                INSERT INTO EstadosTrabajo (Nombre)
                SELECT 'En proceso' WHERE NOT EXISTS (SELECT 1 FROM EstadosTrabajo WHERE Nombre = 'En proceso');
                INSERT INTO EstadosTrabajo (Nombre)
                SELECT 'Completado' WHERE NOT EXISTS (SELECT 1 FROM EstadosTrabajo WHERE Nombre = 'Completado');
                INSERT INTO EstadosTrabajo (Nombre)
                SELECT 'Cancelado' WHERE NOT EXISTS (SELECT 1 FROM EstadosTrabajo WHERE Nombre = 'Cancelado');
            ";
            insertEstadosCmd.ExecuteNonQuery();

            // Insertar TiposTrabajo si no existen
            var insertTiposCmd = connection.CreateCommand();
            insertTiposCmd.Transaction = transaction;
            insertTiposCmd.CommandText = @"
                INSERT INTO TiposTrabajo (Nombre)
                SELECT 'Instalación' WHERE NOT EXISTS (SELECT 1 FROM TiposTrabajo WHERE Nombre = 'Instalación');
                INSERT INTO TiposTrabajo (Nombre)
                SELECT 'Reparación' WHERE NOT EXISTS (SELECT 1 FROM TiposTrabajo WHERE Nombre = 'Reparación');
                INSERT INTO TiposTrabajo (Nombre)
                SELECT 'Mantenimiento' WHERE NOT EXISTS (SELECT 1 FROM TiposTrabajo WHERE Nombre = 'Mantenimiento');
                INSERT INTO TiposTrabajo (Nombre)
                SELECT 'Revisión técnica' WHERE NOT EXISTS (SELECT 1 FROM TiposTrabajo WHERE Nombre = 'Revisión técnica');
            ";
            insertTiposCmd.ExecuteNonQuery();

            // Insertar usuario admin si no existe (revisando por email)
            var insertAdminCmd = connection.CreateCommand();
            insertAdminCmd.Transaction = transaction;
            insertAdminCmd.CommandText = @"
                INSERT INTO Usuarios (Nombre, Email, PasswordHash, EsAdministrador, Activo)
                SELECT 'Admin', 'admin@smartgestion.com', 'admin123', 1, 1
                WHERE NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@smartgestion.com');
            ";
            insertAdminCmd.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
        }
    }
}
