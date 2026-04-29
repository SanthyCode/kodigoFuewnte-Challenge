using Microsoft.EntityFrameworkCore;
using Kodigo.Domain.Entities;
using Kodigo.Application.Interfaces;

namespace Kodigo.Infrastructure.Repositories;

public class SeparataRepository : ISeparataRepository
{
    private readonly ApplicationDbContext _context;

    // Aquí sí podemos usar el DbContext porque estamos en Infrastructure
    public SeparataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Separata>> GetAllAsync()
    {
        return await _context.Separatas.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    // Añade este método dentro de la clase SeparataRepository
    public async Task<List<SeparataItem>> GetOverlappingItemsAsync(DateTime startDate, DateTime endDate)
    {
        // 1. Buscamos los IDs de las campañas que chocan en fechas
        var overlappingSeparataIds = await _context.Separatas
            .Where(s => startDate < s.EndDate && endDate > s.StartDate)
            .Select(s => s.Id)
            .ToListAsync();

        // Si no hay campañas en esas fechas, devolvemos una lista vacía
        if (!overlappingSeparataIds.Any()) return new List<SeparataItem>();

        // 2. Devolvemos SOLO los productos (items) que pertenecen a esas campañas cruzadas
        return await _context.SeparataItems
            .Where(item => overlappingSeparataIds.Contains(item.SeparataId))
            .ToListAsync();
    }
}