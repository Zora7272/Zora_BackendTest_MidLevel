using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class myofficeacpdController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly WebApplication1.Services.IAcpdService _service;

        public myofficeacpdController(AppDbContext db, WebApplication1.Services.IAcpdService service)
        {
            _db = db;
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MyOfficeAcpd myOfficeAcpd)
        {
            var (created, logJson) = await _service.CreateAsync(myOfficeAcpd);
            return CreatedAtAction(nameof(Get), new { id = created.ACPD_SID }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MyOfficeAcpd myOfficeAcpd)
        {
            try
            {
                await _service.UpdateAsync(id, myOfficeAcpd);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
    }
}
