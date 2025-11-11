namespace Inno_Shop.UserService.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool isEmailConfirmed { get; set; }
    public bool isActive { get; set; }
}