using System;

namespace SmartGestionApp.Models
{
    public class Presupuesto
    {
        public int Id { get; set; }
        public int TrabajoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public double Total { get; set; }
        public string? Observaciones { get; set; }
    }
}
