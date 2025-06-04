namespace SmartGestionApp.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? UnidadMedida { get; set; }
        public double PrecioEstimado { get; set; } = 0;
    }
}
