using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;

namespace ConstructionManagement.Application.Services;

public class ProjectStageService(IProjectStageRepository projectStageRepository) : IProjectStageService
{
    public async Task<IEnumerable<ProjectStageDto>> GetProjectStages()
    {
        var projectStages = await projectStageRepository.GetAllAsync();
        return projectStages.Select(p => new ProjectStageDto
        {
            Id = p.Id,
            Name = p.Name
        });
    }
}