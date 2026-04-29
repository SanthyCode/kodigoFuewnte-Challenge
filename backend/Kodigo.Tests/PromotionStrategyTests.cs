using Kodigo.Application.Strategies;
using Xunit;

namespace Kodigo.Tests;

public class PromotionStrategyTests
{
    [Fact]
    public void PercentageStrategy_DeberiaCalcularDescuentoCorrectamente()
    {
        // Arrange (Preparar)
        var strategy = new PercentageStrategy();
        decimal precioBase = 1000m;
        decimal porcentajeDescuento = 10m; // 10%

        // Act (Ejecutar)
        var resultado = strategy.Calculate(precioBase, porcentajeDescuento);

        // Assert (Verificar)
        Assert.Equal(900m, resultado);
    }

    [Fact]
    public void DirectDiscountStrategy_NoDeberiaDevolverPrecioNegativo()
    {
        // Arrange
        var strategy = new DirectDiscountStrategy();
        decimal precioBase = 1000m;
        decimal descuentoDirecto = 1500m; // Descuento mayor al precio

        // Act
        var resultado = strategy.Calculate(precioBase, descuentoDirecto);

        // Assert
        Assert.Equal(0m, resultado); // El precio final debe ser 0, no -500
    }
}