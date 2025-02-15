using System.ComponentModel.DataAnnotations;

namespace ConstructionManagement.Domain.Entities;

public class ProjectCategory
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = String.Empty;
    public virtual List<Project> Projects { get; set; } = new  ();
}