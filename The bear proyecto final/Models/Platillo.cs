using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class Platillo
{
    public int IdPlatillo { get; set; }

    public string? Nombre { get; set; }

    public string? Tipo { get; set; }

    public string? Descripcion { get; set; }

    public short? Gramos { get; set; }

    public short? Calorias { get; set; }

    public decimal? Precio { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public virtual ICollection<PedidoPlatillo> PedidoPlatillos { get; set; } = new List<PedidoPlatillo>();
}
