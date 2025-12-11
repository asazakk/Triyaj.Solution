using Microsoft.EntityFrameworkCore;
using Triyaj.Application.DTOs;
using Triyaj.Application.Services;
using Triyaj.Domain.Entities;
using Triyaj.Domain.ValueObjects;
using Triyaj.Infrastructure;
using Triyaj.Infrastructure.Repositories;

namespace Triyaj.UnitTests;

/// <summary>
/// Test için InMemory veritabanı oluşturan base class
/// </summary>
public abstract class TestBase
{
    protected static TriyajDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TriyajDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TriyajDbContext(options);
    }

    protected static Patient CreateTestPatient() => new()
    {
        Id = Guid.NewGuid(),
        NationalId = "10000000146",
        FirstName = "Test",
        LastName = "Hasta",
        BirthDate = new DateTime(1990, 1, 1),
        Gender = "M",
        Phone = "05001234567"
    };

    protected static Encounter CreateTestEncounter(Guid patientId) => new()
    {
        Id = Guid.NewGuid(),
        PatientId = patientId,
        ArrivalTime = DateTime.UtcNow,
        Source = "Walk-in",
        Status = "Waiting",
        TriageLevel = TriageLevel.Undefined,
        Queueposition = 1
    };
}

/// <summary>
/// Triyaj servisi testleri
/// </summary>
public class TriageServiceTests : TestBase
{
    [Fact]
    public async Task CreateEncounter_ShouldSetWaitingStatus()
    {
        // Arrange
        var context = CreateContext();
        var patient = CreateTestPatient();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var service = new TriageService(
            new GenericRepository<Encounter>(context),
            new GenericRepository<TriageAssesment>(context));

        // Act
        var id = await service.CreateEncounterAsync(new CreateEncounterDto 
        { 
            PatientId = patient.Id, 
            Source = "Walk-in" 
        });

        // Assert
        var encounter = await context.Encounters.FindAsync(id);
        Assert.NotNull(encounter);
        Assert.Equal("Waiting", encounter.Status);
        Assert.Equal(TriageLevel.Undefined, encounter.TriageLevel);
    }

    [Theory]
    [InlineData(85, 120, 80, 85, 37.0, "Red")]    // Düşük SpO2 -> Kırmızı
    [InlineData(140, 120, 80, 96, 37.0, "Red")]   // Yüksek nabız -> Kırmızı
    [InlineData(85, 85, 60, 96, 37.0, "Red")]     // Düşük tansiyon -> Kırmızı
    [InlineData(85, 120, 80, 96, 39.5, "Yellow")] // Yüksek ateş -> Sarı
    [InlineData(105, 120, 80, 96, 37.0, "Yellow")]// Orta nabız -> Sarı
    [InlineData(75, 120, 80, 98, 37.0, "Blue")]   // Mükemmel değerler -> Mavi
    [InlineData(85, 125, 80, 97, 37.2, "Green")]  // Normal değerler -> Yeşil
    public async Task EvaluateTriage_ShouldReturnCorrectLevel(
        int heartRate, int systolicBP, int diastolicBP, int spO2, double temp, string expected)
    {
        // Arrange
        var context = CreateContext();
        var patient = CreateTestPatient();
        context.Patients.Add(patient);
        
        var encounter = CreateTestEncounter(patient.Id);
        context.Encounters.Add(encounter);
        await context.SaveChangesAsync();

        var service = new TriageService(
            new GenericRepository<Encounter>(context),
            new GenericRepository<TriageAssesment>(context));

        var dto = new TriageDto
        {
            HeartRate = heartRate,
            SystolicBP = systolicBP,
            DiastolicBP = diastolicBP,
            SpO2 = spO2,
            Temperature = temp,
            Complaint = "Test"
        };

        // Act
        var result = await service.EvaluateTriageAsync(encounter.Id, dto);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task EvaluateTriage_ShouldThrow_WhenEncounterNotFound()
    {
        // Arrange
        var context = CreateContext();
        var service = new TriageService(
            new GenericRepository<Encounter>(context),
            new GenericRepository<TriageAssesment>(context));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            service.EvaluateTriageAsync(Guid.NewGuid(), new TriageDto()));
    }
}

