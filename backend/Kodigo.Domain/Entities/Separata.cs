namespace Kodigo.Domain.Entities;

public class Separata {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<SeparataItem> Items { get; set; } = new();
}

public class SeparataItem
{
    public Guid Id { get; set; }
    public Guid SeparataId { get; set; }
    public Guid ProductId { get; set; }
    public string PromotionType { get; set; } = string.Empty;
    public decimal PromotionValue { get; set; }
}