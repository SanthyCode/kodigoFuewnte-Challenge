namespace Kodigo.Application.Strategies;

public interface IPromotionStrategy {
    string Type { get; }
    decimal Calculate(decimal originalPrice, decimal value);
}

public class PercentageStrategy : IPromotionStrategy {
    public string Type => "Percentage";
    public decimal Calculate(decimal p, decimal v) => p - (p * (v / 100));
}

public class DirectDiscountStrategy : IPromotionStrategy {
    public string Type => "Direct";
    public decimal Calculate(decimal p, decimal v) => Math.Max(0, p - v);
}