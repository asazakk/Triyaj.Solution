namespace Triyaj.API.Models;

/// <summary>
/// Hasta oluşturma/güncelleme için DTO
/// </summary>
public class CreatePatientDto
{
    public string NationalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Encounter durum güncelleme için DTO
/// </summary>
public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

