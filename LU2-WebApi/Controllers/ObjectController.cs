using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LU2_WebApi.Data;
using LU2_WebApi.Models;
using System.Security.Claims;

namespace LU2_WebApi.Controllers
{
    [ApiController]
    [Route("environments/{environmentId}/objects")]
    [Authorize]
    public class ObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ObjectController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetByEnvironment(int environmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var environment = await _db.Environments
                .FirstOrDefaultAsync(e => e.Id == environmentId && e.UserId == userId);

            if (environment == null)
                return NotFound("Environment niet gevonden of geen toegang");

            var objects = await _db.Objects
                .Where(o => o.EnvironmentId == environmentId)
                .ToListAsync();

            return Ok(objects);
        }

        // POST: api/object
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Object2D obj)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var environment = await _db.Environments
                .FirstOrDefaultAsync(e => e.Id == obj.EnvironmentId && e.UserId == userId);

            if (environment == null)
                return BadRequest("Je hebt geen toegang tot deze environment");

            var newObj = new Object2D
            {
                Type = obj.Type,
                X = obj.X,
                Y = obj.Y,
                EnvironmentId = obj.EnvironmentId
            };

            _db.Objects.Add(newObj);
            await _db.SaveChangesAsync();

            return Ok(newObj);
        }

        // DELETE: api/object/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var obj = await _db.Objects
                .Include(o => o.Environment)
                .FirstOrDefaultAsync(o => o.Id == id && o.Environment!.UserId == userId);

            if (obj == null)
                return NotFound("Object niet gevonden of geen toegang");

            _db.Objects.Remove(obj);
            await _db.SaveChangesAsync();

            return Ok("Object verwijderd");
        }

        // PUT: api/object/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Object2D updated)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var obj = await _db.Objects
                .Include(o => o.Environment)
                .FirstOrDefaultAsync(o => o.Id == id && o.Environment!.UserId == userId);

            if (obj == null)
                return NotFound("Object niet gevonden of geen toegang");

            // Update de eigenschappen
            obj.Type = updated.Type;
            obj.X = updated.X;
            obj.Y = updated.Y;

            await _db.SaveChangesAsync();
            return Ok(obj);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> SaveAllObjects([FromBody] List<Object2D> objects)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Controleer of alle objecten in environments zitten die van deze user zijn
            var validEnvIds = await _db.Environments
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .ToListAsync();

            if (!objects.All(o => validEnvIds.Contains(o.EnvironmentId)))
                return Forbid("Een of meer objecten horen niet bij jouw omgevingen");

            _db.Objects.AddRange(objects);
            await _db.SaveChangesAsync();

            return Ok(objects);
        }
    }
}
