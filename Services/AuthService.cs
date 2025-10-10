using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FormEngineAPI.Models;
using FormEngineAPI.DTOs;
using FormEngineAPI.Repositories;
using AutoMapper;

namespace FormEngineAPI.Services;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    string GenerateJwtToken(User user);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IActivityLogRepository activityLogRepository,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _activityLogRepository = activityLogRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new Exception("Email já está em uso.");
        }

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = registerDto.Role
        };

        await _userRepository.AddAsync(user);

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = user.Id,
            Action = "CREATE",
            Entity = "User",
            EntityId = user.Id,
            Details = $"Novo usuário registrado: {user.Email}"
        });

        var token = GenerateJwtToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new Exception("Email ou senha inválidos.");
        }

        // Log activity
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            UserId = user.Id,
            Action = "LOGIN",
            Entity = "User",
            EntityId = user.Id,
            Details = $"Usuário {user.Email} fez login"
        });

        var token = GenerateJwtToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT SecretKey não configurada");
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(
                int.Parse(jwtSettings["ExpirationHours"] ?? "24")),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
