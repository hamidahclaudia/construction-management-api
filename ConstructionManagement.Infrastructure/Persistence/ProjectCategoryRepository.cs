using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Infrastructure.Persistence;

public class ProjectCategoryRepository(ApplicationDbContext context) : RepositoryBase<ProjectCategory>(context), IProjectCategoryRepository
{
    
}