using System;

namespace SmartGestionApp.Models
{
    public class Adjunto
    {
        public int Id { get; set; }
        public int TrabajoId { get; set; }
        public string RutaArchivo { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
