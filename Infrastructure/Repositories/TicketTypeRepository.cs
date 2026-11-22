using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TicketTypeRepository(AppDbContext appDbContext) : ITicketTypeRepository
{
    public IQueryable<TicketType> GetQuery() => appDbContext.TicketTypes;
    public Task<List<TicketType>> GetTicketTypes() => appDbContext.TicketTypes.ToListAsync();
    public async Task<TicketType?> GetTicketType(int id) => await appDbContext.TicketTypes.FindAsync(id);
    public Task AddTicketType(TicketType ticketType) => appDbContext.TicketTypes.AddAsync(ticketType).AsTask();
    public void UpdateTicketType(TicketType ticketType) => appDbContext.TicketTypes.Update(ticketType);
    public void DeleteTicketType(TicketType ticketType) => appDbContext.TicketTypes.Remove(ticketType);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();
}
