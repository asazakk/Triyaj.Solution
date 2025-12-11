namespace Triyaj.Application.DTOs;

public class TriageDto
{
    public int HeartRate { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int SpO2 { get; set; }
    public double Temperature { get; set; }
    public string Complaint { get; set; } = string.Empty;
}
