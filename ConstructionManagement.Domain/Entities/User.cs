using System.ComponentModel.DataAnnotations;

namespace ConstructionManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; } = String.Empty;
    [Required, MaxLength(100)]
    public string Email { get; set; } = String.Empty;
    [Required, MaxLength(100)]
    public string Password { get; set; } = String.Empty;
    public virtual List<Project> Projects { get; set; } = new List<Project>();
}