/// <summary>
/// TC Kimlik No doğrulama testleri
/// </summary>
public class TcKimlikValidationTests
{
    [Theory]
    [InlineData("10000000146", true)]   // Geçerli
    [InlineData("12345678901", false)]  // Geçersiz algoritma
    [InlineData("12345678", false)]     // Kısa
    [InlineData("123456789012", false)] // Uzun
    [InlineData("01234567890", false)]  // 0 ile başlıyor
    [InlineData("", false)]             // Boş
    [InlineData("abcdefghijk", false)]  // Harf içeriyor
    public void Validate_ShouldReturnExpectedResult(string tcNo, bool expected)
    {
        Assert.Equal(expected, IsValidTc(tcNo));
    }

    private static bool IsValidTc(string tcNo)
    {
        if (string.IsNullOrEmpty(tcNo) || tcNo.Length != 11 || !tcNo.All(char.IsDigit) || tcNo[0] == '0')
            return false;

        var d = tcNo.Select(c => c - '0').ToArray();
        
        // Algoritma kontrolleri
        var oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
        var evenSum = d[1] + d[3] + d[5] + d[7];
        
        return d.Take(10).Sum() % 10 == d[10] && 
               ((oddSum * 7 - evenSum) % 10 + 10) % 10 == d[9];
    }
}

/// <summary>
/// Entity testleri
/// </summary>
public class EntityTests : TestBase
{
    [Fact]
    public async Task Patient_ShouldSaveCorrectly()
    {
        var context = CreateContext();
        var patient = CreateTestPatient();

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var saved = await context.Patients.FindAsync(patient.Id);
        Assert.NotNull(saved);
        Assert.Equal("10000000146", saved.NationalId);
    }

    [Fact]
    public async Task Encounter_ShouldHaveDefaultValues()
    {
        var context = CreateContext();
        var patient = CreateTestPatient();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var encounter = new Encounter { PatientId = patient.Id, Source = "Walk-in" };
        context.Encounters.Add(encounter);
        await context.SaveChangesAsync();

        Assert.Equal("Waiting", encounter.Status);
        Assert.Equal(TriageLevel.Undefined, encounter.TriageLevel);
    }

    [Fact]
    public async Task QueuePosition_ShouldIncrementCorrectly()
    {
        var context = CreateContext();
        var patient = CreateTestPatient();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var service = new TriageService(
            new GenericRepository<Encounter>(context),
            new GenericRepository<TriageAssesment>(context));

        var id1 = await service.CreateEncounterAsync(new CreateEncounterDto { PatientId = patient.Id, Source = "Walk-in" });
        var id2 = await service.CreateEncounterAsync(new CreateEncounterDto { PatientId = patient.Id, Source = "Ambulance" });

        Assert.Equal(1, (await context.Encounters.FindAsync(id1))!.Queueposition);
        Assert.Equal(2, (await context.Encounters.FindAsync(id2))!.Queueposition);
    }

    [Fact]
    public async Task Gender_ShouldSaveCorrectly()
    {
        var context = CreateContext();
        context.Genders.Add(new Gender { Code = "M", Name = "Erkek", IsActive = true });
        await context.SaveChangesAsync();

        var saved = await context.Genders.FirstAsync();
        Assert.Equal("M", saved.Code);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task ArrivalSource_ShouldSaveCorrectly()
    {
        var context = CreateContext();
        context.ArrivalSources.Add(new ArrivalSource { Code = "Walk-in", Name = "Yürüyerek", IsActive = true });
        await context.SaveChangesAsync();

        var saved = await context.ArrivalSources.FirstAsync();
        Assert.Equal("Walk-in", saved.Code);
        Assert.True(saved.IsActive);
    }
}
