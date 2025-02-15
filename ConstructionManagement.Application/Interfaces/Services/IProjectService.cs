using ConstructionManagement.Application.DTOs;

namespace ConstructionManagement.Application.Interfaces.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjects();
    Task<ProjectDto?> GetProjectById(int id);
    Task AddProject(ProjectDto projectDto);
    Task UpdateProject(ProjectDto projectDto);
    Task DeleteProject(int id);
}