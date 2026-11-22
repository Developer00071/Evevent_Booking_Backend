using System.Net;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AttendeeService(IAttendeeRepository attendeeRepository, IOrderRepository orderRepository, ITicketRepository ticketRepository) : IAttendeeServer
{
    public async Task<AttendeeDto> AddAttendee(AttendeeCreateDto dto)
    {
        var emailExists = await attendeeRepository.GetQuery().AnyAsync(attendee => attendee.Email == dto.Email);
        if (emailExists)
        {
            throw new ApiException((int)HttpStatusCode.Conflict, "Email is already registered.");
        }

        var attendee = new Attendee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            RegisteredAt = DateTime.UtcNow
        };
        await attendeeRepository.AddAttendee(attendee);
        await attendeeRepository.SaveChangesAsync();
        return EventBookingMapper.MapAttendee(attendee);
    }

    public async Task<List<AttendeeDto>> GetAttendees()
    {
        return await attendeeRepository.GetQuery().AsNoTracking()
            .OrderBy(attendee => attendee.Id)
            .Select(attendee => new AttendeeDto
            {
                Id = attendee.Id,
                FullName = attendee.FullName,
                Email = attendee.Email,
                RegisteredAt = attendee.RegisteredAt
            }).ToListAsync();
    }

    public async Task UpdateAttendeeProfile(int id, AttendeeProfileDto dto)
    {
        var attendee = await attendeeRepository.GetQuery().Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException((int)HttpStatusCode.NotFound, "Attendee not found.");
        if (attendee.Profile is null)
        {
            attendee.Profile = new AttendeeProfile { AttendeeId = id };
        }

        attendee.Profile.DateOfBirth = dto.DateOfBirth;
        attendee.Profile.City = dto.City;
        attendee.Profile.Bio = dto.Bio;
        await attendeeRepository.SaveChangesAsync();
    }

    public async Task<List<OrderDto>> GetAttendeeOrders(int id)
    {
        var exists = await attendeeRepository.GetQuery().AnyAsync(attendee => attendee.Id == id);
        if (!exists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Attendee not found.");
        }

        return await orderRepository.GetQuery().AsNoTracking().Where(order => order.AttendeeId == id)
            .OrderByDescending(order => order.CreatedAt)
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
            }).ToListAsync();
    }

    public async Task<List<TicketDto>> GetAttendeeTickets(int id)
    {
        var exists = await attendeeRepository.GetQuery().AnyAsync(attendee => attendee.Id == id);
        if (!exists)
        {
            throw new ApiException((int)HttpStatusCode.NotFound, "Attendee not found.");
        }

        return await ticketRepository.GetQuery().AsNoTracking()
            .Where(ticket => ticket.AttendeeId == id)
            .Select(ticket => new TicketDto
            {
                Code = ticket.Code,
                EventTitle = ticket.TicketType.Event.Title,
                TicketTypeName = ticket.TicketType.Name,
                Price = ticket.Price,
                IsCheckedIn = ticket.IsCheckedIn
            })
            .ToListAsync();
    }
}
