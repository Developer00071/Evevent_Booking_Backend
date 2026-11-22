using Application.DTOs;

namespace Application.Interfaces;

public interface IVenueServer
{
    Task<VenueDto> AddVenue(VenueCreateDto dto);
    Task<List<VenueDto>> GetVenues();
    Task DeleteVenue(int id);
}
