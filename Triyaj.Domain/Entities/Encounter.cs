﻿using Triyaj.Domain.ValueObjects;

namespace Triyaj.Domain.Entities;

public class Encounter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime ArrivalTime { get; set; }
    public string Source { get; set; } = string.Empty;//walk,Ambulance..
    public TriageLevel TriageLevel { get; set; } = TriageLevel.Undefined;
    public string Status { get; set; }="Waiting";//Waiting,In Process,Completed
    public Guid AssignedDoctorId { get; set; }
    public int Queueposition { get; set; }
}
