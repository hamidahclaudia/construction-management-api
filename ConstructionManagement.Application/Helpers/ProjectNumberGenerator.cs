namespace ConstructionManagement.Application.Helpers;

public class ProjectNumberGenerator
{
    public int GenerateHashedRandomId()
    {
        int hash = Math.Abs(Guid.NewGuid().GetHashCode());
        return (hash % 900000) + 100000; // Ensures 6-digit ID
    }
}