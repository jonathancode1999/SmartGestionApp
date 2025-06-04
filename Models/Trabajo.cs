using System;

namespace SmartGestionApp.Models
{
    public class Trabajo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int UsuarioId { get; set; }
        public int? EstadoId { get; set; }  // Nullable
        public int? TipoTrabajoId { get; set; }  // Nullable
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
