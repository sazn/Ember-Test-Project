using Ember.Data;
using Ember.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ember.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly MovieContext _context;

        public RegistrationsController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
        //{
        //    var result = await _context.Registrations.ToListAsync();
        //    return result;
        //}

        // Assuming this is in your controller
        public async Task<ActionResult<object>> GetRegistrations()
        {
            var registrations = await _context.Registrations.ToListAsync();

            var result = new
            {
                data = registrations.Select(r => new
                {
                    id = r.Id.ToString(), // Ensure this is a string
                    type = "registration",
                    attributes = new
                    {
                        name = r.Name,
                        email = r.Email,
                        accountType = r.AccountType,
                        createdAt = r.CreatedAt.ToString("o") // ISO 8601 format
                    }
                })
            };

            return Ok(result);
        }

        // GET: api/registrations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Registration>> GetRegistration(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);

            if (registration == null)
            {
                return NotFound(); // Return 404 if the registration is not found
            }

            return registration; // Return the registration object
        }


        [HttpPost]
        public async Task<ActionResult<Registration>> PostRegistration(Registration registration)
        {
            registration.CreatedAt = DateTime.UtcNow;
            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRegistrations), new { id = registration.Id }, registration);
        }

        // DELETE: api/registrations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content after a successful delete
        }

        // PUT: api/registrations/{id} (Update registration)
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRegistration(int id, [FromBody] Registration updatedRegistration)
        {
            if (id != updatedRegistration.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistrationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Respond with 204 No Content on success
        }

        // Helper method to check if a registration exists
        private bool RegistrationExists(int id)
        {
            return _context.Registrations.Any(e => e.Id == id);
        }
    }
}