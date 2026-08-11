using AuthWebapi.Data;
using AuthWebapi.Entities;
using AuthWebapi.Models;
using AuthWebapi.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IAuthService authService, userDbcontext context) : ControllerBase
    {

        private readonly userDbcontext _context = context;

        public static users user= new();
         
        [HttpPost("register")]
        public async Task<ActionResult<registerResponseDto>> Register(userRegisterDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("username already exist");
            }

            return Ok(user);
        }

        [HttpPost("login")]

        public async Task<ActionResult<TokenResponseDto?>> Login(loginDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return Unauthorized("invalid username or password");
            }
            return Ok(result);
        }

        [HttpPost("validate-refresh-token")]
        public async Task<ActionResult<RefreshTokenValidationDto>> ValidateRefreshToken(RefreshTokenRequestDto request)
        {
            try
            {
                var user = await _context.users.FindAsync(request.UserId);

                if (user is null)
                {
                    return Ok(new RefreshTokenValidationDto
                    {
                        IsValid = false,
                        Message = "User not found",
                        RefreshTokenExpiryTime = DateTime.MinValue,
                        DatabaseTime = DateTime.UtcNow //

                    });
                }

                bool isValid = user.RefreshToken == request.RefreshToken &&
                              user.RefreshTokenExpiryTime > DateTime.UtcNow;

                return Ok(new RefreshTokenValidationDto
                {
                    IsValid = isValid,
                    Message = isValid ? "Refresh token is valid" :
                             user.RefreshTokenExpiryTime <= DateTime.UtcNow ? "Refresh token expired" : "Invalid refresh token",
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                    DatabaseTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Ok(new RefreshTokenValidationDto
                {
                    IsValid = false,
                    Message = $"Validation error: {ex.Message}",
                    RefreshTokenExpiryTime = DateTime.MinValue
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

        [Authorize]
        [HttpGet]

        public IActionResult authonticatedonly()
        {
            return Ok("you are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are and admin!");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
        {
            try
            {
                var user = await _context.users.FindAsync(request.UserId);
                
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow;
                    
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"User {user.username} logged out - Refresh token invalidated");
                    
                    return Ok(new { Message = "Logged out successfully", Success = true });
                }
                
                return Ok(new { Message = "User not found", Success = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                return BadRequest(new { Message = "Logout failed", Error = ex.Message, Success = false });
            }
        }

    }
}
