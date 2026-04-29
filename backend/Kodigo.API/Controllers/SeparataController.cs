using Microsoft.AspNetCore.Mvc;
using Kodigo.Infrastructure;
using Kodigo.Domain.Entities;
using Kodigo.Application.Validators;
using Kodigo.Application.Services;
using Kodigo.Application.DTOs;

namespace Kodigo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeparataController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly SeparataValidator _validator;
    private readonly PromotionService _promotionService;

    // Inyectamos el PromotionService aquí
    public SeparataController(ApplicationDbContext context, SeparataValidator validator, PromotionService promotionService)
    {
        _context = context;
        _validator = validator;
        _promotionService = promotionService;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateSeparata([FromBody] SeparataDto request)
    {
        // Pasamos el request completo
        var errors = await _validator.ValidateAsync(request);
        return Ok(new { isValid = errors.Count == 0, errors });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSeparata([FromBody] SeparataDto request)
    {
        var errors = await _validator.ValidateAsync(request);

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

        if (request.Items != null && request.Items.Any())
        {
            // AQUÍ CUMPLIMOS EL REQUERIMIENTO: Vinculamos los productos
            if (request.Items != null && request.Items.Any())
            {
                foreach (var item in request.Items)
                {
                    // 1. Buscamos el producto real en la base de datos
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                    {
                        // Si un hacker intenta mandar un ID falso desde React, lo bloqueamos
                        return BadRequest(new { mensaje = $"El producto con ID {item.ProductId} no existe." });
                    }

                    // 2. Usamos el patrón Strategy con el precio base REAL
                    decimal precioFinal = _promotionService.CalculateFinalPrice(item.PromotionType, product.BasePrice, item.PromotionValue);

                    var nuevoItem = new SeparataItem
                    {
                        Id = Guid.NewGuid(),
                        SeparataId = nuevaSeparata.Id,
                        ProductId = item.ProductId,
                        PromotionType = item.PromotionType,
                        PromotionValue = item.PromotionValue
                    };

                    _context.SeparataItems.Add(nuevoItem);

                    // Opcional para mostrar tu nivel: Imprimimos en consola el cálculo
                    Console.WriteLine($"Producto: {product.Name} | Precio Base: {product.BasePrice} | Tipo: {item.PromotionType} | Valor Promoción: {item.PromotionValue} | PRECIO FINAL: {precioFinal}");
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Separata y productos guardados con éxito.", data = nuevaSeparata });
    }
}