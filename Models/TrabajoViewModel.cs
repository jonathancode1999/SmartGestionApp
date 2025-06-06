using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGestionApp.Models
{
    public class TrabajoViewModel
    {
        public int Id { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string UsuarioNombre { get; set; } = "";
        public string EstadoNombre { get; set; } = "";
        public string TipoTrabajoNombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }

}
