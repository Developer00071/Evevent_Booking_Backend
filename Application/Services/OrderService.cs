using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService(IOrderRepository orderRepository, IAttendeeRepository attendeeRepository, ITicketTypeRepository ticketTypeRepository, ITicketRepository ticketRepository) : IOrderServer
{
    public async Task<OrderDto> AddOrder(CreateOrderDto dto)
    {
        if (dto.Items.Count == 0 || dto.Items.Any(item => item.Quantity < 1))
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "At least one valid order item is required.");
        }

        OrderDto? orderDto = null;
        await orderRepository.InTransactionAsync(async () =>
        {
            var attendeeExists = await attendeeRepository.GetQuery().AnyAsync(attendee => attendee.Id == dto.AttendeeId);
            if (!attendeeExists)
            {
                throw new ApiException((int)HttpStatusCode.NotFound, "Attendee not found.");
            }

            var requested = dto.Items.GroupBy(item => item.TicketTypeId)
                .Select(group => new { TicketTypeId = group.Key, Quantity = group.Sum(item => item.Quantity) })
                .ToList();
            var ticketTypes = await ticketTypeRepository.GetQuery()
                .Where(ticketType => requested.Select(item => item.TicketTypeId).Contains(ticketType.Id))
                .Include(ticketType => ticketType.Event)
                .ToListAsync();
            if (ticketTypes.Count != requested.Count)
            {
                throw new ApiException((int)HttpStatusCode.BadRequest, "Ticket type does not exist.");
            }
            if (ticketTypes.Select(ticketType => ticketType.EventId).Distinct().Count() > 1)
            {
                throw new ApiException((int)HttpStatusCode.Conflict, "An order can contain tickets for only one event.");
            }

            foreach (var requestedItem in requested)
            {
                var ticketType = ticketTypes.Single(item => item.Id == requestedItem.TicketTypeId);
                if (ticketType.Event.Status != EventStatus.Published || ticketType.Event.StartDate <= DateTime.UtcNow)
                {
                    throw new ApiException((int)HttpStatusCode.Conflict, "Tickets can only be purchased for future published events.");
                }
                if (ticketType.Quantity - ticketType.SoldCount < requestedItem.Quantity)
                {
                    throw new ApiException((int)HttpStatusCode.Conflict, "Not enough tickets are available.");
                }
            }

            foreach (var eventGroup in requested.GroupBy(item => ticketTypes.Single(type => type.Id == item.TicketTypeId).EventId))
            {
                var eventId = eventGroup.Key;
                var existing = await ticketRepository.GetQuery()
                    .Where(ticket => ticket.AttendeeId == dto.AttendeeId &&
                        ticket.TicketType.EventId == eventId &&
                        ticket.Order.Status != OrderStatus.Cancelled &&
                        ticket.Order.Status != OrderStatus.Expired)
                    .CountAsync();
                if (existing + eventGroup.Sum(item => item.Quantity) > 5)
                {
                    throw new ApiException((int)HttpStatusCode.Conflict, "An attendee can purchase at most five tickets per event.");
                }
            }

            var order = new Order
            {
                AttendeeId = dto.AttendeeId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
            await orderRepository.AddOrder(order);
            await orderRepository.SaveChangesAsync();

            var tickets = new List<Ticket>();
            foreach (var requestedItem in requested)
            {
                var ticketType = ticketTypes.Single(item => item.Id == requestedItem.TicketTypeId);
                ticketType.SoldCount += requestedItem.Quantity;
                order.TotalAmount += ticketType.Price * requestedItem.Quantity;
                for (var index = 0; index < requestedItem.Quantity; index++)
                {
                    tickets.Add(new Ticket
                    {
                        OrderId = order.Id,
                        TicketTypeId = ticketType.Id,
                        AttendeeId = dto.AttendeeId,
                        Code = Guid.NewGuid(),
                        Price = ticketType.Price,
                        PurchasedAt = DateTime.UtcNow
                    });
                }
            }
            await ticketRepository.AddTickets(tickets);
            await orderRepository.SaveChangesAsync();
            orderDto = await GetOrder(order.Id);
        });
        return orderDto ?? throw new InvalidOperationException("Order was not created.");
    }

    public async Task<OrderDto> GetOrder(int id)
    {
        return await orderRepository.GetQuery().AsNoTracking()
            .Where(order => order.Id == id)
            .Select(order => new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Tickets = order.Tickets.Select(ticket => new TicketDto
                {
                    Code = ticket.Code,
                    EventTitle = ticket.TicketType.Event.Title,
                    TicketTypeName = ticket.TicketType.Name,
                    Price = ticket.Price,
                    IsCheckedIn = ticket.IsCheckedIn
                }).ToList()
            }).FirstOrDefaultAsync()
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Order not found.");
    }

    public async Task PayOrder(int id)
    {
        var order = await orderRepository.GetQuery().FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Order not found.");
        if (order.Status != OrderStatus.Pending || order.CreatedAt <= DateTime.UtcNow.AddMinutes(-15))
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Only unexpired pending orders can be paid.");
        }

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        await orderRepository.SaveChangesAsync();
    }

    public async Task CancelOrder(int id)
    {
        await orderRepository.InTransactionAsync(async () =>
        {
            var order = await orderRepository.GetQuery().Include(item => item.Tickets).ThenInclude(ticket => ticket.TicketType)
                .FirstOrDefaultAsync(item => item.Id == id)
                ?? throw new ApiException((int)HttpStatusCode.NotFound, "Order not found.");
            if (order.Status != OrderStatus.Pending)
            {
                throw new ApiException((int)HttpStatusCode.Conflict, "Only pending orders can be cancelled.");
            }

            order.Status = OrderStatus.Cancelled;
            foreach (var ticket in order.Tickets)
            {
                ticket.TicketType.SoldCount--;
            }
            await orderRepository.SaveChangesAsync();
        });
    }

    public async Task ExpireOrders()
    {
        await orderRepository.InTransactionAsync(async () =>
        {
            var expiredOrders = await orderRepository.GetQuery()
                .Where(order => order.Status == OrderStatus.Pending && order.CreatedAt <= DateTime.UtcNow.AddMinutes(-15))
                .Include(order => order.Tickets).ThenInclude(ticket => ticket.TicketType)
                .ToListAsync();
            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Expired;
                foreach (var ticket in order.Tickets)
                {
                    ticket.TicketType.SoldCount--;
                }
            }
            await orderRepository.SaveChangesAsync();
        });
    }
}
