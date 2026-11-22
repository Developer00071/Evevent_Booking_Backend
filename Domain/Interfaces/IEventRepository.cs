using Domain.Entities;

namespace Domain.Interfaces;

public interface IEventRepository
{
    IQueryable<Event> GetQuery();
    Task<List<Event>> GetEvents();
    Task<Event?> GetEvent(int id);
    Task AddEvent(Event eventEntity);
    void UpdateEvent(Event eventEntity);
    void DeleteEvent(Event eventEntity);
    Task<int> SaveChangesAsync();
}
