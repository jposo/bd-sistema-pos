using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class Mesa
{
    public int IdMesa { get; set; }

    public byte? Capacidad { get; set; }

    public string? Ubicacion { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
