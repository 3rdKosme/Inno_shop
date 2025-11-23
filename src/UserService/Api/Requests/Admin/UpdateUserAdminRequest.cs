using Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

namespace Inno_Shop.UserService.Api.Requests.Admin;

public record UpdateUserAdminRequest(string Name)
{
    public UpdateUserAdminCommand ToCommand(int id) => 
        new UpdateUserAdminCommand(id, Name);
}