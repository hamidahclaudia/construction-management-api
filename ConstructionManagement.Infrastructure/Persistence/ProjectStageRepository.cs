using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Infrastructure.Persistence;

public class ProjectStageRepository(ApplicationDbContext context) : RepositoryBase<ProjectStage>(context), IProjectStageRepository
{
    
}