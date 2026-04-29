using Kodigo.Domain.Entities;
using Kodigo.Application.Interfaces;
using Kodigo.Application.DTOs; // Importamos los DTOs
using System.Text.RegularExpressions;
using System.Text;

namespace Kodigo.Application.Validators;

public class SeparataValidator
{
    private readonly ISeparataRepository _repository;

    public SeparataValidator(ISeparataRepository repository)
    {
        _repository = repository;
    }

    // AHORA RECIBE EL DTO COMPLETO
    public async Task<List<string>> ValidateAsync(SeparataDto request)
    {
        var errors = new List<string>();

        // 1. Validaciones de Nombre y Fechas
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3)
            errors.Add("El nombre es demasiado corto o inválido.");
        else
        {
            var normalizedRequestName = NormalizeName(request.Name);
            var separatasExistentes = await _repository.GetAllAsync();
            
            if (separatasExistentes.Any(s => NormalizeName(s.Name) == normalizedRequestName))
                errors.Add("Ya existe una campaña con este nombre.");

            if (request.EndDate > request.StartDate)
            {
                bool existeSolapamiento = separatasExistentes.Any(s => 
                    request.StartDate < s.EndDate && request.EndDate > s.StartDate);

                if (existeSolapamiento)
                    errors.Add("El rango de fechas se solapa con una oferta existente.");
            }
        }

        if (request.EndDate <= request.StartDate)
            errors.Add("La fecha de fin no puede ser anterior o igual al inicio.");

        // 2. ¡NUEVA REGLA!: Validación Financiera de los Productos
        if (request.Items != null && request.Items.Any())
        {
            foreach (var item in request.Items)
            {
                var product = await _repository.GetProductByIdAsync(item.ProductId);
                
                if (product != null && item.PromotionType == "Direct")
                {
                    // Comprobamos que el descuento no quiebre la empresa
                    if (item.PromotionValue > product.BasePrice)
                    {
                        errors.Add($"El descuento de ${item.PromotionValue} supera el precio base de '{product.Name}' (${product.BasePrice}).");
                    }
                }
            }
        }

        return errors;
    }

    private static string NormalizeName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var normalizedString = input.Normalize(NormalizationForm.FormD).ToLowerInvariant();
        return Regex.Replace(normalizedString, @"[^a-z0-9]", "");
    }
}