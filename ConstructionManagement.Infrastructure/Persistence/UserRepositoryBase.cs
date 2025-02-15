using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.Infrastructure.Persistence;

public class UserRepositoryBase(ApplicationDbContext context) : RepositoryBase<User>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}