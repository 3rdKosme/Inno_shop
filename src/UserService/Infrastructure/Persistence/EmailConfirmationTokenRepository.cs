using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class EmailConfirmationTokenTokenRepository(AppDbContext context) : TokenRepository<EmailConfirmationToken>(context), IEmailConfirmationTokenRepository { }