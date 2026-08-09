using EventHub.Application.Services;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using EventHub.Domain.Exceptions;

namespace EventHub.Presentation.Menus;

public sealed class EventHubConsole
{
    private readonly EventService _eventService;
    private readonly BookingService _bookingService;
    private readonly ReportService _reportService;

    private readonly Customer _customer;
    private readonly Organizer _organizer;

    public EventHubConsole(
        EventService eventService,
        BookingService bookingService,
        ReportService reportService,
        Customer customer,
        Organizer organizer)
    {
        _eventService = eventService;
        _bookingService = bookingService;
        _reportService = reportService;
        _customer = customer;
        _organizer = organizer;
    }

    public void Run()
    {
        var running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine("            EVENTHUB");
            Console.WriteLine("==================================");
            Console.WriteLine("1. Browse events");
            Console.WriteLine("2. Search events");
            Console.WriteLine("3. Customer menu");
            Console.WriteLine("4. Organizer menu");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            Console.Write("Select option: ");
            var input = Console.ReadLine();

            try
            {
                switch (input)
                {
                    case "1":
                        BrowseEvents();
                        break;

                    case "2":
                        SearchEvents();
                        break;

                    case "3":
                        CustomerMenu();
                        break;

                    case "4":
                        OrganizerMenu();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        ShowMessage("Invalid option.");
                        break;
                }
            }
            catch (EventHubException exception)
            {
                ShowMessage($"Error: {exception.Message}");
            }
            catch (Exception exception)
            {
                ShowMessage(
                    $"Unexpected error: {exception.Message}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Thank you for using EventHub.");
    }

    private void BrowseEvents()
    {
        Console.Clear();
        Console.WriteLine("AVAILABLE EVENTS");
        Console.WriteLine("================");

        var events = _eventService.GetPublicEvents();

        DisplayEvents(events);

        Pause();
    }

    private void SearchEvents()
    {
        Console.Clear();
        Console.WriteLine("SEARCH EVENTS");
        Console.WriteLine("=============");

        Console.Write("Search term (leave empty for all): ");
        var searchTerm = Console.ReadLine();

        Console.Write("City (leave empty for all): ");
        var city = Console.ReadLine();

        var events = _eventService.SearchEvents(
            searchTerm,
            city: city);

        DisplayEvents(events);

        Pause();
    }

    private void CustomerMenu()
    {
        var running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine("CUSTOMER MENU");
            Console.WriteLine("=============");
            Console.WriteLine($"Customer: {_customer.FullName}");
            Console.WriteLine();
            Console.WriteLine("1. Browse events");
            Console.WriteLine("2. Book event");
            Console.WriteLine("3. View my bookings");
            Console.WriteLine("4. Cancel booking");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    BrowseEvents();
                    break;

                case "2":
                    CreateBooking();
                    break;

                case "3":
                    ViewBookings();
                    break;

                case "4":
                    CancelBooking();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    ShowMessage("Invalid option.");
                    break;
            }
        }
    }

    private void OrganizerMenu()
    {
        var running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine("ORGANIZER MENU");
            Console.WriteLine("==============");
            Console.WriteLine($"Organizer: {_organizer.FullName}");
            Console.WriteLine();
            Console.WriteLine("1. Create event");
            Console.WriteLine("2. Publish event");
            Console.WriteLine("3. View my events");
            Console.WriteLine("4. Cancel event");
            Console.WriteLine("5. Event report");
            Console.WriteLine("0. Back");
            Console.WriteLine();

            Console.Write("Select option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    CreateEvent();
                    break;

                case "2":
                    PublishEvent();
                    break;

                case "3":
                    ViewOrganizerEvents();
                    break;

                case "4":
                    CancelEvent();
                    break;

                case "5":
                    ShowEventReport();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    ShowMessage("Invalid option.");
                    break;
            }
        }
    }

    private void CreateBooking()
    {
        var events = _eventService.GetPublicEvents();

        Console.Clear();

        DisplayEvents(events);

        Console.WriteLine();
        Console.Write("Enter event ID: ");

        if (!Guid.TryParse(
                Console.ReadLine(),
                out var eventId))
        {
            ShowMessage("Invalid event ID.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Ticket type:");
        Console.WriteLine("1. Standard");
        Console.WriteLine("2. VIP");
        Console.WriteLine("3. Student");

        Console.Write("Select ticket type: ");

        var ticketType = Console.ReadLine() switch
        {
            "1" => TicketType.Standard,
            "2" => TicketType.Vip,
            "3" => TicketType.Student,
            _ => throw new ValidationException(
                "Invalid ticket type.")
        };

        Console.Write("Quantity: ");

        if (!int.TryParse(
                Console.ReadLine(),
                out var quantity))
        {
            ShowMessage("Invalid quantity.");
            return;
        }

        var booking = _bookingService.CreateBooking(
            _customer.Id,
            eventId,
            ticketType,
            quantity);

        ShowMessage(
            $"Booking confirmed. Total price: {booking.TotalPrice:F2} KM");
    }

    private void ViewBookings()
    {
        Console.Clear();

        Console.WriteLine("MY BOOKINGS");
        Console.WriteLine("===========");

        var bookings =
            _bookingService.GetCustomerBookings(
                _customer.Id);

        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
        }
        else
        {
            foreach (var booking in bookings)
            {
                Console.WriteLine(
                    $"ID: {booking.Id}");

                Console.WriteLine(
                    $"Event ID: {booking.EventId}");

                Console.WriteLine(
                    $"Ticket: {booking.Ticket.Type}");

                Console.WriteLine(
                    $"Quantity: {booking.Ticket.Quantity}");

                Console.WriteLine(
                    $"Total: {booking.TotalPrice:F2} KM");

                Console.WriteLine(
                    $"Status: {booking.Status}");

                Console.WriteLine("----------------------------------");
            }
        }

        Pause();
    }

    private void CancelBooking()
    {
        ViewBookings();

        Console.Write("Enter booking ID to cancel: ");

        if (!Guid.TryParse(
                Console.ReadLine(),
                out var bookingId))
        {
            ShowMessage("Invalid booking ID.");
            return;
        }

        _bookingService.CancelBooking(
            _customer.Id,
            bookingId);

        ShowMessage("Booking cancelled successfully.");
    }

    private void CreateEvent()
    {
        Console.Clear();

        Console.WriteLine("CREATE EVENT");
        Console.WriteLine("============");

        Console.WriteLine("Event type:");
        Console.WriteLine("1. Concert");
        Console.WriteLine("2. Conference");
        Console.WriteLine("3. Workshop");

        var eventType = Console.ReadLine() switch
        {
            "1" => EventType.Concert,
            "2" => EventType.Conference,
            "3" => EventType.Workshop,
            _ => throw new ValidationException(
                "Invalid event type.")
        };

        Console.Write("Title: ");
        var title = Console.ReadLine() ?? string.Empty;

        Console.Write("Description: ");
        var description =
            Console.ReadLine() ?? string.Empty;

        Console.Write("Days from today: ");

        if (!int.TryParse(
                Console.ReadLine(),
                out var days))
        {
            ShowMessage("Invalid number of days.");
            return;
        }

        Console.Write("Capacity: ");

        if (!int.TryParse(
                Console.ReadLine(),
                out var capacity))
        {
            ShowMessage("Invalid capacity.");
            return;
        }

        Console.Write("Base price: ");

        if (!decimal.TryParse(
                Console.ReadLine(),
                out var basePrice))
        {
            ShowMessage("Invalid price.");
            return;
        }

        Console.Write("Primary detail: ");
        var primaryDetail =
            Console.ReadLine() ?? string.Empty;

        Console.Write("Secondary detail: ");
        var secondaryDetail =
            Console.ReadLine() ?? string.Empty;

        SkillLevel? skillLevel = null;

        if (eventType == EventType.Workshop)
        {
            Console.WriteLine(
                "Skill level: 1 Beginner, 2 Intermediate, 3 Advanced");

            skillLevel = Console.ReadLine() switch
            {
                "1" => SkillLevel.Beginner,
                "2" => SkillLevel.Intermediate,
                "3" => SkillLevel.Advanced,
                _ => throw new ValidationException(
                    "Invalid skill level.")
            };
        }

        var venue = new Venue(
            "EventHub Venue",
            "Main Street 1",
            "Sarajevo",
            1000);

        var eventItem = _eventService.CreateEvent(
            _organizer.Id,
            eventType,
            title,
            description,
            DateTime.UtcNow.AddDays(days),
            venue,
            capacity,
            basePrice,
            primaryDetail,
            secondaryDetail,
            skillLevel);

        ShowMessage(
            $"Event created with ID: {eventItem.Id}");
    }

    private void PublishEvent()
    {
        ViewOrganizerEvents();

        Console.Write("Enter event ID to publish: ");

        if (!Guid.TryParse(
                Console.ReadLine(),
                out var eventId))
        {
            ShowMessage("Invalid event ID.");
            return;
        }

        _eventService.PublishEvent(
            _organizer.Id,
            eventId);

        ShowMessage("Event published successfully.");
    }

    private void CancelEvent()
    {
        ViewOrganizerEvents();

        Console.Write("Enter event ID to cancel: ");

        if (!Guid.TryParse(
                Console.ReadLine(),
                out var eventId))
        {
            ShowMessage("Invalid event ID.");
            return;
        }

        _eventService.CancelEvent(
            _organizer.Id,
            eventId);

        ShowMessage("Event cancelled successfully.");
    }

    private void ViewOrganizerEvents()
    {
        Console.Clear();

        Console.WriteLine("MY EVENTS");
        Console.WriteLine("=========");

        var events = _eventService
            .SearchEvents();

        var ownedEvents = events
            .Where(eventItem =>
                eventItem.OrganizerId == _organizer.Id)
            .ToList();

        DisplayEvents(ownedEvents);

        Console.WriteLine();
    }

    private void ShowEventReport()
    {
        ViewOrganizerEvents();

        Console.Write("Enter event ID: ");

        if (!Guid.TryParse(
                Console.ReadLine(),
                out var eventId))
        {
            ShowMessage("Invalid event ID.");
            return;
        }

        var report = _reportService.GenerateEventReport(
            _organizer.Id,
            eventId);

        Console.Clear();

        Console.WriteLine("EVENT REPORT");
        Console.WriteLine("============");
        Console.WriteLine($"Event: {report.EventTitle}");
        Console.WriteLine($"Capacity: {report.Capacity}");
        Console.WriteLine(
            $"Confirmed tickets: {report.ConfirmedTickets}");
        Console.WriteLine(
            $"Available seats: {report.AvailableSeats}");
        Console.WriteLine(
            $"Cancelled bookings: {report.CancelledBookings}");
        Console.WriteLine(
            $"Occupancy: {report.OccupancyRate:F2}%");
        Console.WriteLine(
            $"Revenue: {report.Revenue:F2} KM");

        Pause();
    }

    private static void DisplayEvents(
        IEnumerable<Event> events)
    {
        var eventList = events.ToList();

        if (eventList.Count == 0)
        {
            Console.WriteLine("No events found.");
            return;
        }

        foreach (var eventItem in eventList)
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"ID: {eventItem.Id}");
            Console.WriteLine(eventItem.GetEventDetails());
            Console.WriteLine(
                $"City: {eventItem.Venue.City}");
            Console.WriteLine(
                $"Date: {eventItem.StartDate:g}");
            Console.WriteLine(
                $"Base price: {eventItem.BasePrice:F2} KM");
            Console.WriteLine(
                $"Available seats: {eventItem.AvailableSeats}/{eventItem.Capacity}");
            Console.WriteLine(
                $"Status: {eventItem.Status}");
        }

        Console.WriteLine("----------------------------------");
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine(
            "Press Enter to continue...");

        Console.ReadLine();
    }
}