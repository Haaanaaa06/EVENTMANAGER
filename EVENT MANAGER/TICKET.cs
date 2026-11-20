using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
   

    public class Ticket:IEntity
    {
        private decimal _price;

        public int Id { get; }
        public int EventId { get; }
        public int AttendeeId { get; }
        public bool CheckedIn { get; private set; }   // can only change through CheckIn()

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative.");
                _price = value;
            }
        }

        public Ticket(int id, int eventId, int attendeeId, decimal price)
        {
            Id = id;
            EventId = eventId;
            AttendeeId = attendeeId;
            Price = price;
            CheckedIn = false;
        }

        public void CheckIn()
        {
            if (CheckedIn)
                throw new InvalidOperationException("This ticket has already been checked in.");
            CheckedIn = true;
        }

        public string GetSummary() =>
            $"[Ticket #{Id}] Event #{EventId} | Attendee #{AttendeeId} | {Price:C} | " +
            (CheckedIn ? "Checked in" : "Not checked in");
    }

}

