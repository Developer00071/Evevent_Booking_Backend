using Domain.Entities;

namespace Domain.Interfaces;

public interface ITicketRepository
{
    IQueryable<Ticket> GetQuery();
    Task<List<Ticket>> GetTickets();
    Task<Ticket?> GetTicket(int id);
    Task AddTicket(Ticket ticket);
    Task AddTickets(IEnumerable<Ticket> tickets);
    void UpdateTicket(Ticket ticket);
    void DeleteTicket(Ticket ticket);
    Task<int> SaveChangesAsync();
}
