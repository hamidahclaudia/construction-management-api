using ConstructionManagement.Application.DTOs;
using Moq;
using Xunit;
using ConstructionManagement.Application.Services;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Messaging;
using ConstructionManagement.Application.Helpers;
using ConstructionManagement.Domain.Entities;
using ConstructionManagement.Domain.Enum;

namespace ConstructionManagement.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
        private readonly Mock<ProjectNumberGenerator> _projectNumberGeneratorMock;
        private readonly Mock<IElasticSearchRepository> _elasticSearchRepositoryMock;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();
            _projectNumberGeneratorMock = new Mock<ProjectNumberGenerator>();
            _elasticSearchRepositoryMock = new Mock<IElasticSearchRepository>();

            _projectService = new ProjectService(
                _projectRepositoryMock.Object,
                _kafkaProducerMock.Object,
                _projectNumberGeneratorMock.Object,
                _elasticSearchRepositoryMock.Object
            );
        }
        
        [Fact]
        public async Task GetAllProjects_ShouldReturnProjectDtos_WhenProjectsExist()
        {
            var projects = new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Name = "Project 1",
                    Location = "Location 1",
                    ProjectStageId = 1,
                    ProjectCategoryId = 1,
                    StartDate = DateTime.Now,
                    Description = "Description 1",
                    UserId = 1,
                    ProjectStage = new ProjectStage { Id = 1, Name = "Stage 1" },
                    ProjectCategory = new ProjectCategory { Id = 1, Name = "Category 1" }
                }
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectsWithDetailsAsync()).ReturnsAsync(projects);
            
            var result = await _projectService.GetAllProjects();
            
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Project 1", result.First().Name);
        }
        
        [Fact]
        public async Task GetProjectById_ShouldReturnProjectDto_WhenProjectExists()
        {
            var project = new Project
            {
                Id = 1,
                Name = "Project 1",
                Location = "Location 1",
                ProjectStageId = 1,
                ProjectCategoryId = 1,
                StartDate = DateTime.Now,
                Description = "Description 1",
                UserId = 1,
                ProjectStage = new ProjectStage { Id = 1, Name = "Stage 1" },
                ProjectCategory = new ProjectCategory { Id = 1, Name = "Category 1" }
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectsWithDetailsById(1)).ReturnsAsync(project);
            
            var result = await _projectService.GetProjectById(1);

            Assert.NotNull(result);
            Assert.Equal("Project 1", result?.Name);
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnNull_WhenProjectDoesNotExist()
        {
            _projectRepositoryMock.Setup(repo => repo.GetProjectsWithDetailsById(It.IsAny<int>())).ReturnsAsync((Project)null);
            
            var result = await _projectService.GetProjectById(999);
            
            Assert.Null(result);
        }
        
        [Fact]
        public async Task AddProject_ShouldThrowException_WhenStartDateIsInPast()
        {
            var projectDto = new ProjectDto
            {
                Name = "Project 1",
                Location = "Location 1",
                ProjectStageId = (int)ProjectCategoryEnum.Concept,
                StartDate = DateTime.Now.AddDays(-1),
                ProjectCategoryId = 1,
                Description = "Description 1",
                UserId = 1
            };
            
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _projectService.AddProject(projectDto));
            Assert.Equal("Project cannot be created", exception.Message);
        }

        [Fact]
        public async Task AddProject_ShouldCreateProject_WhenStartDateIsValid()
        {
            var projectDto = new ProjectDto
            {
                Name = "Project 1",
                Location = "Location 1",
                ProjectStageId = (int)ProjectCategoryEnum.Concept,
                StartDate = DateTime.Now.AddDays(1),
                ProjectCategoryId = 1,
                Description = "Description 1",
                UserId = 1
            };
            
            _projectRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);
            _projectRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.FromResult(12345));
            
            await _projectService.AddProject(projectDto);
            
            _projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>()), Times.Once);
            _kafkaProducerMock.Verify(kafka => kafka.SendMessageAsync(It.IsAny<Project>()), Times.Once);
        }
        
        [Fact]
        public async Task UpdateProject_ShouldUpdateProject_WhenProjectExists()
        {
            var existingProject = new Project { Id = 1, Name = "Project 1" };
            var projectDto = new ProjectDto { Id = 1, Name = "Updated Project" };

            _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProject);
            
            await _projectService.UpdateProject(projectDto);
            
            Assert.Equal("Updated Project", existingProject.Name);
            _projectRepositoryMock.Verify(repo => repo.Update(It.IsAny<Project>()), Times.Once);
            _projectRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task DeleteProject_ShouldDeleteProject_WhenProjectExists()
        {
            var projectToDelete = new Project { Id = 1, Name = "Project 1" };
            _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(projectToDelete);
            
            await _projectService.DeleteProject(1);
            
            _projectRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Project>()), Times.Once);
            _projectRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProject_ShouldNotDelete_WhenProjectDoesNotExist()
        {
            _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Project)null);
            
            await _projectService.DeleteProject(999);
            
            _projectRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

    }
    
}