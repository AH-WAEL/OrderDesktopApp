using AuthWebapi.Data;
using AuthWebapi.Entities;
using AuthWebapi.Migrations;
using AuthWebapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthWebapi.services
{
    public class Roles
    {
        public static string Admin => "Admin";
        public static string User => "User";

    }
    public class AuthService(userDbcontext context , IConfiguration configuration ) : IAuthService
    {
        public async Task<registerResponseDto?> RegisterAsync(userRegisterDTO request)
        {

            if (await context.users.AnyAsync(u=> u.username == request.username) || await context.users.AnyAsync(e => e.email == request.email))
            {
                return null;
            }

            var user = new users(); 

            var hashedpassword = new PasswordHasher<users>()
            .HashPassword(user, request.password);
            user.id = Guid.NewGuid();
            user.username = request.username;
            user.email = request.email;
            user.HashPassword = hashedpassword;
            user.Role = request.Role;

            context.users.Add(user);

            await context.SaveChangesAsync();
            registerResponseDto response = new registerResponseDto()
            {
                id=user.id,
                username = request.username,
                email = request.email,
                Role= request.Role,
            };

            return response;
        }

        public async Task<TokenResponseDto?> LoginAsync(loginDto request)
        {
            var user = await context.users.FirstOrDefaultAsync(u => u.username == request.username);
            if (user == null ||
                new PasswordHasher<users>().VerifyHashedPassword(user, user.HashPassword, request.password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            TokenResponseDto response = await createTokenRespnse(user);
            return response;

        }

        private async Task<TokenResponseDto> createTokenRespnse(users user)
        {
            return new TokenResponseDto
            {
                AccessToken = createtoken(user),
                RefreshToken = await GeneratAndSaveRefreshToken(user),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
        }

        private async Task<users?> ValidateRefreshToken (Guid userid , string reftreshToken)
        {
            var user = await context.users.FindAsync(userid);
            if (user is null || user.RefreshToken != reftreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user; 
        }
        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshToken(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            return await createTokenRespnse(user);
        }

        private string generateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GeneratAndSaveRefreshToken(users users)
        {
            var refreshtoken = generateRefreshToken();
            users.RefreshToken = refreshtoken;
            if (users.Role == Roles.User)
                users.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            else if (users.Role == Roles.Admin)
                users.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(120);
            await context.SaveChangesAsync();
            return refreshtoken;
        }

        private string createtoken(users users)
        {
            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Name , users.username),
                new Claim(ClaimTypes.NameIdentifier, users.id.ToString().ToString()),
                new Claim(ClaimTypes.Email, users.email),
                new Claim(ClaimTypes.Role, users.Role)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey"))!);
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokendescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(tokendescriptor);
        }

    }
}
