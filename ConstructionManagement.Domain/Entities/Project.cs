using System.ComponentModel.DataAnnotations;

namespace ConstructionManagement.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;
    public int ProjectStageId { get; set; }
    public virtual ProjectStage ProjectStage { get; set; } = default!;
    public int ProjectCategoryId { get; set; }
    public virtual ProjectCategory ProjectCategory { get; set; } = default!;
    public DateTime StartDate { get; set; }
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    

}
