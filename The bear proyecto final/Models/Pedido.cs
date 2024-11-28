using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int? IdMesa { get; set; }

    public int? IdMesero { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? Subtotal { get; set; }

    public bool? Pagado { get; set; }

    public virtual Mesa? IdMesaNavigation { get; set; }

    public virtual Chef? IdMeseroNavigation { get; set; }

    public virtual ICollection<PedidoPlatillo> PedidoPlatillos { get; set; } = new List<PedidoPlatillo>();
}
