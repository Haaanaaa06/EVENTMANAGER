using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace EVENT_MANAGER
    {
        public static class Program
        {
            private static readonly EventManager Manager = new EventManager();
            static void Main(string[] args)
            {
                while (true)
                {
                    ShowMenu();
                    string choice = Prompt("Choose an option");
                    Console.WriteLine();

                    if (choice == "0")
                    {
                        Console.WriteLine("Goodbye!");
                        return;
                    }

                    try
                    {
                        HandleChoice(choice);
                    }
                    catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    if (choice == "11") continue;   // the reports menu handles its own pauses

                    Console.WriteLine();
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            private static void ShowMenu()
            {
                Console.Clear();
                Console.WriteLine("=========== EVENT MANAGEMENT SYSTEM ===========");
                Console.WriteLine(" 1. Add organizer");
                Console.WriteLine(" 2. Add venue");
                Console.WriteLine(" 3. Create event");
                Console.WriteLine(" 4. Register attendee");
                Console.WriteLine(" 5. Sell ticket");
                Console.WriteLine(" 6. Check in ticket");
                Console.WriteLine(" 7. List organizers / venues / events");
                Console.WriteLine(" 8. List attendees / tickets");
                Console.WriteLine(" 9. Event report");
                Console.WriteLine(" 10. Load demo data");
                Console.WriteLine(" 11. LINQ reports");
                Console.WriteLine(" 0. Exit");
                Console.WriteLine("===============================================");
            }

            private static void HandleChoice(string choice)
            {
                switch (choice)
                {
                    case "1":
                        var o = Manager.AddOrganizer(Prompt("Name"), Prompt("Contact email"));
                        Console.WriteLine("Added: " + o.GetSummary());
                        break;

                    case "2":
                        var v = Manager.AddVenue(Prompt("Name"), Prompt("Location"), PromptInt("Capacity"));
                        Console.WriteLine("Added: " + v.GetSummary());
                        break;

                    case "3":
                        PrintAll(Manager.Organizers);
                        PrintAll(Manager.Venues);
                        var ev = Manager.CreateEvent(
                            Prompt("Title"),
                            PromptInt("Organizer Id"),
                            PromptInt("Venue Id"),
                            PromptDate("Event date (yyyy-MM-dd)"));
                        Console.WriteLine("Created: " + ev.GetSummary());
                        break;

                    case "4":
                        var a = Manager.RegisterAttendee(Prompt("Name"), Prompt("Email"));
                        Console.WriteLine("Registered: " + a.GetSummary());
                        break;

                    case "5":
                        PrintAll(Manager.Events);
                        PrintAll(Manager.Attendees);
                        var t = Manager.SellTicket(
                            PromptInt("Event Id"),
                            PromptInt("Attendee Id"),
                            PromptDecimal("Price"));
                        Console.WriteLine("Sold: " + t.GetSummary());
                        break;

                    case "6":
                        PrintAll(Manager.Tickets);
                        var checkedIn = Manager.CheckInTicket(PromptInt("Ticket Id"));
                        Console.WriteLine("Checked in: " + checkedIn.GetSummary());
                        break;

                    case "7":
                        Console.WriteLine("--- Organizers ---");
                        PrintAll(Manager.Organizers);
                        Console.WriteLine("--- Venues ---");
                        PrintAll(Manager.Venues);
                        Console.WriteLine("--- Events ---");
                        PrintAll(Manager.Events);
                        break;

                    case "8":
                        Console.WriteLine("--- Attendees ---");
                        PrintAll(Manager.Attendees);
                        Console.WriteLine("--- Tickets ---");
                        PrintAll(Manager.Tickets);
                        break;

                    case "9":
                        PrintAll(Manager.Events);
                        Console.WriteLine(Manager.BuildEventReport(PromptInt("Event Id")));
                        break;

                    case "10":
                        if (Manager.Events.Count > 0)
                            throw new InvalidOperationException("Demo data can only be loaded into an empty system.");
                        Manager.SeedDemoData();
                        Console.WriteLine("Demo data loaded.");
                        break;

                    case "11":
                        QueryMenu.Run(Manager);
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }

            // Polymorphism through the IEntity interface
            private static void PrintAll(IEnumerable<IEntity> items)
            {
                var list = items.ToList();
                if (list.Count == 0)
                {
                    Console.WriteLine("  (none)");
                    return;
                }
                foreach (var item in list)
                    Console.WriteLine("  " + item.GetSummary());
            }

            // ---------- Input helpers ----------
            private static string Prompt(string label)
            {
                Console.Write($"{label}: ");
                return (Console.ReadLine() ?? string.Empty).Trim();
            }

            private static int PromptInt(string label)
            {
                if (!int.TryParse(Prompt(label), out int value))
                    throw new ArgumentException("Please enter a valid whole number.");
                return value;
            }

            private static decimal PromptDecimal(string label)
            {
                if (!decimal.TryParse(Prompt(label), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                    throw new ArgumentException("Please enter a valid amount (e.g. 99.50).");
                return value;
            }

            private static DateTime PromptDate(string label)
            {
                if (!DateTime.TryParseExact(Prompt(label), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out DateTime value))
                    throw new ArgumentException("Please enter the date as yyyy-MM-dd.");
                return value;
            }
        }
    }
