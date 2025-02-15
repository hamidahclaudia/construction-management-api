namespace ConstructionManagement.Domain.Entities;

public class ProjectStage
{
    public int Id { get; set; }
    
    public string Name { get; set; } = String.Empty;
    public virtual List<Project> Projects { get; set; } = new();
}