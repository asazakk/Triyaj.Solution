using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triyaj.Application.DTOs;
using Triyaj.Application.Services;
using Triyaj.Infrastructure;

namespace Triyaj.API.Controllers;

/// <summary>
/// MVC Controller - Sayfa görüntüleme için
/// </summary>
public class EncountersController : Controller
{
    private readonly TriyajDbContext _db;

    public EncountersController(TriyajDbContext db) => _db = db;

    public IActionResult Index() => View();

    public async Task<IActionResult> Triage(Guid id)
    {
        var encounter = await _db.Encounters
            .Include(e => e.Patient)
            .FirstOrDefaultAsync(e => e.Id == id);

        return encounter == null ? NotFound() : View(encounter);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await _db.Patients.ToListAsync();
        return View();
    }
}

/// <summary>
/// API Controller - REST endpoint'leri
/// </summary>
[ApiController]
[Route("api/encounters")]
public class EncountersApiController : ControllerBase
{
    private readonly ITriageService _triageService;
    private readonly TriyajDbContext _db;

    public EncountersApiController(ITriageService triageService, TriyajDbContext db)
    {
        _triageService = triageService;
        _db = db;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var encounters = await _db.Encounters
            .Include(e => e.Patient)
            .Where(e => e.Status == "Waiting")
            .OrderBy(e => e.TriageLevel)
            .ThenBy(e => e.Queueposition)
            .ToListAsync();

        var assessments = await GetLatestAssessments(encounters.Select(e => e.Id));

        return Ok(encounters.Select(e => MapEncounterWithAssessment(e, assessments)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var encounters = await _db.Encounters
            .Include(e => e.Patient)
            .OrderByDescending(e => e.ArrivalTime)
            .Select(e => new
            {
                e.Id,
                PatientName = e.Patient.FirstName + " " + e.Patient.LastName,
                e.ArrivalTime,
                TriageLevel = e.TriageLevel.ToString(),
                e.Status
            })
            .ToListAsync();
        
        return Ok(encounters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var encounter = await _db.Encounters
            .Include(e => e.Patient)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (encounter == null) return NotFound();

        var assessment = await _db.TriageAssesments
            .Where(t => t.EncounterId == id)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        return Ok(new
        {
            encounter.Id,
            encounter.PatientId,
            PatientName = encounter.Patient.FirstName + " " + encounter.Patient.LastName,
            encounter.Patient.NationalId,
            encounter.ArrivalTime,
            TriageLevel = encounter.TriageLevel.ToString(),
            encounter.Source,
            encounter.Status,
            assessment?.HeartRate,
            assessment?.SystolicBP,
            assessment?.DiastolicBP,
            assessment?.SpO2,
            assessment?.Temperature,
            assessment?.Complaint,
            assessment?.RecommendedAction,
            AssessmentDate = assessment?.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEncounterDto dto)
    {
        var id = await _triageService.CreateEncounterAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPost("{id}/triage")]
    public async Task<IActionResult> Triage(Guid id, [FromBody] TriageDto dto)
    {
        try
        {
            var result = await _triageService.EvaluateTriageAsync(id, dto);
            return Ok(new { status = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        var encounter = await _db.Encounters.FindAsync(id);
        if (encounter == null) return NotFound();

        encounter.Status = dto.Status;
        await _db.SaveChangesAsync();
        return Ok();
    }

    // Helper metodlar
    private async Task<List<Domain.Entities.TriageAssesment>> GetLatestAssessments(IEnumerable<Guid> ids)
    {
        return await _db.TriageAssesments
            .Where(t => ids.Contains(t.EncounterId))
            .GroupBy(t => t.EncounterId)
            .Select(g => g.OrderByDescending(t => t.CreatedAt).First())
            .ToListAsync();
    }

    private static object MapEncounterWithAssessment(
        Domain.Entities.Encounter e, 
        List<Domain.Entities.TriageAssesment> assessments)
    {
        var a = assessments.FirstOrDefault(x => x.EncounterId == e.Id);
        return new
        {
            e.Id,
            e.PatientId,
            PatientName = e.Patient.FirstName + " " + e.Patient.LastName,
            e.Patient.NationalId,
            e.ArrivalTime,
            TriageLevel = e.TriageLevel.ToString(),
            e.Source,
            e.Status,
            QueuePosition = e.Queueposition,
            a?.HeartRate,
            a?.SystolicBP,
            a?.DiastolicBP,
            a?.SpO2,
            a?.Temperature,
            a?.Complaint,
            a?.RecommendedAction
        };
    }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

