using Domain.Entities;

namespace Domain.Interfaces;

public interface IAttendeeProfileRepository
{
    IQueryable<AttendeeProfile> GetQuery();
    Task<List<AttendeeProfile>> GetAttendeeProfiles();
    Task<AttendeeProfile?> GetAttendeeProfile(int id);
    Task AddAttendeeProfile(AttendeeProfile profile);
    void UpdateAttendeeProfile(AttendeeProfile profile);
    void DeleteAttendeeProfile(AttendeeProfile profile);
    Task<int> SaveChangesAsync();
}
