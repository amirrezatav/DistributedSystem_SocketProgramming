using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Shared.Models
{
    public class PacketResult
    {
        private DispatcherTimer timer;

        public PacketResult(EventHandler Timer_Tick)
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(30); // Set the timer interval (e.g., 1 second)
        }

        public long RequestId { get; set; }
        public long ClientId { get; set; }
        public int AnsNumber { get; set; } = 0;
        public List<Person> People { get; set; } = new();

    }
}
