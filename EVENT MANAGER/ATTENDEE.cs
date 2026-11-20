using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{



    public class ATTENDEE:Person
    {
        public ATTENDEE(int id, string name, string email)
           : base(id, name, email) { }

      

        public override string GetSummary() => $"[Attendee #{Id}] {Name} <{Email}>";

    }
}