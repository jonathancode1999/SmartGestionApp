using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGestionApp.Models
{
    public class ClienteTelefono
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Numero { get; set; }
        public string? Tipo { get; set; } // "Móvil", "Fijo", etc.
    }

}
