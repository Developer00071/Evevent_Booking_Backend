using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(venue => venue.Id);
        builder.HasMany(venue => venue.Events)
            .WithOne(eventEntity => eventEntity.Venue)
            .HasForeignKey(eventEntity => eventEntity.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasData(
            new Venue { Id = 1, Name = "Main Hall", Address = "1 Main Street", City = "New York", Capacity = 500 },
            new Venue { Id = 2, Name = "City Center", Address = "2 Center Street", City = "Boston", Capacity = 300 });
    }
}

public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
{
    public void Configure(EntityTypeBuilder<Organizer> builder)
    {
        builder.HasKey(organizer => organizer.Id);
        builder.HasMany(organizer => organizer.Events)
            .WithOne(eventEntity => eventEntity.Organizer)
            .HasForeignKey(eventEntity => eventEntity.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);
        builder.HasIndex(category => category.Name).IsUnique();
        builder.HasData(
            new Category { Id = 1, Name = "Music" },
            new Category { Id = 2, Name = "Technology" },
            new Category { Id = 3, Name = "Business" },
            new Category { Id = 4, Name = "Art" },
            new Category { Id = 5, Name = "Sports" });
    }
}

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(eventEntity => eventEntity.Id);
        builder.Property(eventEntity => eventEntity.Status).HasConversion<string>();
        builder.HasIndex(eventEntity => new { eventEntity.Status, eventEntity.StartDate });
        builder.HasQueryFilter(eventEntity => !eventEntity.IsDeleted);
        builder.ToTable(table => table.HasCheckConstraint("CK_Events_EndDate", "\"EndDate\" > \"StartDate\""));
        builder.HasOne(eventEntity => eventEntity.Venue)
            .WithMany(venue => venue.Events)
            .HasForeignKey(eventEntity => eventEntity.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(eventEntity => eventEntity.Organizer)
            .WithMany(organizer => organizer.Events)
            .HasForeignKey(eventEntity => eventEntity.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(eventEntity => eventEntity.Categories)
            .WithMany(category => category.Events)
            .UsingEntity(join => join.ToTable("EventCategories"));
        builder.HasMany(eventEntity => eventEntity.TicketTypes)
            .WithOne(ticketType => ticketType.Event)
            .HasForeignKey(ticketType => ticketType.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(eventEntity => eventEntity.Reviews)
            .WithOne(review => review.Event)
            .HasForeignKey(review => review.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.HasKey(ticketType => ticketType.Id);
        builder.Property(ticketType => ticketType.Price).HasPrecision(10, 2);
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_TicketTypes_Quantity", "\"Quantity\" >= 0");
            table.HasCheckConstraint("CK_TicketTypes_SoldCount", "\"SoldCount\" <= \"Quantity\"");
            table.HasCheckConstraint("CK_TicketTypes_SoldCountNonNegative", "\"SoldCount\" >= 0");
        });
        builder.HasOne(ticketType => ticketType.Event)
            .WithMany(eventEntity => eventEntity.TicketTypes)
            .HasForeignKey(ticketType => ticketType.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
{
    public void Configure(EntityTypeBuilder<Attendee> builder)
    {
        builder.HasKey(attendee => attendee.Id);
        builder.HasIndex(attendee => attendee.Email).IsUnique();
        builder.HasOne(attendee => attendee.Profile)
            .WithOne(profile => profile.Attendee)
            .HasForeignKey<AttendeeProfile>(profile => profile.AttendeeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(attendee => attendee.Orders)
            .WithOne(order => order.Attendee)
            .HasForeignKey(order => order.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(attendee => attendee.Tickets)
            .WithOne(ticket => ticket.Attendee)
            .HasForeignKey(ticket => ticket.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(attendee => attendee.Reviews)
            .WithOne(review => review.Attendee)
            .HasForeignKey(review => review.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AttendeeProfileConfiguration : IEntityTypeConfiguration<AttendeeProfile>
{
    public void Configure(EntityTypeBuilder<AttendeeProfile> builder)
    {
        builder.HasKey(profile => profile.AttendeeId);
        builder.HasOne(profile => profile.Attendee)
            .WithOne(attendee => attendee.Profile)
            .HasForeignKey<AttendeeProfile>(profile => profile.AttendeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Status).HasConversion<string>();
        builder.Property(order => order.TotalAmount).HasPrecision(12, 2);
        builder.HasMany(order => order.Tickets)
            .WithOne(ticket => ticket.Order)
            .HasForeignKey(ticket => ticket.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(ticket => ticket.Id);
        builder.HasIndex(ticket => ticket.Code).IsUnique();
        builder.Property(ticket => ticket.Price).HasPrecision(10, 2);
        builder.HasOne(ticket => ticket.Order)
            .WithMany(order => order.Tickets)
            .HasForeignKey(ticket => ticket.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ticket => ticket.TicketType)
            .WithMany(ticketType => ticketType.Tickets)
            .HasForeignKey(ticket => ticket.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(review => review.Id);
        builder.HasIndex(review => new { review.EventId, review.AttendeeId }).IsUnique();
        builder.HasOne(review => review.Event)
            .WithMany(eventEntity => eventEntity.Reviews)
            .HasForeignKey(review => review.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(review => review.Attendee)
            .WithMany(attendee => attendee.Reviews)
            .HasForeignKey(review => review.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
