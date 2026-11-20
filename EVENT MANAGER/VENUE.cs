using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
    
    public class VENUE: IEntity
    {
        private string _name = string.Empty;
        private string _location = string.Empty;
        private int _capacity;

        public int Id { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Venue name cannot be empty.");
                _name = value.Trim();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Location cannot be empty.");
                _location = value.Trim();
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Capacity must be greater than zero.");
                _capacity = value;
            }
        }

        public VENUE(int id, string name, string location, int capacity)
        {
            Id = id;
            Name = name;
            Location = location;
            Capacity = capacity;
        }

        public string GetSummary() => $"[Venue #{Id}] {Name}, {Location} (Capacity: {Capacity})";
    }


}


