using ConstructionManagement.Application.DTOs;

namespace ConstructionManagement.Application.Interfaces.Services;

public interface IProjectCategoryService
{
    Task<IEnumerable<ProjectCategoryDto>> GetCategories();
}