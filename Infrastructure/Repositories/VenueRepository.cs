using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VenueRepository(AppDbContext appDbContext) : IVenueRepository
{
    public IQueryable<Venue> GetQuery() => appDbContext.Venues;
    public Task<List<Venue>> GetVenues() => appDbContext.Venues.ToListAsync();
    public async Task<Venue?> GetVenue(int id) => await appDbContext.Venues.FindAsync(id);
    public Task AddVenue(Venue venue) => appDbContext.Venues.AddAsync(venue).AsTask();
    public void UpdateVenue(Venue venue) => appDbContext.Venues.Update(venue);
    public void DeleteVenue(Venue venue) => appDbContext.Venues.Remove(venue);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
