using LU2_WebApi.Data;
using LU2_WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LU2_WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> Create([FromBody] string name)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Console.WriteLine("📢 userId uit token: " + userId);

        var exists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!exists)
        {
            Console.WriteLine("❌ UserId bestaat NIET in AspNetUsers tabel!");
            return NotFound("UserId bestaat niet in database");
        }

        var env = new Environment2D
        {
            Name = name,
            UserId = userId ?? throw new ArgumentNullException(nameof(userId), "UserId cannot be null")
        };

        _db.Environments.Add(env);

        try
        {
            await _db.SaveChangesAsync();
            return Ok(env);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ EX bij save: " + ex.Message);
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
