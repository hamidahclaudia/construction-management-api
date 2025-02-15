namespace ConstructionManagement.Application.DTOs;

public class ProjectCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public List<ProjectDto> Projects { get; set; } = new  ();
}