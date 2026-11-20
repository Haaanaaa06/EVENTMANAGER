using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
  

    public class Event:IEntity
    {
        private string _title = string.Empty;

        public int Id { get; }
        public int OrganizerId { get; }
        public int VenueId { get; }
        public DateTime EventDate { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Event title cannot be empty.");
                _title = value.Trim();
            }
        }

        public Event(int id, string title, int organizerId, int venueId, DateTime eventDate)
        {
            Id = id;
            Title = title;
            OrganizerId = organizerId;
            VenueId = venueId;
            EventDate = eventDate;
        }

        public string GetSummary() =>
            $"[Event #{Id}] {Title} | {EventDate:yyyy-MM-dd} | Organizer #{OrganizerId} | Venue #{VenueId}";
    }

}

