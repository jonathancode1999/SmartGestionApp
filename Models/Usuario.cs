using System;

namespace SmartGestionApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool EsAdministrador { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
