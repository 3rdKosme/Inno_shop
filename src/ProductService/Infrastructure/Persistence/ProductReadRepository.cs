using Inno_Shop.ProductService.Application.Abstractions;
using Inno_Shop.ProductService.Domain.Entities;
using Inno_Shop.ProductService.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Inno_Shop.ProductService.Infrastructure.Persistence;

public class ProductReadRepository(IDbConnection dbConnection) : IProductReadRepository
{
    //private readonly AppDbContext _context = context;
    
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {

    }
}