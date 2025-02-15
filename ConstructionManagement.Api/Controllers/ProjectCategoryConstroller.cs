using ConstructionManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.Api.Controllers;

[Route("api/projectcategories")]
[ApiController]
[Authorize]
public class ProjectCategoryConstroller(IProjectCategoryService projectCategoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjectCategories()
    {
        var categories = await projectCategoryService.GetCategories();
        return Ok(categories);
    }
}