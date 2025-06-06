using System;

namespace SmartGestionApp.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ClienteTelefono> Telefonos { get; set; }
        public List<ClienteEmail> Emails { get; set; }
        public Direccion? Direccion { get; set; }

        public string TelefonosTexto => string.Join(", ", Telefonos.Select(t => $"{t.Numero} ({t.Tipo})"));
        public string EmailsTexto => string.Join(", ", Emails.Select(e => $"{e.Email} ({e.Observacion})"));
        public string DireccionTexto => Direccion != null
            ? $"{Direccion.Calle}, {Direccion.Ciudad}, {Direccion.Provincia}, {Direccion.Pais} ({Direccion.CodigoPostal})"
            : "Sin dirección";

    }

}
