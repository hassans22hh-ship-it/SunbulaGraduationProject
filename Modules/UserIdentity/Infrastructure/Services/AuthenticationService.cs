using Application.Options;
using Application.Services.Abstraction;
using Application.UserDTO;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.ValueOpjects;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace UserIdentityInfrastructure.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private const string EmailAlreadyRegisteredMessage = "Email is already registered";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly JwtOptions _jwtOptions;
        public AuthenticationService(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IOptions<JwtOptions> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jwtOptions = jwtOptions.Value;
        }
        #region GetUserProfile
        public async Task<UserDto> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new UserNotFoundException(userId);

            return _mapper.Map<UserDto>(user);
        }

        #endregion
        #region LoginService
        public async Task<AuthREsponseDto> LoginAsync(LoginDto loginDto,
         string? deviceInfo,
         CancellationToken cancellationToken)
        {
            var email = Email.Create(loginDto.Email);
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!user.IsActive)
                throw new UnauthorizedException("User account is deactivated");

            user.RecordLogin();
            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpiryDays());
            user.AddRefreshToken(refreshToken, refreshTokenExpiry, deviceInfo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new AuthREsponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                User = _mapper.Map<UserDto>(user)
            };

        }

        #endregion
        #region LogoutService
        public async Task LogoutAsync(Guid UserId,
          string? refreshToken,
          CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdWithRefreshTokensAsync(UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(UserId);

            }
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var tokenToRevoke = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                if (tokenToRevoke != null)
                    user.RevokeRefreshToken(tokenToRevoke.Id);
            }
            else
            {
                user.RevokeAllRefreshTokens();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

        #endregion
        #region RefreshToken
        public async Task<AuthREsponseDto> RefreshTokenAsync(string refreshToken,
        string? deviceInfo,
        CancellationToken cancellationToken)
        {

            var User = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (User == null)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Invalid refresh token");
            }
            var token = User.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token is null)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Invalid refresh token");
            }

            // Validate token
            token.Validate();

            if (!User.IsActive)
                throw new UnauthorizedException("User account is deactivated");

            // Revoke old token (token rotation)
            token.Revoke();

            // Generate new tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(User);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpiryDays());

            User.AddRefreshToken(newRefreshToken, refreshTokenExpiry, token.DeviceInfo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthREsponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                User = _mapper.Map<UserDto>(User)
            };


        }
        #endregion
        #region RegisterService
        public async Task<AuthREsponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            var email = Email.Create(registerDto.Email);
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken);
            if (emailExists)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException($"{nameof(registerDto.Email)} {EmailAlreadyRegisteredMessage}");

            }
            // Hash password (In production, store in separate UserPassword table)
            var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
            // Create user using domain factory method
            var User = Domain.Entities.User.Create(
                email,

                registerDto.FirstName,
                registerDto.LastName,
                  passwordHash,
                registerDto.Phone);
            // Add user
            await _unitOfWork.Users.AddAsync(User, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(User);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpiryDays());

            User.AddRefreshToken(refreshToken, refreshTokenExpiry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthREsponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                User = _mapper.Map<UserDto>(User)
            };
        }

        #endregion
        #region UpdateProfileSerivce
        public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto updateProfileDto, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new UserNotFoundException(userId);

            user.UpdateProfile(updateProfileDto.FirstName, updateProfileDto.LastName, updateProfileDto.Phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDto>(user);
        }
        #endregion
    }

}
