using BeautyPoint.Configurations;
using BeautyPoint.Dtos;
using BeautyPoint.Models;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BeautyPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly JwtSettings _jwtSettings;


        public AuthController(UserManager<User> userManager, IConfiguration configuration, UserService userService, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _configuration = configuration;
            _userService = userService;
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return Unauthorized("Invalid username or password.");
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            var userInfo = new UserInfoDto
            {
                UserId = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsLoggedIn = true
            };

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                MyAuthInfo = userInfo
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                Console.WriteLine("No refresh token provided.");
                return BadRequest("Refresh token is required.");
            }
            
            var principal = ValidateJwtToken(request.RefreshToken, validateLifetime: false);
            if (principal == null)
            {
                Console.WriteLine("Failed to validate refresh token. Token might be malformed or invalid.");
                return Unauthorized("Invalid refresh token.");
            }

            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Refresh token validation succeeded, but UserId claim is missing.");
                return Unauthorized("Invalid refresh token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"No user found with ID {userId}. Possible invalid refresh token.");
                return Unauthorized("User not found.");
            }

            Console.WriteLine($"Successfully validated refresh token for user {user.UserName} (ID: {userId}).");
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(userId);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        private string GenerateJwtToken(User user)
        {
            if (string.IsNullOrEmpty(user.Role.ToString()))
            {
                throw new InvalidOperationException("User role is not defined.");
            }

            var secretKey = _jwtSettings.SecretKey;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Audience)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpirationInMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(string userId)
        {
            var secretKey = _jwtSettings.SecretKey;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), 
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            
            Console.WriteLine($"Generated Refresh Token: {tokenString}");
            Console.WriteLine($"User ID used for refresh token: {userId}");

            return tokenString;
        }

        private ClaimsPrincipal? ValidateJwtToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _jwtSettings.SecretKey;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = validateLifetime,
                    IssuerSigningKey = key,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null)
                {
                    Console.WriteLine("Invalid token format.");
                    return null;
                }

                var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("Refresh token missing 'sub' claim.");
                    return null;
                }

                var jtiClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim))
                {
                    Console.WriteLine("Refresh token missing 'jti' claim.");
                    return null;
                }

                var iatClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value;
                if (string.IsNullOrEmpty(iatClaim))
                {
                    Console.WriteLine("Refresh token missing 'iat' claim.");
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token validation failed due to expiration: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }


    }
}