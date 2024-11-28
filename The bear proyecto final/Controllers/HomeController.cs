using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using The_bear_proyecto_final.Models;

namespace The_bear_proyecto_final.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly RestauranteContext _context;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(RestauranteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("Api/GetDishes")]
        public IActionResult GetDishes()
        {
            var platillos = _context.Platillos.ToList();
            var dataList = new List<object>();
            foreach (var item in platillos)
            {
                dataList.Add(item);
            }

            return Json(dataList);
        }

        [HttpGet]
        [Route("Api/GetActiveOrders")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var query = await _context.Pedidos
                .Where(p => (bool)!p.Pagado) // Filter where pagado = 0
                //.GroupJoin(
                //    _context.PedidoPlatillos,
                //    p => p.IdPedido,
                //    pp => pp.IdPedido,
                //    (p, pedidoPlatillos) => new
                //    {
                //        IdPedido = p.IdPedido,
                //        IdMesa = p.IdMesa,
                //        TiempoPrimerOrden = pedidoPlatillos.Min(pp => pp.Fecha)
                //    })
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return Json(query);
        }

        [HttpGet]
        [Route("Api/GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            // Verificar si el pedido existe y recuperar detalles junto con el nombre del mesero
            var query = await _context.Pedidos
                .Where(p => p.IdPedido == orderId) // Filtrar por ID de pedido
                .Join(
                    _context.Chefs,                  // Tabla con la que se hace el join
                    p => p.IdMesero,                 // Clave foránea en la tabla pedido
                    c => c.IdChef,                   // Clave primaria en la tabla chef
                    (p, c) => new                    // Resultado del join
                    {
                        Pedido = p,
                        MeseroNombre = c.Nombre,
                        MeseroApellido = c.Apellido,
                    }
                )
                .Select(result => new
                {
                    IdPedido = result.Pedido.IdPedido,
                    IdMesa = result.Pedido.IdMesa,
                    Fecha = result.Pedido.Fecha,
                    Subtotal = result.Pedido.Subtotal,
                    Pagado = result.Pedido.Pagado,
                    Mesero = $"{result.MeseroNombre} {result.MeseroApellido}",
                    Platillos = _context.PedidoPlatillos
                        .Where(pp => pp.IdPedido == result.Pedido.IdPedido)
                        .Select(pp => new
                        {
                            pp.IdPlatillo,
                            pp.CantidadPedido,
                            pp.Especificaciones
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (query == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            return Json(query);
        }

        [HttpPost]
        [Route("Api/CreateNewOrder")]
        public async Task<IActionResult> CreateNewOrder([FromBody] NewOrderRequest request)
        {
            // Validar que la mesa y el mesero existen
            var mesa = await _context.Mesas.FindAsync(request.IdMesa);
            if (mesa == null)
            {
                return NotFound(new { message = "Mesa no encontrada" });
            }

            var mesero = await _context.Chefs.FindAsync(request.IdMesero);
            if (mesero == null)
            {
                return BadRequest(new { message = "Mesero no válido" });
            }

            // Crear el pedido
            var nuevoPedido = new Pedido
            {
                IdMesa = request.IdMesa,
                IdMesero = request.IdMesero,
                Fecha = DateTime.Now,
                Subtotal = 0, // Subtotal inicial
                Pagado = false
            };
            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync(); // Guardar para generar el ID del pedido

            return Ok(new { message = "Pedido creado exitosamente", IdPedido = nuevoPedido.IdPedido });
        }

        [HttpPost]
        [Route("Api/SendToKitchen")]
        public async Task<IActionResult> SendToKitchen([FromBody] AddPlatillosRequest request)
        {
            Console.WriteLine("Request Received!");
            Debug.WriteLine("Request Received!");
            // Validar que el pedido existe
            var pedido = await _context.Pedidos.FindAsync(request.IdPedido);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            // Validar y agregar los platillos
            var platillos = new List<PedidoPlatillo>();
            foreach (var item in request.Platillos)
            {
                var platillo = await _context.Platillos.FindAsync(item.IdPlatillo);
                if (platillo == null)
                {
                    return BadRequest(new { message = $"Platillo con ID {item.IdPlatillo} no encontrado" });
                }

                platillos.Add(new PedidoPlatillo
                {
                    IdPedido = request.IdPedido,
                    IdPlatillo = item.IdPlatillo,
                    CantidadPedido = (byte)item.Cantidad,
                    Especificaciones = item.Especificaciones,
                    Estado = "pending",
                });
            }

            // Guardar los platillos en la base de datos
            _context.PedidoPlatillos.AddRange(platillos);

            // Calcular el nuevo subtotal del pedido
            pedido.Subtotal += platillos.Sum(p => p.CantidadPedido * _context.Platillos
                .First(pl => pl.IdPlatillo == p.IdPlatillo).Precio);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Platillos enviados a la cocina exitosamente" });
        }

    }
}
