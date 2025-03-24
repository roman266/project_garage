using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using project_garage.Interfaces.IService;
using System.Security.Cryptography;
using project_garage.Models.DbModels;
using project_garage.Interfaces.IRepository;
using project_garage.Models.ViewModels;
using project_garage.Models.JWTSettings;


namespace project_garage.Service
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JWTSettings _jwtSettings;

        public TokenService(IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor, JWTSettings jwtSettings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthDto> StartSessionAsync(string userId, string email)
        {
            var authDto = new AuthDto
            {
                AccessToken = GenerateAccessToken(userId, email),
                RefreshToken = await CreateAndStoreRefreshTokenAsync(userId, email),
                AccessCookieOptions = GetCookieOptions(TimeSpan.FromMinutes(_jwtSettings.TokenValidityInMinutes)),
                RefreshCookieOptions = GetCookieOptions(TimeSpan.FromDays(_jwtSettings.RefreshTokenValidityInDays)),
            };

            return authDto;
        }

        public CookieOptions GetCookieOptions(TimeSpan expiresIn) 
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(expiresIn),
            };

            return cookieOptions;
        }

        public string GetJwtFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available");

            if (httpContext.Request.Cookies.TryGetValue("RefreshToken", out var jwtToken))
            {
                return jwtToken;
            }

            throw new UnauthorizedAccessException("JWT token not found in cookies");
        }

        public string GenerateAccessToken(string userId, string email)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<string> CreateAndStoreRefreshTokenAsync(string userId, string email)
        {
            string refreshToken = GenerateRefreshToken(userId, email);

            var refreshTokenEntity = new RefreshTokenModel
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityInDays),
                IsRevoked = false,
            };

            await _refreshTokenRepository.AddTokenAsync(refreshTokenEntity);

            return refreshToken;
        }

        public string GenerateRefreshToken(string userId, string email)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token, userId);
            return refreshToken != null && refreshToken.ExpiryDate > DateTime.UtcNow && !refreshToken.IsRevoked;
        }

        public async Task<AuthDto> RenewAccessTokenAsync()
        {
            var refreshToken = GetJwtFromCookie();
            var payload = DecodeJwtToken(refreshToken);

            if (!await ValidateRefreshTokenAsync(refreshToken, payload.UserId))
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            await RevokeRefreshTokenAsync(refreshToken);

            var authDto = await StartSessionAsync(payload.UserId, payload.Email);

            return authDto;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _refreshTokenRepository.UpdateTokenAsync(refreshToken);
            }
        }

        public DecodedTokenDto DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new ArgumentException("Incorrect JWT");
            
            var jwtToken = handler.ReadJwtToken(token);

            string userId = jwtToken.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            string email = jwtToken.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            var payload = new DecodedTokenDto
            {
                UserId = userId,
                Email = email,
            };

            return payload;
        }
    }
}
