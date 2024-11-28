using System;
using System.Collections.Generic;

namespace The_bear_proyecto_final.Models;

public partial class VentasMeseroMesa
{
    public string? Mesero { get; set; }

    public string? UbicacionMesa { get; set; }

    public decimal? Subtotal { get; set; }

    public bool? Pagado { get; set; }

    public int? TotalPlatillos { get; set; }
}
