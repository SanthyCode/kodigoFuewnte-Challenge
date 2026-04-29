using Kodigo.Domain.Entities;

namespace Kodigo.Application.Interfaces;

public interface ISeparataRepository
{
    // 1. Traer todas las separatas
    Task<List<Separata>> GetAllAsync();
    
    // 2. Traer un producto por su ID (¡Este es el que te está pidiendo!)
    Task<Product?> GetProductByIdAsync(Guid productId);
    
    // 3. Traer los items solapados en fechas
    Task<List<SeparataItem>> GetOverlappingItemsAsync(DateTime startDate, DateTime endDate);
}