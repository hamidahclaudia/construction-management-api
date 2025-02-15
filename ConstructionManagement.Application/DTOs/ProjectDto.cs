namespace ConstructionManagement.Application.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int ProjectStageId { get; set; }
    public string ProjectStageName { get; set; } = string.Empty;
    public int ProjectCategoryId { get; set; }
    public string ProjectCategoryName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }
}