﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using redis.WebAPi.Model.UserModels;
using redis.WebAPi.Repository.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Login functionality
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    // Registration functionality
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        // Check if the username or email already exists
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
        {
            return BadRequest("Username already exists.");
        }

        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            return BadRequest("Email is already in use.");
        }

        // Password validation: Ensure password is at least 8 characters long and contains both letters and numbers
        if (!IsValidPassword(model.Password))
        {
            return BadRequest("Password must be at least 8 characters long and contain both letters and numbers.");
        }

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // Create a new user
        var user = new User
        {
            Username = model.Username,
            PasswordHash = hashedPassword,
            Email = model.Email,
            Role = model.Role ?? "user",  // Default role is "user" if not provided
        };

        // Add the user to the database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Return success message
        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
    {
        var userId = GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized("User not authenticated.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.TargetUsername);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // If the user is an admin, they can change any user's password
        if (User.IsInRole("admin"))
        {
            // Allow admin to change the target user's password
        }
        else
        {
            // If the user is a regular user, they can only change their own password
            if (userId != user.Id)
            {
                return Unauthorized("You can only change your own password.");
            }
        }

        // Verify the old password
        if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
        {
            return BadRequest("Old password is incorrect.");
        }

        // Validate the complexity of the new password
        if (!IsValidPassword(model.NewPassword))
        {
            return BadRequest("New password must be at least 8 characters long and contain both letters and numbers.");
        }

        // Hash the new password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        // Update the password
        user.PasswordHash = hashedPassword;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Password changed successfully." });
    }

    // Password validation method
    private bool IsValidPassword(string password)
    {
        // Password must be at least 8 characters long and contain both letters and numbers
        var passwordRegex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$");
        return passwordRegex.IsMatch(password);
    }

    // Generate JWT Token
    private string GenerateJwtToken(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Get the current logged-in user's ID
    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        return int.TryParse(userIdClaim, out var userId) ? userId : (int?)null;
    }
}
