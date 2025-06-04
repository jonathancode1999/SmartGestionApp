namespace SmartGestionApp.Models
{
    public class MaterialUsado
    {
        public int Id { get; set; }
        public int TrabajoId { get; set; }
        public int MaterialId { get; set; }
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
    }
}
