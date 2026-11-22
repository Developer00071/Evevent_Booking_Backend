using Application.DTOs;

namespace Application.Interfaces;

public interface IOrganizerServer
{
    Task<OrganizerDto> AddOrganizer(OrganizerCreateDto dto);
    Task<List<OrganizerDto>> GetOrganizers();
    Task<List<EventDto>> GetOrganizerEvents(int id);
}
