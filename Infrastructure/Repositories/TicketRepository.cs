using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TicketRepository(AppDbContext appDbContext) : ITicketRepository
{
    public IQueryable<Ticket> GetQuery() => appDbContext.Tickets;
    public Task<List<Ticket>> GetTickets() => appDbContext.Tickets.ToListAsync();
    public async Task<Ticket?> GetTicket(int id) => await appDbContext.Tickets.FindAsync(id);
    public Task AddTicket(Ticket ticket) => appDbContext.Tickets.AddAsync(ticket).AsTask();
    public Task AddTickets(IEnumerable<Ticket> tickets) => appDbContext.Tickets.AddRangeAsync(tickets);
    public void UpdateTicket(Ticket ticket) => appDbContext.Tickets.Update(ticket);
    public void DeleteTicket(Ticket ticket) => appDbContext.Tickets.Remove(ticket);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
