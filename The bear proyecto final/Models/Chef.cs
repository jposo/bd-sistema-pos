using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class Chef
{
    public int IdChef { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Puesto { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public decimal? Sueldo { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
