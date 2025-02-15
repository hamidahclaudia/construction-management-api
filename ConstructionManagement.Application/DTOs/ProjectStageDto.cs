namespace ConstructionManagement.Application.DTOs;

public class ProjectStageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public virtual List<ProjectDto> Projects { get; set; } = new();
}