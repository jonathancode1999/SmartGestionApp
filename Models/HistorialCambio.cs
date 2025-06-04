using System;

namespace SmartGestionApp.Models
{
    public class HistorialCambio
    {
        public int Id { get; set; }
        public int? TrabajoId { get; set; }
        public int? UsuarioId { get; set; }
        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
