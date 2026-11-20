using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENT_MANAGER
{
   public interface IEntity
    {

        int Id { get; }
        string GetSummary();
    }
}
