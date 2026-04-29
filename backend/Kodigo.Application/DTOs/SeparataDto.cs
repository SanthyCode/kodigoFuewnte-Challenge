namespace Kodigo.Application.DTOs;

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
    public string PromotionType { get; set; } = string.Empty;
    public decimal PromotionValue { get; set; }
}