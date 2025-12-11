﻿namespace Triyaj.Domain.Entities;

public class TriageAssesment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;
    public int HeartRate { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int SpO2 { get; set; }
    public double Temperature { get; set; }
    public string Complaint { get; set; } = string.Empty;
    public int TriageScore { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
