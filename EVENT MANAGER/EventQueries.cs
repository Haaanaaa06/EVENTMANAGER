using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace EVENT_MANAGER
    {

        public class EventStat
        {
            public Event Event { get; set; }
            public VENUE Venue { get; set; }
            public int TicketsSold { get; set; }
            public int CheckedIn { get; set; }
            public decimal Revenue { get; set; }
            public bool IsFullyBooked { get { return TicketsSold >= Venue.Capacity; } }
        }

        public class AttendeeEventRow
        {
            public ATTENDEE Attendee { get; set; }
            public Event Event { get; set; }
            public Ticket Ticket { get; set; }
        }

        public class EventDetail
        {
            public Event Event { get; set; }
            public ORGANIZER Organizer { get; set; }
            public VENUE Venue { get; set; }
        }

        public class MultiEventAttendee
        {
            public ATTENDEE Attendee { get; set; }
            public List<Event> Events { get; set; }
        }

        
        public class EventQueries
        {
            private readonly EventManager _m;

            public EventQueries(EventManager manager)
            {
                if (manager == null) throw new ArgumentNullException("manager");
                _m = manager;
            }
        // Get event statistics
        private List<EventStat> GetStats()
        {
            List<EventStat> stats = new List<EventStat>();

            foreach (var e in _m.Events)
            {
                var venue = _m.Venues.FirstOrDefault(v => v.Id == e.VenueId);

                if (venue == null)
                    continue;

                var tickets = _m.Tickets.Where(t => t.EventId == e.Id).ToList();

                EventStat stat = new EventStat
                {
                    Event = e,
                    Venue = venue,
                    TicketsSold = tickets.Count,
                    CheckedIn = tickets.Count(t => t.CheckedIn),
                    Revenue = tickets.Sum(t => t.Price)
                };

                stats.Add(stat);
            }

            return stats;
        }


        // 1. All events organized by a specific organizer
        public IEnumerable<Event> GetEventsByOrganizer(int organizerId)
            {
                if (!_m.Organizers.Any(o => o.Id == organizerId))
                    throw new InvalidOperationException("Organizer #" + organizerId + " does not exist.");

                return _m.Events
                         .Where(e => e.OrganizerId == organizerId)
                         .OrderBy(e => e.EventDate);
            }

        // 2. All attendees for a specific event
        public IEnumerable<AttendeeEventRow> GetAttendeesForEvent(int eventId)
        {
            return _m.Tickets
                .Where(t => t.EventId == eventId)
                .Select(t => new AttendeeEventRow
                {
                    Ticket = t,
                    Event = _m.FindEvent(t.EventId),
                    Attendee = _m.Attendees.First(a => a.Id == t.AttendeeId)
                });
        }

        // 3. Total ticket revenue per event (highest first)
        public IEnumerable<EventStat> GetRevenuePerEvent()
            {
                return GetStats().OrderByDescending(s => s.Revenue);
            }

            // 4. Number of attendees per event (most first)
            public IEnumerable<EventStat> GetAttendeeCountPerEvent()
            {
                return GetStats().OrderByDescending(s => s.TicketsSold)
                                   .ThenBy(s => s.Event.Title);
            }

            // 5. Upcoming events sorted by date
            public IEnumerable<Event> GetUpcomingEvents()
            {
                return _m.Events
                         .Where(e => e.EventDate.Date >= DateTime.Today)
                         .OrderBy(e => e.EventDate);
            }

        // 6. Attendees who did not check in (only for events happening today or already past)
        public IEnumerable<AttendeeEventRow> GetAttendeesNotCheckedIn()
        {
            return _m.Tickets
                .Where(t => !t.CheckedIn)
                .Where(t => _m.Events.Any(e =>
                    e.Id == t.EventId &&
                    e.EventDate.Date <= DateTime.Today))
                .Select(t => new AttendeeEventRow
                {
                    Ticket = t,
                    Attendee = _m.Attendees.First(a => a.Id == t.AttendeeId),
                    Event = _m.FindEvent(t.EventId)
                });
        }
        // 7a. Most attended events
        public IEnumerable<EventStat> GetMostAttendedEvents(int top)
            {
                return GetStats().OrderByDescending(s => s.TicketsSold)
                                   .ThenBy(s => s.Event.Title)
                                   .Take(top);
            }

            // 7b. Top revenue-generating events
            public IEnumerable<EventStat> GetTopRevenueEvents(int top)
            {
                return GetStats().OrderByDescending(s => s.Revenue)
                                   .ThenBy(s => s.Event.Title)
                                   .Take(top);
            }

        // 8. Event with organizer and venue info
        public IEnumerable<EventDetail> GetEventDetails()
        {
            return _m.Events
                .OrderBy(e => e.EventDate)
                .Select(e => new EventDetail
                {
                    Event = e,
                    Organizer = _m.Organizers.First(o =>
                        o.Id == e.OrganizerId),
                    Venue = _m.Venues.First(v =>
                        v.Id == e.VenueId)
                });
        }

        // 9. Attendees attending multiple events

        public IEnumerable<MultiEventAttendee> GetAttendeesInMultipleEvents()
        {
            return _m.Tickets
                .GroupBy(t => t.AttendeeId)
                .Where(g => g.Select(t => t.EventId)
                              .Distinct()
                              .Count() > 1)
                .Select(g => new MultiEventAttendee
                {
                    Attendee = _m.Attendees.First(a => a.Id == g.Key),
                    Events = g.Select(t => _m.FindEvent(t.EventId))
                              .Distinct()
                              .ToList()
                });
        }

        // 10. Fully booked events
        public IEnumerable<EventStat> GetFullyBookedEvents()
            {
                return GetStats().Where(s => s.IsFullyBooked)
                                   .OrderBy(s => s.Event.EventDate);
            }
        }

        
        public static class QueryMenu
        {
            public static void Run(EventManager manager)
            {
                var queries = new EventQueries(manager);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=============== LINQ REPORTS ===============");
                    Console.WriteLine(" 1. Events by organizer");
                    Console.WriteLine(" 2. Attendees for an event");
                    Console.WriteLine(" 3. Total revenue per event");
                    Console.WriteLine(" 4. Number of attendees per event");
                    Console.WriteLine(" 5. Upcoming events (sorted by date)");
                    Console.WriteLine(" 6. Attendees who didn't check in");
                    Console.WriteLine(" 7. Top events (most attended / top revenue)");
                    Console.WriteLine(" 8. Events with organizer and venue info");
                    Console.WriteLine(" 9. Attendees attending multiple events");
                    Console.WriteLine("10. Fully booked events");
                    Console.WriteLine(" 0. Back");
                    Console.WriteLine("============================================");

                    string choice = Prompt("Choose a report");
                    if (choice == "0") return;

                    Console.WriteLine();
                    try
                    {
                        Handle(choice, manager, queries);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    Console.WriteLine();
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            private static void Handle(string choice, EventManager manager, EventQueries q)
            {
                switch (choice)
                {
                    case "1":
                        PrintList(manager.Organizers.Cast<IEntity>(), x => x.GetSummary());
                        int orgId = PromptInt("Organizer Id");
                        PrintList(q.GetEventsByOrganizer(orgId), e => e.GetSummary());
                        break;

                    case "2":
                        PrintList(manager.Events.Cast<IEntity>(), x => x.GetSummary());
                        int evId = PromptInt("Event Id");
                        PrintList(q.GetAttendeesForEvent(evId),
                            r => r.Attendee.Name + " <" + r.Attendee.Email + "> | Ticket #" + r.Ticket.Id);
                        break;

                    case "3":
                        PrintList(q.GetRevenuePerEvent(),
                            s => PadTitle(s) + " revenue: " + s.Revenue.ToString("C"));
                        break;

                    case "4":
                        PrintList(q.GetAttendeeCountPerEvent(),
                            s => PadTitle(s) + " attendees: " + s.TicketsSold);
                        break;

                    case "5":
                        PrintList(q.GetUpcomingEvents(), e => e.GetSummary());
                        break;

                    case "6":
                        PrintList(q.GetAttendeesNotCheckedIn(),
                            r => r.Attendee.Name + " | " + r.Event.Title + " (" +
                                 r.Event.EventDate.ToString("yyyy-MM-dd") + ") | Ticket #" + r.Ticket.Id);
                        break;

                    case "7":
                        Console.WriteLine("--- Top 3 most attended ---");
                        PrintList(q.GetMostAttendedEvents(3),
                            s => PadTitle(s) + " attendees: " + s.TicketsSold);
                        Console.WriteLine("--- Top 3 by revenue ---");
                        PrintList(q.GetTopRevenueEvents(3),
                            s => PadTitle(s) + " revenue: " + s.Revenue.ToString("C"));
                        break;

                    case "8":
                        PrintList(q.GetEventDetails(),
                            d => d.Event.Title + " | " + d.Event.EventDate.ToString("yyyy-MM-dd") +
                                 "\n      Organizer: " + d.Organizer.Name + " (" + d.Organizer.Email + ")" +
                                 "\n      Venue    : " + d.Venue.Name + ", " + d.Venue.Location +
                                 " (capacity " + d.Venue.Capacity + ")");
                        break;

                    case "9":
                        PrintList(q.GetAttendeesInMultipleEvents(),
                            m => m.Attendee.Name + " -> " + m.Events.Count + " events: " +
                                 string.Join(", ", m.Events.Select(e => e.Title)));
                        break;

                    case "10":
                        PrintList(q.GetFullyBookedEvents(),
                            s => PadTitle(s) + " sold " + s.TicketsSold + "/" + s.Venue.Capacity);
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }

         
            private static string PadTitle(EventStat s)
            {
                return s.Event.Title.PadRight(28);
            }

            private static void PrintList<T>(IEnumerable<T> items, Func<T, string> format)
            {
                var list = items.ToList();
                if (list.Count == 0)
                {
                    Console.WriteLine("  (no results)");
                    return;
                }
                foreach (T item in list)
                    Console.WriteLine("  " + format(item));
            }

            private static string Prompt(string label)
            {
                Console.Write(label + ": ");
                return (Console.ReadLine() ?? string.Empty).Trim();
            }

            private static int PromptInt(string label)
            {
                int value;
                if (!int.TryParse(Prompt(label), out value))
                    throw new ArgumentException("Please enter a valid whole number.");
                return value;
            }
        }
    }


