using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Inno_Shop.UserService.Infrastructure.Persistence;

public class PasswordResetTokenRepository(AppDbContext context) : TokenRepository<PasswordResetToken>(context), IPasswordResetTokenRepository { }