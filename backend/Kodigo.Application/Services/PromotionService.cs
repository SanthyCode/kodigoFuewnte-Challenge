using Kodigo.Application.Strategies;

namespace Kodigo.Application.Services;

public class PromotionService
{
    private readonly IEnumerable<IPromotionStrategy> _strategies;

    public PromotionService(IEnumerable<IPromotionStrategy> strategies)
    {
        _strategies = strategies;
    }

    public decimal CalculateFinalPrice(string type, decimal basePrice, decimal value)
    {
        // 1. Buscamos la estrategia exacta usando tu propiedad 'Type'
        var strategy = _strategies.FirstOrDefault(s => s.Type == type);
        
        // 2. Si la encuentra, ejecuta tu método 'Calculate'. Si no, devuelve el precio base.
        return strategy?.Calculate(basePrice, value) ?? basePrice;
    }
}