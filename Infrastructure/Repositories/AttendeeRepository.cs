using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AttendeeRepository(AppDbContext appDbContext) : IAttendeeRepository
{
    public IQueryable<Attendee> GetQuery() => appDbContext.Attendees;
    public Task<List<Attendee>> GetAttendees() => appDbContext.Attendees.ToListAsync();
    public async Task<Attendee?> GetAttendee(int id) => await appDbContext.Attendees.FindAsync(id);
    public Task AddAttendee(Attendee attendee) => appDbContext.Attendees.AddAsync(attendee).AsTask();
    public void UpdateAttendee(Attendee attendee) => appDbContext.Attendees.Update(attendee);
    public void DeleteAttendee(Attendee attendee) => appDbContext.Attendees.Remove(attendee);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
