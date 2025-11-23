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
    public async Task<EmailChangeResultDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var user = await userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if(!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
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
            if(! await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                try
                {
                    user.ChangeEmail(request.Email);
                }
                catch (DomainArgumentNullException ex) 
                {
                    throw new BusinessRuleValidationException(ex.Message);
                }

                var accessToken = jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
                var refreshToken = jwtTokenService.GenerateRefreshToken();

                var token = new Inno_Shop.UserService.Domain.Entities.RefreshToken(user.Id, 
                    refreshToken, DateTime.UtcNow.AddDays(refreshTokenSettings.Value.ExpireDays));

                await refreshTokenRepository.AddAsync(token, cancellationToken);

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
                user.ChangePassword(passwordHasher.HashPassword(request.NewPassword));
            }
            catch (DomainArgumentNullException ex) 
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }
        
        await userRepository.UpdateAsync(user, cancellationToken);

        await emailService.SendAsync(user.Email, EmailTemplate.ProfileChangedUser, new ProfileChangedModel { Name = user.Name }, cancellationToken);

        return resultDto;
    }
}