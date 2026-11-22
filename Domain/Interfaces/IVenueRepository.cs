using Domain.Entities;

namespace Domain.Interfaces;

public interface IVenueRepository
{
    IQueryable<Venue> GetQuery();
    Task<List<Venue>> GetVenues();
    Task<Venue?> GetVenue(int id);
    Task AddVenue(Venue venue);
    void UpdateVenue(Venue venue);
    void DeleteVenue(Venue venue);
    Task<int> SaveChangesAsync();
}
