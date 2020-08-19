using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Event
{
    public class TeamEvent:IEvent
    {
        public int Id { get; set; }
    }
}
