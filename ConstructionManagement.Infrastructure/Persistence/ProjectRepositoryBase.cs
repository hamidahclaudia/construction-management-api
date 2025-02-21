using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.Infrastructure.Persistence;

public class ProjectRepositoryBase(ApplicationDbContext context) : RepositoryBase<Project>(context), IProjectRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Project>> GetProjectsByCategoryAsync(int categoryId)
    {
        return await _context.Projects
            .Where(p => p.ProjectCategoryId == categoryId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Project>> GetProjectsWithDetailsAsync()
    {
        return await _context.Projects
            .Include(p => p.ProjectStage)
            .Include(p => p.ProjectCategory)
            .ToListAsync();
    }
    
    public async Task<Project?> GetProjectsWithDetailsById(int id)
    {
        return await _context.Projects
            .Where(p => p.Id == id)
            .Include(p => p.ProjectStage)
            .Include(p => p.ProjectCategory)
            .FirstOrDefaultAsync();
    }


    public async Task<Project> AddProjectAsync(Project project)
    {
        var category = await _context.ProjectCategories
            .FirstOrDefaultAsync(c => c.Id == project.ProjectCategoryId);

        if (category == null)
        {
            category = new ProjectCategory
            {
                Name = project.ProjectCategory.Name
            };

            _context.ProjectCategories.Add(category);
            await _context.SaveChangesAsync();
            project.ProjectCategoryId = category.Id;
        }
        
        _context.Projects.Add(project);
        return project;

    }
}