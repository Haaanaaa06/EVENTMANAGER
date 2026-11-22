using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
    
        public class EventManager
        {
            private readonly List<Organizer> _organizers = new List<Organizer>();
            private readonly List<VENUE> _venues = new List<VENUE>();
            private readonly List<Event> _events = new List<Event>();
            private readonly List<ATTENDEE> _attendees = new List<ATTENDEE>();
            private readonly List<Ticket> _tickets = new List<Ticket>();

            private readonly Random _random = new Random();
            private readonly HashSet<int> _usedIds = new HashSet<int>();


            private int GenerateId()
            {
                int id;
                do
                {
                    id = _random.Next(100000, 1000000);
                } while (!_usedIds.Add(id));
                return id;
            }

            public IReadOnlyList<Organizer> Organizers => _organizers;
            public IReadOnlyList<VENUE> Venues => _venues;
            public IReadOnlyList<Event> Events => _events;
            public IReadOnlyList<ATTENDEE> Attendees => _attendees;
            public IReadOnlyList<Ticket> Tickets => _tickets;

            public Organizer AddOrganizer(string name, string email)
            {
                var o = new Organizer(GenerateId(), name, email);
                _organizers.Add(o);
                return o;
            }

            public VENUE AddVenue(string name, string location, int capacity)
            {
                var v = new VENUE(GenerateId(), name, location, capacity);
                _venues.Add(v);
                return v;
            }

            public Event CreateEvent(string title, int organizerId, int venueId, DateTime date)
            {
                if (date.Date < DateTime.Today)
                    throw new InvalidOperationException("An event cannot be created on a past date.");
                if (!_organizers.Any(o => o.Id == organizerId))
                    throw new InvalidOperationException($"Organizer #{organizerId} does not exist.");
                if (!_venues.Any(v => v.Id == venueId))
                    throw new InvalidOperationException($"Venue #{venueId} does not exist.");
                if (_events.Any(e => e.VenueId == venueId && e.EventDate.Date == date.Date))
                    throw new InvalidOperationException("That venue is already booked on this date.");

                var ev = new Event(GenerateId(), title, organizerId, venueId, date);
                _events.Add(ev);
                return ev;
            }

            public ATTENDEE RegisterAttendee(string name, string email)
            {
                if (_attendees.Any(X => X.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("An attendee with this email is already registered.");

                var a = new ATTENDEE(GenerateId(), name, email);
                _attendees.Add(a);
                return a;
            }

            public Ticket SellTicket(int eventId, int attendeeId, decimal price)
            {
                var ev = FindEvent(eventId);
                var venue = _venues.First(v => v.Id == ev.VenueId);

                if (ev.EventDate.Date < DateTime.Today)
                    throw new InvalidOperationException("Cannot sell tickets: this event has already taken place.");
                if (!_attendees.Any(a => a.Id == attendeeId))
                    throw new InvalidOperationException($"Attendee #{attendeeId} does not exist.");
                if (_tickets.Any(t => t.EventId == eventId && t.AttendeeId == attendeeId))
                    throw new InvalidOperationException("This attendee already has a ticket for the event.");
                if (_tickets.Count(t => t.EventId == eventId) >= venue.Capacity)
                    throw new InvalidOperationException("Sold out: venue capacity reached.");

                var ticket = new Ticket(GenerateId(), eventId, attendeeId, price);
                _tickets.Add(ticket);
                return ticket;
            }

            public Ticket CheckInTicket(int ticketId)
            {
                var ticket = _tickets.FirstOrDefault(t => t.Id == ticketId)
                             ?? throw new InvalidOperationException($"Ticket #{ticketId} does not exist.");
                ticket.CheckIn();
                return ticket;
            }

            public Event FindEvent(int eventId) =>
                _events.FirstOrDefault(e => e.Id == eventId)
                ?? throw new InvalidOperationException($"Event #{eventId} does not exist.");

            public string BuildEventReport(int eventId)
            {
                var ev = FindEvent(eventId);
                var org = _organizers.First(o => o.Id == ev.OrganizerId);
                var venue = _venues.First(v => v.Id == ev.VenueId);
                var tickets = _tickets.Where(t => t.EventId == eventId).ToList();

                var lines = new List<string>
            {
                "----- EVENT REPORT -----",
                $"Title      : {ev.Title}",
                $"Date       : {ev.EventDate:yyyy-MM-dd}",
                $"Organizer  : {org.Name} ({org.Email})",
                $"Venue      : {venue.Name}, {venue.Location}",
                $"Tickets    : {tickets.Count} / {venue.Capacity} sold",
                $"Checked in : {tickets.Count(t => t.CheckedIn)}",
                $"Revenue    : {tickets.Sum(t => t.Price):C}",
                "Attendees:"
            };

                if (tickets.Count == 0)
                    lines.Add("  (none)");

                foreach (var t in tickets)
                {
                    var a = _attendees.First(x => x.Id == t.AttendeeId);
                    lines.Add($"  - {a.Name} | Ticket #{t.Id} | {(t.CheckedIn ? "Checked in" : "Not checked in")}");
                }
                return string.Join(Environment.NewLine, lines);
            }

            public void SeedDemoData()
            {
                var o1 = AddOrganizer("Tech Events Co.", "info@techevents.com");
                var o2 = AddOrganizer("Sara Ahmed", "sara@example.com");
                var v1 = AddVenue("Cairo Conference Hall", "Nasr City, Cairo", 3);
                var v2 = AddVenue("Open Air Theatre", "Zamalek, Cairo", 200);
                var e1 = CreateEvent("C# Developer Summit", o1.Id, v1.Id, DateTime.Today.AddDays(30));
                CreateEvent("Summer Music Night", o2.Id, v2.Id, DateTime.Today.AddDays(45));
                var a1 = RegisterAttendee("Omar Hassan", "omar@example.com");
                var a2 = RegisterAttendee("Mona Ali", "mona@example.com");
                SellTicket(e1.Id, a1.Id, 150m);
                SellTicket(e1.Id, a2.Id, 150m);
            }
        }
    }
