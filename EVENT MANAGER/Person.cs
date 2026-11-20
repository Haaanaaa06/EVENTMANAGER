using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
   public abstract class Person:IEntity
    {
        private string _name = string.Empty;
        private string _email = string.Empty;

        public int Id { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value.Trim();
            }
        }


        public string Email
        {
            get => _email;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('@') || !value.Contains('.'))
                    throw new ArgumentException("A valid email address is required.");
                _email = value.Trim();
            }
        }
        protected Person(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public abstract string GetSummary();
    }

  
}

