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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Object2D updated)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var obj = await _db.Objects
                .Include(o => o.Environment)
                .FirstOrDefaultAsync(o => o.Id == id && o.Environment!.UserId == userId);

            if (obj == null)
                return NotFound("Object niet gevonden of geen toegang");

            obj.Type = updated.Type;
            obj.X = updated.X;
            obj.Y = updated.Y;

            await _db.SaveChangesAsync();
            return Ok(obj);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll(int environmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var environment = await _db.Environments
                .FirstOrDefaultAsync(e => e.Id == environmentId && e.UserId == userId);

            if (environment == null)
                return NotFound("Environment niet gevonden of geen toegang");

            var objects = await _db.Objects
                .Where(o => o.EnvironmentId == environmentId)
                .ToListAsync();

            if (!objects.Any())
                return NotFound("Geen objecten gevonden in deze environment");

            _db.Objects.RemoveRange(objects);
            await _db.SaveChangesAsync();

            return Ok("Alle objecten zijn verwijderd");
        }
    }
}
