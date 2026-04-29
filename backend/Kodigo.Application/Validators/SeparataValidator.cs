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

        // 1. Validaciones de Nombre
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3)
            errors.Add("El nombre es demasiado corto o inválido.");
        else
        {
            var normalizedRequestName = NormalizeName(request.Name);
            var separatasExistentes = await _repository.GetAllAsync();

            if (separatasExistentes.Any(s => NormalizeName(s.Name) == normalizedRequestName))
                errors.Add("Ya existe una campaña con este nombre.");
        }

        // 2. Validaciones Lógicas de Fechas
        if (request.EndDate <= request.StartDate)
        {
            errors.Add("La fecha de fin no puede ser anterior o igual al inicio.");
        }

        // 3. ¡NUEVA LÓGICA DE SOLAPAMIENTO POR PRODUCTO!
        if (request.EndDate > request.StartDate && request.Items != null && request.Items.Any())
        {
            // Traemos todos los items de la base de datos que ya están ocupados en estas fechas
            var overlappingItems = await _repository.GetOverlappingItemsAsync(request.StartDate, request.EndDate);

            foreach (var item in request.Items)
            {
                // Si el producto que intentamos guardar ya está en la lista de cruzados...
                if (overlappingItems.Any(oi => oi.ProductId == item.ProductId))
                {
                    var product = await _repository.GetProductByIdAsync(item.ProductId);
                    string productName = product != null ? product.Name : "Un producto";

                    // Mostramos un error súper específico al usuario
                    errors.Add($"El producto '{productName}' ya tiene una oferta activa en este rango de fechas.");
                }

                // (Mantenemos la validación financiera que hicimos antes)
                var productForFinancialCheck = await _repository.GetProductByIdAsync(item.ProductId);
                if (productForFinancialCheck != null && item.PromotionType == "Direct")
                {
                    if (item.PromotionValue > productForFinancialCheck.BasePrice)
                    {
                        errors.Add($"El descuento de ${item.PromotionValue} supera el precio base de '{productForFinancialCheck.Name}' (${productForFinancialCheck.BasePrice}).");
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