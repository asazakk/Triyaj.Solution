using Triyaj.Application.DTOs;

namespace Triyaj.Application.Services;

public interface ITriageService
{
    Task<Guid> CreateEncounterAsync(CreateEncounterDto createEncounterDto);
    Task<string> EvaluateTriageAsync(Guid encounterId, TriageDto triageDto);
}
