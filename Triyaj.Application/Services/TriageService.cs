﻿using Triyaj.Application.DTOs;
using Triyaj.Domain.Entities;
using Triyaj.Domain.ValueObjects;
using Triyaj.Infrastructure.Repositories;

namespace Triyaj.Application.Services;

public class TriageService : ITriageService
{
    private readonly IGenericRepository<Encounter> _encRepo;
    private readonly IGenericRepository<TriageAssesment> _triageRepo;

    public TriageService(IGenericRepository<Encounter> encRepo, IGenericRepository<TriageAssesment> triageRepo)
    {
        _encRepo = encRepo;
        _triageRepo = triageRepo;
    }

    public async Task<Guid> CreateEncounterAsync(CreateEncounterDto dto)
    {
        // Mevcut bekleyen hastaların sayısını al
        var allEncounters = await _encRepo.GetAllAsync();
        var waitingCount = allEncounters.Count(e => e.Status == "Waiting");

        var enc = new Encounter
        {
            PatientId = dto.PatientId,
            ArrivalTime = DateTime.UtcNow,
            Source = dto.Source,
            Status = "Waiting",
            TriageLevel = TriageLevel.Undefined,
            Queueposition = waitingCount + 1
        };

        await _encRepo.AddAsync(enc);
        await _encRepo.SaveChangesAsync();
        return enc.Id;
    }

    public async Task<string> EvaluateTriageAsync(Guid encounterId, TriageDto triageDto)
    {
        var enc = await _encRepo.GetByIdAsync(encounterId);
        if (enc == null) throw new Exception("Encounter not found");

        // Triyaj seviyesi hesaplama algoritması
        var level = CalculateTriageLevel(triageDto);

        enc.TriageLevel = level;

        var assessment = new TriageAssesment
        {
            EncounterId = enc.Id,
            HeartRate = triageDto.HeartRate,
            SystolicBP = triageDto.SystolicBP,
            DiastolicBP = triageDto.DiastolicBP,
            SpO2 = triageDto.SpO2,
            Temperature = triageDto.Temperature,
            Complaint = triageDto.Complaint,
            TriageScore = (int)level,
            RecommendedAction = GetRecommendedAction(level),
            CreatedAt = DateTime.UtcNow
        };

        await _triageRepo.AddAsync(assessment);
        _encRepo.Update(enc);
        await _encRepo.SaveChangesAsync();

        return level.ToString();
    }

    private TriageLevel CalculateTriageLevel(TriageDto dto)
    {
        // Kırmızı - Acil durumlar
        if (dto.SpO2 < 90 || dto.HeartRate > 130 || dto.SystolicBP < 90 || dto.SystolicBP > 200)
            return TriageLevel.Red;

        // Sarı - Öncelikli durumlar
        if (dto.Temperature > 39 || dto.HeartRate > 100 || dto.SpO2 < 94 || 
            dto.SystolicBP < 100 || dto.SystolicBP > 160)
            return TriageLevel.Yellow;

        // Mavi - Hafif durumlar (ayaktan tedavi)
        if (dto.Temperature < 37.5 && dto.HeartRate >= 60 && dto.HeartRate <= 80 &&
            dto.SpO2 >= 98 && dto.SystolicBP >= 110 && dto.SystolicBP <= 130)
            return TriageLevel.Blue;

        // Yeşil - Normal durumlar
        return TriageLevel.Green;
    }

    private string GetRecommendedAction(TriageLevel level)
    {
        return level switch
        {
            TriageLevel.Red => "Acil müdahale gerekli - Resüsitasyon",
            TriageLevel.Yellow => "Öncelikli muayene - 15 dakika içinde",
            TriageLevel.Green => "Normal sıra - 60 dakika içinde",
            TriageLevel.Blue => "Ayaktan tedavi - Poliklinik yönlendirmesi",
            _ => "Değerlendirme bekleniyor"
        };
    }
}