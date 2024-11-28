using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class PedidoPlatillo
{
    public int IdPedido { get; set; }

    public int IdPlatillo { get; set; }

    public byte? CantidadPedido { get; set; }

    public string? Especificaciones { get; set; }

    public string? Estado { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Platillo IdPlatilloNavigation { get; set; } = null!;
}
