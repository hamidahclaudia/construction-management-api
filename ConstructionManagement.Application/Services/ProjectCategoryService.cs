using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;

namespace ConstructionManagement.Application.Services;

public class ProjectCategoryService(IProjectCategoryRepository projectCategoryRepository) : IProjectCategoryService
{
    public async Task<IEnumerable<ProjectCategoryDto>> GetCategories()
    {
        var projectCategories = await projectCategoryRepository.GetAllAsync();
        return projectCategories.Select(p => new ProjectCategoryDto
        {
            Id = p.Id,
            Name = p.Name
        });
    }
}