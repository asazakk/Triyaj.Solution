namespace Triyaj.Application.DTOs;

public class CreateEncounterDto
{
    public Guid PatientId { get; set; }
    public string Source { get; set; } = string.Empty;
}
