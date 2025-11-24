using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public class AddUserCommandHandler(IUserRepository userRepository, IEmailService emailService, IPasswordHasher passwordHasher, 
    IJwtTokenService jwtTokenService, ITokenRepository<Domain.Entities.RefreshToken> refreshTokenRepository) : IRequestHandler<AddUserCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(AddUserCommand request, CancellationToken cancellationToken = default)
    {
        if(await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new EmailAlreadyExistsException(ErrorMessages.EmailAlreadyExists);
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);

        User user;
        try
        {
            user = User.Create(request.Name, request.Email, passwordHash);
        }
        catch (DomainArgumentNullException ex) 
        { 
            throw new BusinessRuleValidationException(ex.Message);
        }
        

        await userRepository.AddAsync(user, cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserRole.ToString());
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        var token = new Domain.Entities.RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        await refreshTokenRepository.AddAsync(token, cancellationToken);

        await emailService.SendAsync(user.Email, EmailTemplate.ProfileCreated, new ProfileCreatedModel { Name = user.Name }, cancellationToken);

        return new AuthResultDto(accessToken, refreshToken);
    }
}