using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IPersonalDetailsService
{
    Task<PersonalDetailsDto?> GetProfileDataAsync(Guid userId, CancellationToken ct = default);
    Task<PersonalDetailsDto> UpdateProfileDataAsync(PersonalDetailsDto details, CancellationToken ct = default);
    Task<bool> UpdateEmailAsync(Guid userId, string newEmail, CancellationToken ct = default);
    Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default);
}

public class PersonalDetailsService : IPersonalDetailsService
{
    private readonly AppDbContext _context;

    public PersonalDetailsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PersonalDetailsDto?> GetProfileDataAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await _context.PersonalDetails.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        return entity is null ? null : MapToDto(entity);
    }

    public async Task<PersonalDetailsDto> UpdateProfileDataAsync(PersonalDetailsDto details, CancellationToken ct = default)
    {
        var entity = await _context.PersonalDetails
            .FirstOrDefaultAsync(p => p.UserId == details.UserId, ct);

        if (entity is null)
        {
            entity = new PersonalDetails
            {
                Id = Guid.NewGuid(),
                UserId = details.UserId
            };
            _context.PersonalDetails.Add(entity);
        }

        entity.Name = details.Name;
        entity.Email = details.Email;
        entity.Phone = details.Phone;
        entity.Address = details.Address;
        entity.Bio = details.Bio;

        await _context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<bool> UpdateEmailAsync(Guid userId, string newEmail, CancellationToken ct = default)
    {
        var entity = await _context.PersonalDetails
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (entity is null)
        {
            return false;
        }

        entity.Email = newEmail;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default)
    {
        return !await _context.PersonalDetails.AnyAsync(p => p.Email == email, ct);
    }

    private static PersonalDetailsDto MapToDto(PersonalDetails entity) =>
        new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            Bio = entity.Bio
        };
}

