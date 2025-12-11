namespace Triyaj.Domain.Entities;

public class Gender
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // M, F
    public string Name { get; set; } = string.Empty; // Erkek, Kadın
    public bool IsActive { get; set; } = true;
}

