using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ReportService(IEventRepository eventRepository, ITicketRepository ticketRepository, ITicketTypeRepository ticketTypeRepository) : IReportServer
{
    public async Task<List<TicketTypeSalesDto>> GetEventSales(int id)
    {
        var eventExists = await eventRepository.GetQuery().AnyAsync(item => item.Id == id);
        if (!eventExists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Event not found.");
        }

        var grouped = await ticketRepository.GetQuery()
            .Where(ticket => ticket.TicketType.EventId == id && ticket.Order.Status == OrderStatus.Paid)
            .GroupBy(ticket => new { ticket.TicketTypeId, ticket.TicketType.Name, ticket.TicketType.Quantity })
            .Select(group => new
            {
                group.Key.TicketTypeId,
                group.Key.Name,
                group.Key.Quantity,
                TicketsSold = group.Count(),
                Revenue = group.Sum(ticket => ticket.Price),
                CheckedIn = group.Count(ticket => ticket.IsCheckedIn)
            }).ToListAsync();
        var ticketTypes = await ticketTypeRepository.GetQuery().AsNoTracking()
            .Where(ticketType => ticketType.EventId == id)
            .Select(ticketType => new { ticketType.Id, ticketType.Name, ticketType.Quantity })
            .ToListAsync();

        return ticketTypes.Select(ticketType =>
        {
            var sales = grouped.FirstOrDefault(item => item.TicketTypeId == ticketType.Id);
            var sold = sales?.TicketsSold ?? 0;
            return new TicketTypeSalesDto
            {
                TicketTypeId = ticketType.Id,
                TicketTypeName = ticketType.Name,
                TicketsSold = sold,
                Revenue = sales?.Revenue ?? 0,
                OccupancyPercentage = ticketType.Quantity == 0 ? 0 : (decimal)sold * 100 / ticketType.Quantity,
                CheckInPercentage = sold == 0 ? 0 : (decimal)(sales?.CheckedIn ?? 0) * 100 / sold
            };
        }).ToList();
    }

    public async Task<List<TopEventDto>> GetTopEvents(int count)
    {
        if (count < 1 || count > 100)
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "count must be between 1 and 100.");
        }

        return await ticketRepository.GetQuery().AsNoTracking()
            .Where(ticket => ticket.Order.Status == OrderStatus.Paid)
            .GroupBy(ticket => new { ticket.TicketType.EventId, ticket.TicketType.Event.Title })
            .Select(group => new TopEventDto
            {
                EventId = group.Key.EventId,
                Title = group.Key.Title,
                Revenue = group.Sum(ticket => ticket.Price)
            })
            .OrderByDescending(eventEntity => eventEntity.Revenue)
            .Take(count)
            .ToListAsync();
    }
}
