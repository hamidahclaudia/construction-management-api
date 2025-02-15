using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.Api.Controllers;

[Route("api/projectstages")]
[ApiController]
[Authorize]
public class ProjectStageController(IProjectStageService projectStageService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjectCategories()
    {
        var stages = await projectStageService.GetProjectStages();
        return Ok(stages);
    }
}

