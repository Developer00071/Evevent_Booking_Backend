using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrganizerRepository(AppDbContext appDbContext) : IOrganizerRepository
{
    public IQueryable<Organizer> GetQuery() => appDbContext.Organizers;
    public Task<List<Organizer>> GetOrganizers() => appDbContext.Organizers.ToListAsync();
    public async Task<Organizer?> GetOrganizer(int id) => await appDbContext.Organizers.FindAsync(id);
    public Task AddOrganizer(Organizer organizer) => appDbContext.Organizers.AddAsync(organizer).AsTask();
    public void UpdateOrganizer(Organizer organizer) => appDbContext.Organizers.Update(organizer);
    public void DeleteOrganizer(Organizer organizer) => appDbContext.Organizers.Remove(organizer);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
