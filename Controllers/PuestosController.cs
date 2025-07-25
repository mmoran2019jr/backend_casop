using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using promerica_backend.Data;
using promerica_backend.Models;

namespace promerica_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PuestosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/puestos (usa SP ObtenerJerarquia)
        [HttpGet]
        public async Task<IActionResult> GetJerarquia()
        {
            var puestos = await _context.Puestos
                .FromSqlRaw("EXEC ObtenerJerarquia")
                .ToListAsync();

            var jerarquia = ConstruirJerarquia(null, puestos);
            return Ok(jerarquia);
        }

        private List<object> ConstruirJerarquia(int? jefeCodigo, List<Puestos> lista)
        {
            return lista
                .Where(p => p.CodigoJefe == jefeCodigo)
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Puesto,
                    p.Nombre,
                    p.CodigoJefe,
                    Subordinados = ConstruirJerarquia(p.Codigo, lista)
                }).ToList<object>();
        }

        // GET: api/puestos/5 (usa SP ObtenerPuestoPorId)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var param = new SqlParameter("@Id", id);
            var resultado = await _context.Puestos
                .FromSqlRaw("EXEC ObtenerPuestoPorId @Id", param)
                .ToListAsync();

            var puesto = resultado.FirstOrDefault();
            return puesto == null ? NotFound() : Ok(puesto);
        }

        // POST: api/puestos (usa SP InsertarPuesto)
        [HttpPost]
        public async Task<IActionResult> Create(Puestos model)
        {
            var parametros = new[]
            {
            new SqlParameter("@Codigo", model.Codigo),
            new SqlParameter("@Puesto", model.Puesto),
            new SqlParameter("@Nombre", model.Nombre),
            new SqlParameter("@CodigoJefe", model.CodigoJefe ?? (object)DBNull.Value)
        };

            await _context.Database.ExecuteSqlRawAsync("EXEC InsertarPuesto @Codigo, @Puesto, @Nombre, @CodigoJefe", parametros);

            return Ok(new { message = "Puesto insertado correctamente" });
        }

        // PUT: api/puestos/5 (usa SP ActualizarPuesto)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Puestos model)
        {
            if (id != model.Id)
                return BadRequest();

            var parametros = new[]
            {
            new SqlParameter("@Id", id),
            new SqlParameter("@Codigo", model.Codigo),
            new SqlParameter("@Puesto", model.Puesto),
            new SqlParameter("@Nombre", model.Nombre),
            new SqlParameter("@CodigoJefe", model.CodigoJefe ?? (object)DBNull.Value)
        };

            await _context.Database.ExecuteSqlRawAsync("EXEC ActualizarPuesto @Id, @Codigo, @Puesto, @Nombre, @CodigoJefe", parametros);
            return Ok(new { message = "Puesto actualizado correctamente" });
        }

        // DELETE: api/puestos/5 (usa SP EliminarPuesto)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parametro = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC EliminarPuesto @Id", parametro);
            return Ok(new { message = "Puesto eliminado correctamente" });
        }
    }
}
