using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventRepository(AppDbContext appDbContext) : IEventRepository
{
    public IQueryable<Event> GetQuery() => appDbContext.Events;
    public Task<List<Event>> GetEvents() => appDbContext.Events.ToListAsync();
    public async Task<Event?> GetEvent(int id) => await appDbContext.Events.FindAsync(id);
    public Task AddEvent(Event eventEntity) => appDbContext.Events.AddAsync(eventEntity).AsTask();
    public void UpdateEvent(Event eventEntity) => appDbContext.Events.Update(eventEntity);
    public void DeleteEvent(Event eventEntity) => appDbContext.Events.Remove(eventEntity);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
