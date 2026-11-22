using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AttendeeProfileRepository(AppDbContext appDbContext) : IAttendeeProfileRepository
{
    public IQueryable<AttendeeProfile> GetQuery() => appDbContext.AttendeeProfiles;
    public Task<List<AttendeeProfile>> GetAttendeeProfiles() => appDbContext.AttendeeProfiles.ToListAsync();
    public async Task<AttendeeProfile?> GetAttendeeProfile(int id) => await appDbContext.AttendeeProfiles.FindAsync(id);
    public Task AddAttendeeProfile(AttendeeProfile profile) => appDbContext.AttendeeProfiles.AddAsync(profile).AsTask();
    public void UpdateAttendeeProfile(AttendeeProfile profile) => appDbContext.AttendeeProfiles.Update(profile);
    public void DeleteAttendeeProfile(AttendeeProfile profile) => appDbContext.AttendeeProfiles.Remove(profile);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
