namespace Triyaj.API.Models;

/// <summary>
/// Bekleyen hasta listesi için View Model
/// </summary>
public class PendingPatientViewModel
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }
    public string TriageLevel { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    
    // Triyaj verileri
    public int? HeartRate { get; set; }
    public int? SystolicBP { get; set; }
    public int? DiastolicBP { get; set; }
    public int? SpO2 { get; set; }
    public double? Temperature { get; set; }
    public string? Complaint { get; set; }
    public string? RecommendedAction { get; set; }
}

/// <summary>
/// Hasta detay için View Model
/// </summary>
public class PatientDetailViewModel
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }
    public string TriageLevel { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    // Triyaj verileri
    public int? HeartRate { get; set; }
    public int? SystolicBP { get; set; }
    public int? DiastolicBP { get; set; }
    public int? SpO2 { get; set; }
    public double? Temperature { get; set; }
    public string? Complaint { get; set; }
    public string? RecommendedAction { get; set; }
    public DateTime? AssessmentDate { get; set; }
}

