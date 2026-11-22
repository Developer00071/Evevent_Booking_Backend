using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrganizerRepository
{
    IQueryable<Organizer> GetQuery();
    Task<List<Organizer>> GetOrganizers();
    Task<Organizer?> GetOrganizer(int id);
    Task AddOrganizer(Organizer organizer);
    void UpdateOrganizer(Organizer organizer);
    void DeleteOrganizer(Organizer organizer);
    Task<int> SaveChangesAsync();
}
