using Kodigo.Domain.Entities;

namespace Kodigo.Application.Interfaces;

public interface ISeparataRepository
{
    // Solo definimos qué necesitamos, no cómo se hace
    Task<List<Separata>> GetAllAsync();
    Task<Product?> GetProductByIdAsync(Guid productId);
}