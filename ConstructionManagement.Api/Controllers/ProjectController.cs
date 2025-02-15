using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.Api.Controllers;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await projectService.GetAllProjects();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await projectService.GetProjectById(id);
        if(project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectDto project)
    {
        try
        {
            await projectService.AddProject(project);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
        
        return CreatedAtAction(nameof(GetProject), new {id = project.Id}, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto project)
    {
        await projectService.UpdateProject(project);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await projectService.DeleteProject(id);
        return NoContent();
    }
}