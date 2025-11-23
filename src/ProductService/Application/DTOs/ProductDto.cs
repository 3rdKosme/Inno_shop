namespace Inno_Shop.ProductService.Application.DTOs;

public record ProductDto(int Id, string Name, string Description, double Price, bool IsAvailable, int UserId, DateTime CreatedAt);