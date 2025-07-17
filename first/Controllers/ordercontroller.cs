using first.Data;
using first.module;
using first.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace first.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly firstcontext _context;
        private readonly producer _producer;
        private static int nextId = 1;

        public OrderController(firstcontext context,producer producer)
        {
            _context = context;
            _producer = producer;
        }

        [HttpGet]
        public async Task<ActionResult<order>> GetAll()
        {
          var orders= await _context.orders.ToListAsync();
            return Ok(orders);       
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<order>> Get(int id)
        {
            var order = await _context.orders.FindAsync(id); // Corrected method to FindAsync
            if (order == null) return NotFound();
            var orderJson = JsonSerializer.Serialize(order);
            await _producer.send($"{orderJson}");
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<order>> Create([FromBody] order newOrder)
        {
            _context.orders.Add(newOrder);

            await _context.SaveChangesAsync();
            var orderJson = JsonSerializer.Serialize(newOrder);
            await _producer.send($"{orderJson}");
            return Ok(newOrder);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, order updatedOrder)
        {
            var existing = await _context.orders.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Id = id; // Ensure the ID is set correctly
            existing.Item = updatedOrder.Item;
            existing.Quantity = updatedOrder.Quantity;
            existing.OrderDate = updatedOrder.OrderDate;
            existing.TotalPrice = updatedOrder.TotalPrice;
            await _context.SaveChangesAsync();
            var orderJson = JsonSerializer.Serialize(existing);
            await _producer.send($"{orderJson}");
            return Ok(existing);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var order = await _context.orders.FindAsync(id);
            if (order == null) return NotFound();
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();
            var orderJson = JsonSerializer.Serialize(order);
            await _producer.send($"{orderJson}");
            return NoContent();
        } 
        
    }
}
