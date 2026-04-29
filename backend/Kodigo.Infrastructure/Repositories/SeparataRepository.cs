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
}