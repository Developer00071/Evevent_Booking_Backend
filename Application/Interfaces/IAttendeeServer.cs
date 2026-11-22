using Application.DTOs;

namespace Application.Interfaces;

public interface IAttendeeServer
{
    Task<AttendeeDto> AddAttendee(AttendeeCreateDto dto);
    Task<List<AttendeeDto>> GetAttendees();
    Task UpdateAttendeeProfile(int id, AttendeeProfileDto dto);
    Task<List<OrderDto>> GetAttendeeOrders(int id);
    Task<List<TicketDto>> GetAttendeeTickets(int id);
}
