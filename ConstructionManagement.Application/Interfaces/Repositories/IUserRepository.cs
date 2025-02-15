using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Application.Interfaces.Repositories;

public interface IUserRepository: IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailTakenAsync(string email);
}