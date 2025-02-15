using ConstructionManagement.Application.DTOs;

namespace ConstructionManagement.Application.Interfaces.Services;

public interface IProjectStageService
{
    Task<IEnumerable<ProjectStageDto>> GetProjectStages();
}