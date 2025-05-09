using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LU2_WebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LU2_WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User userObject)
    {
        var user = new ApplicationUser { UserName = userObject.email };
        var result = await _userManager.CreateAsync(user, userObject.password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Registration successful" });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User userObject)
    {
        var user = await _userManager.FindByNameAsync(userObject.email);
        if (user == null) return Unauthorized("Gebruiker onbekend");

        var result = await _signInManager.CheckPasswordSignInAsync(user, userObject.password, false);
        if (!result.Succeeded) return Unauthorized("Wachtwoord onjuist");

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id), // <- deze is leidend
            new Claim(ClaimTypes.Name, user.UserName!)     // <- als je de naam wil meesturen
        };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
