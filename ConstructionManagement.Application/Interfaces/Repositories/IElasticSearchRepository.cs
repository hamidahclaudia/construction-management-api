using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Application.Interfaces.Repositories;

public interface IElasticSearchRepository
{
    Task<IEnumerable<Project>> GetProjectsAsync();
    Task<Project?> GetProjectByIdAsync(int id);
}