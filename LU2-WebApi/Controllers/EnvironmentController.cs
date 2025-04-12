using LU2_WebApi.Data;
using LU2_WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LU2_WebApi.Controllers;

[Route("environments")]
[ApiController]
[Authorize]
public class EnvironmentController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EnvironmentController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyEnvironments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var environments = await _db.Environments
            .Where(e => e.UserId == userId)
            .ToListAsync();

        return Ok(environments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Environment2D environment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("📢 userId uit token: " + userId);

        if (string.IsNullOrWhiteSpace(environment.Name) || environment.Name.Length > 25)
            return BadRequest("Naam moet tussen 1 en 25 karakters zijn.");

        if (environment.MaxX < 20 || environment.MaxX > 200)
            return BadRequest("Lengte moet tussen 20 en 200 zijn.");

        if (environment.MaxY < 10 || environment.MaxY > 100)
            return BadRequest("Hoogte moet tussen 10 en 100 zijn.");

        var exists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!exists)
        {
            Console.WriteLine("UserId bestaat NIET in AspNetUsers tabel!");
            return NotFound("UserId bestaat niet in database");
        }

        var existingCount = await _db.Environments.CountAsync(e => e.UserId == userId);
        if (existingCount >= 5)
            return BadRequest("Je mag maximaal 5 omgevingen aanmaken.");

        var nameExists = await _db.Environments.AnyAsync(e => e.UserId == userId && e.Name == environment.Name);
        if (nameExists)
            return BadRequest("Je hebt al een omgeving met deze naam.");

        environment.UserId = userId ?? throw new ArgumentNullException(nameof(userId), "UserId cannot be null");

        _db.Environments.Add(environment);

        try
        {
            await _db.SaveChangesAsync();
            return Ok(environment);
        }
        catch (Exception ex)
        {
            Console.WriteLine("EX bij save: " + ex.Message);
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var env = await _db.Environments.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (env == null) return NotFound();

        _db.Environments.Remove(env);
        await _db.SaveChangesAsync();

        return Ok("Verwijderd");
    }
}
