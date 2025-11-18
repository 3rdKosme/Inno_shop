using MediatR;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

public class UpdateUserAdminCommandHandler(IUserRepository userRepository, IEmailService emailService)
    : IRequestHandler<UpdateUserAdminCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    public async Task<Unit> Handle(UpdateUserAdminCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if(request.Name != null)
        {
            user.ChangeName(request.Name);
            try
            {
                user.ChangeName(request.Name);
            }
            catch (DomainArgumentNullException ex)
            {
                throw new BusinessRuleValidationException(ex.Message);
            }
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendAsync(user.Email, EmailTemplate.ProfileChangedAdmin, new ProfileChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}