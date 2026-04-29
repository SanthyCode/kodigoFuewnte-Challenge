using Microsoft.AspNetCore.Mvc;
using Kodigo.Infrastructure;
using Kodigo.Domain.Entities;
using Kodigo.Application.Validators;

namespace Kodigo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeparataController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly SeparataValidator _validator;

    public SeparataController(ApplicationDbContext context, SeparataValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    // 1. NUEVO ENDPOINT PARA EL FRONTEND: Valida todo en tiempo real
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateSeparata([FromBody] SeparataDto request)
    {
        var errors = await _validator.ValidateAsync(request.Name, request.StartDate, request.EndDate);
        
        // Devolvemos si es válido y la lista de errores encontrados
        return Ok(new { isValid = errors.Count == 0, errors });
    }

    // 2. ENDPOINT DE GUARDADO TOTALMENTE LIMPIO
    [HttpPost]
    public async Task<IActionResult> CreateSeparata([FromBody] SeparataDto request)
    {
        // Re-validamos por seguridad (nunca confíes en el Frontend)
        var errors = await _validator.ValidateAsync(request.Name, request.StartDate, request.EndDate);
        
        if (errors.Count > 0)
        {
            return BadRequest(new { mensaje = string.Join(" | ", errors) });
        }

        var nuevaSeparata = new Separata
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _context.Separatas.Add(nuevaSeparata);
        await _context.SaveChangesAsync();
        
        return Ok(new { mensaje = "Separata creada y guardada con éxito en la base de datos.", data = nuevaSeparata });
    }
}

public class SeparataDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<SeparataItemDto> Items { get; set; } = new();
}

public class SeparataItemDto
{
    public Guid ProductId { get; set; }
    public string PromotionType { get; set; } = "Direct";
    public decimal PromotionValue { get; set; }
}