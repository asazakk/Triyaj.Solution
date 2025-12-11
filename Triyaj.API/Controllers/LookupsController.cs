using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triyaj.Infrastructure;

namespace Triyaj.API.Controllers;

[ApiController]
[Route("api/lookups")]
public class LookupsController : ControllerBase
{
    private readonly TriyajDbContext _dbContext;

    public LookupsController(TriyajDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Gender Endpoints

    [HttpGet("genders")]
    public async Task<IActionResult> GetGenders()
    {
        var genders = await _dbContext.Genders
            .Where(g => g.IsActive)
            .OrderBy(g => g.Id)
            .Select(g => new { g.Id, g.Code, g.Name })
            .ToListAsync();
        return Ok(genders);
    }

    #endregion

    #region ArrivalSource Endpoints

    [HttpGet("arrival-sources")]
    public async Task<IActionResult> GetArrivalSources()
    {
        var sources = await _dbContext.ArrivalSources
            .Where(s => s.IsActive)
            .OrderBy(s => s.Id)
            .Select(s => new { s.Id, s.Code, s.Name })
            .ToListAsync();
        return Ok(sources);
    }

    #endregion
}


