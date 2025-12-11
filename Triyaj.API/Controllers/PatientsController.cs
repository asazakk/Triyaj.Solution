using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triyaj.Domain.Entities;
using Triyaj.Infrastructure;

namespace Triyaj.API.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsApiController : ControllerBase
{
    private readonly TriyajDbContext _db;

    public PatientsApiController(TriyajDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _db.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new
            {
                p.Id,
                p.NationalId,
                p.FirstName,
                p.LastName,
                FullName = p.FirstName + " " + p.LastName,
                p.BirthDate,
                p.Gender,
                p.Phone
            })
            .ToListAsync();
        
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var patient = await _db.Patients.FindAsync(id);
        return patient == null ? NotFound() : Ok(patient);
    }

    [HttpGet("search/{nationalId}")]
    public async Task<IActionResult> SearchByNationalId(string nationalId)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.NationalId == nationalId);
        return patient == null 
            ? NotFound(new { message = "Hasta bulunamadı" }) 
            : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        // TC validasyonu
        var tcError = ValidateTcFormat(dto.NationalId);
        if (tcError != null) return BadRequest(new { error = tcError });

        if (!IsValidTcAlgorithm(dto.NationalId))
            return BadRequest(new { error = "Geçersiz T.C. Kimlik numarası." });

        // Mükerrer kontrol
        var existing = await _db.Patients.FirstOrDefaultAsync(p => p.NationalId == dto.NationalId);
        if (existing != null)
            return Conflict(new { 
                error = "Bu T.C. Kimlik numarasıyla kayıtlı hasta zaten mevcut.",
                existingPatient = new { existing.Id, FullName = $"{existing.FirstName} {existing.LastName}" }
            });

        // Cinsiyet kontrolü
        if (!await _db.Genders.AnyAsync(g => g.Code == dto.Gender && g.IsActive))
            return BadRequest(new { error = "Geçersiz cinsiyet kodu." });

        var patient = new Patient
        {
            NationalId = dto.NationalId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            Phone = dto.Phone
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = patient.Id }, new { id = patient.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePatientDto dto)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.BirthDate = dto.BirthDate;
        patient.Gender = dto.Gender;
        patient.Phone = dto.Phone;

        await _db.SaveChangesAsync();
        return Ok();
    }

    // TC Kimlik validasyon metodları
    private static string? ValidateTcFormat(string tc)
    {
        if (string.IsNullOrWhiteSpace(tc) || tc.Length != 11)
            return "T.C. Kimlik numarası 11 haneli olmalıdır.";
        if (!tc.All(char.IsDigit))
            return "T.C. Kimlik numarası sadece rakamlardan oluşmalıdır.";
        if (tc[0] == '0')
            return "T.C. Kimlik numarası 0 ile başlayamaz.";
        return null;
    }

    private static bool IsValidTcAlgorithm(string tc)
    {
        var d = tc.Select(c => c - '0').ToArray();
        var oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
        var evenSum = d[1] + d[3] + d[5] + d[7];
        
        return d.Take(10).Sum() % 10 == d[10] && 
               ((oddSum * 7 - evenSum) % 10 + 10) % 10 == d[9];
    }
}

public class CreatePatientDto
{
    public string NationalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

