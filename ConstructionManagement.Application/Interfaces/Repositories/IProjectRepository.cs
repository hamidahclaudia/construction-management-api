using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Application.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByCategoryAsync(int categoryId);
    Task<IEnumerable<Project>> GetProjectsWithDetailsAsync();
    Task<Project?> GetProjectsWithDetailsById(int id);
    Task<Project> AddProjectAsync(Project project);
}