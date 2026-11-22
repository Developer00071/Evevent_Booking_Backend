using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TicketService(ITicketRepository ticketRepository) : ITicketServer
{
    public async Task<TicketDto> GetTicket(Guid code)
    {
        return await ticketRepository.GetQuery().AsNoTracking()
            .Where(ticket => ticket.Code == code)
            .Select(ticket => new TicketDto
            {
                Code = ticket.Code,
                EventTitle = ticket.TicketType.Event.Title,
                TicketTypeName = ticket.TicketType.Name,
                Price = ticket.Price,
                IsCheckedIn = ticket.IsCheckedIn
            })
            .FirstOrDefaultAsync()
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Ticket not found.");
    }

    public async Task CheckIn(Guid code)
    {
        var ticket = await ticketRepository.GetQuery()
            .Include(item => item.Order)
            .Include(item => item.TicketType).ThenInclude(ticketType => ticketType.Event)
            .FirstOrDefaultAsync(item => item.Code == code)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Ticket not found.");
        if (ticket.Order.Status != OrderStatus.Paid)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Only paid tickets can be checked in.");
        }
        if (ticket.IsCheckedIn)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Ticket was already checked in.");
        }
        if (DateTime.UtcNow < ticket.TicketType.Event.StartDate.AddHours(-2))
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Check-in opens two hours before the event.");
        }

        ticket.IsCheckedIn = true;
        ticket.CheckedInAt = DateTime.UtcNow;
        await ticketRepository.SaveChangesAsync();
    }
}
