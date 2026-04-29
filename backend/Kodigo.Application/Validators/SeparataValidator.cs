using Kodigo.Domain.Entities;
using Kodigo.Application.Interfaces;
using System.Text.RegularExpressions;
using System.Text;

namespace Kodigo.Application.Validators;

public class SeparataValidator
{
    private readonly ISeparataRepository _repository; // Usamos la interfaz, no el DbContext

    public SeparataValidator(ISeparataRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<string>> ValidateAsync(string name, DateTime startDate, DateTime endDate)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            errors.Add("El nombre es demasiado corto o inválido.");
        }
        else
        {
            var normalizedRequestName = NormalizeName(name);
            
            // Aquí usamos nuestra interfaz para traer los datos
            var separatasExistentes = await _repository.GetAllAsync();
            
            if (separatasExistentes.Any(s => NormalizeName(s.Name) == normalizedRequestName))
            {
                errors.Add("Ya existe una campaña con este nombre.");
            }

            if (endDate > startDate)
            {
                bool existeSolapamiento = separatasExistentes.Any(s => 
                    startDate < s.EndDate && endDate > s.StartDate);

                if (existeSolapamiento)
                {
                    errors.Add("El rango de fechas se solapa con una oferta existente.");
                }
            }
        }

        if (endDate <= startDate)
        {
            errors.Add("La fecha de fin no puede ser anterior o igual al inicio.");
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