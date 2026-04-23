using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<UserDto> CreateAsync(UserDto user, CancellationToken ct = default);
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        return entity is null ? null : new UserDto { Id = entity.Id };
    }

    public async Task<UserDto> CreateAsync(UserDto user, CancellationToken ct = default)
    {
        var entity = new User
        {
            Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id
        };

        _context.Users.Add(entity);
        await _context.SaveChangesAsync(ct);

        user.Id = entity.Id;
        return user;
    }
}

