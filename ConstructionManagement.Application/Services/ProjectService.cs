using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Helpers;
using ConstructionManagement.Application.Interfaces.Messaging;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;
using ConstructionManagement.Domain.Entities;
using ConstructionManagement.Domain.Enum;

namespace ConstructionManagement.Application.Services;

public class ProjectService(IProjectRepository projectRepository,
    IKafkaProducer kafkaProducer, 
    ProjectNumberGenerator projectNumberGenerator,
    IElasticSearchRepository elasticSearchRepository) : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetAllProjects()
    {
        var projects = await projectRepository.GetProjectsWithDetailsAsync();
        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Location = p.Location,
            ProjectStageId = p.ProjectStageId,
            ProjectStageName = p.ProjectStage.Name,
            ProjectCategoryId = p.ProjectCategoryId,
            ProjectCategoryName = p.ProjectCategory.Name,
            StartDate = p.StartDate,
            Description = p.Description,
            UserId = p.UserId
        });
    }

    public async Task<ProjectDto?> GetProjectById(int id)
    {
        var project = await projectRepository.GetProjectsWithDetailsById(id);
        if (project != null)
        {
            return new ProjectDto()
            {
                Id = project.Id,
                Name = project.Name,
                Location = project.Location,
                ProjectStageId = project.ProjectStageId,
                ProjectStageName = project.ProjectStage.Name,
                ProjectCategoryId = project.ProjectCategoryId,
                ProjectCategoryName = project.ProjectCategory.Name,
                StartDate = project.StartDate,
                Description = project.Description,
                UserId = project.UserId
            };
        }
        return null;
    }
    
    public async Task AddProject(ProjectDto projectDto)
    {
        if ((projectDto.ProjectStageId == (int)ProjectCategoryEnum.Concept
            || projectDto.ProjectStageId == (int)ProjectCategoryEnum.DesignAndDocumentation
            || projectDto.ProjectStageId == (int)ProjectCategoryEnum.PreConstruction)
            && projectDto.StartDate <= DateTime.Now)
        {
            throw new ApplicationException("Project cannot be created");
        }
            
        var project = new Project
        {
            Id = projectNumberGenerator.GenerateHashedRandomId(),
            Name = projectDto.Name,
            Location = projectDto.Location,
            ProjectStageId = projectDto.ProjectStageId,
            ProjectCategoryId = projectDto.ProjectCategoryId,
            StartDate = projectDto.StartDate,
            Description = projectDto.Description,
            UserId = projectDto.UserId
        };

        await projectRepository.AddAsync(project);
        await projectRepository.SaveChangesAsync();

        project.ProjectCategory = new ProjectCategory()
        {
            Id = projectDto.ProjectCategoryId,
            Name = projectDto.ProjectCategoryName,
        };
        project.ProjectStage = new ProjectStage()
        {
            Id = projectDto.ProjectStageId,
            Name = projectDto.ProjectStageName,
        };
        await kafkaProducer.SendMessageAsync(project);
    }
    
    public async Task UpdateProject(ProjectDto projectDto)
    {
        var project = await projectRepository.GetByIdAsync(projectDto.Id);
        if (project != null)
        {
            project.Name = projectDto.Name;
            project.Location = projectDto.Location;
            project.ProjectStageId = projectDto.ProjectStageId;
            project.ProjectCategoryId = projectDto.ProjectCategoryId;
            project.StartDate = projectDto.StartDate;
            project.Description = projectDto.Description;
            project.UserId = projectDto.UserId;
            
            projectRepository.Update(project);
            await projectRepository.SaveChangesAsync();
        }
    }
    
    public async Task DeleteProject(int id)
    {
        var projectToDelete = await projectRepository.GetByIdAsync(id);
        if (projectToDelete == null)
            return;
        projectRepository.Delete(projectToDelete);
        await projectRepository.SaveChangesAsync();
    }
    
}