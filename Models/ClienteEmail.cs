using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGestionApp.Models
{
    public class ClienteEmail
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Email { get; set; }
        public string? Observacion { get; set; } // ej: "personal", "trabajo"
    }

}
