namespace Triyaj.Domain.Entities;

public class ArrivalSource
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // Walk-in, Ambulance, Transfer, Other
    public string Name { get; set; } = string.Empty; // Yürüyerek, Ambulans, Sevk, Diğer
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

