using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
   

        public class Organizer : Person
        {
            public ORGANIZER(int id, string name, string contactEmail)
                : base(id, name, contactEmail) { }

           

            public override string GetSummary() => $"[Organizer #{Id}] {Name} <{Email}>";
        }

      
       

    }
