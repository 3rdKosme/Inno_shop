using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Settings;
using Microsoft.Extensions.Options;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    IPasswordHasher passwordHasher, ICurrentUserService currentUserService, IJwtTokenService jwtTokenService, 
    IOptions<RefreshTokenSettings> refreshTokenSettings, IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<UpdateUserCommand, EmailChangeResultDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly RefreshTokenSettings _refreshTokenSettings = refreshTokenSettings.Value;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<EmailChangeResultDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if(!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(ErrorMessages.IncorrectPassword);
        }

        if(request.Name != null)
        {
            try
            {
                user.ChangeName(request.Name);
            }
            catch (DomainArgumentNullException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }

        }

        EmailChangeResultDto resultDto;

        if (request.Email != null) 
        { 
            if(! await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                try
                {
                    user.ChangeEmail(request.Email);
                }
                catch (DomainArgumentNullException ex) 
                {
                    throw new BusinessRuleValidationException(ex.Message);
                }

                var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                var token = new Inno_Shop.UserService.Domain.Entities.RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpireDays));

                await _refreshTokenRepository.AddAsync(token, cancellationToken);

                resultDto = new EmailChangeResultDto(accessToken, refreshToken);
            }
            else
            {
                throw new EmailAlreadyExistsException(ErrorMessages.EmailAlreadyExists);
            }
        }
        else
        {
            resultDto = new EmailChangeResultDto(null, null);
        }

        if (request.NewPassword != null)
        {
            try
            {
                user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            }
            catch (DomainArgumentNullException ex) 
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }
        
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendAsync(user.Email, EmailTemplate.ProfileChangedUser, new ProfileChangedModel { Name = user.Name }, cancellationToken);

        return resultDto;
    }
}