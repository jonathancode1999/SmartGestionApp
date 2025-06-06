using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGestionApp.Models
{
    public class Direccion
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Calle { get; set; }
        public string CodigoPostal { get; set; }
    }

}
