namespace The_bear_proyecto_final.Models
{
    public class NewOrderRequest
    {
        public int IdMesa { get; set; }
        public int IdMesero { get; set; }
    }


    public class AddPlatillosRequest
    {
        public int IdPedido { get; set; }
        public List<PlatilloRequest> Platillos { get; set; }
    }

    public class PlatilloRequest
    {
        public int IdPlatillo { get; set; }
        public int Cantidad { get; set; }
        public string Especificaciones { get; set; }
    }

}
