using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Event
{
    public class TeamServiceCacheEventHandler : IEventHandler<TeamEvent>
    {
        public void Handle(TeamEvent evt)
        {
            Console.WriteLine($"成员信息条件到缓存成功：{evt.Id}");
        }
    }
}
