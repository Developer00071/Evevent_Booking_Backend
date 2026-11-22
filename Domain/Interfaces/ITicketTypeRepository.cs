using Domain.Entities;

namespace Domain.Interfaces;

public interface ITicketTypeRepository
{
    IQueryable<TicketType> GetQuery();
    Task<List<TicketType>> GetTicketTypes();
    Task<TicketType?> GetTicketType(int id);
    Task AddTicketType(TicketType ticketType);
    void UpdateTicketType(TicketType ticketType);
    void DeleteTicketType(TicketType ticketType);
    Task<int> SaveChangesAsync();
}
