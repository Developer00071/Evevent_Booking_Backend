using Domain.Entities;

namespace Domain.Interfaces;

public interface IAttendeeRepository
{
    IQueryable<Attendee> GetQuery();
    Task<List<Attendee>> GetAttendees();
    Task<Attendee?> GetAttendee(int id);
    Task AddAttendee(Attendee attendee);
    void UpdateAttendee(Attendee attendee);
    void DeleteAttendee(Attendee attendee);
    Task<int> SaveChangesAsync();
}
