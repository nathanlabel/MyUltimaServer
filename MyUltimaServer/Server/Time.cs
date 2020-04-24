using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace MyUltimaServer.Server
{
    public enum TimeCycle
    {
        Dawn,
        Day,
        Dusk,
        Night
    }
    public class Time
    {
        private static Timer m_Time;
        public double TimePeriod { get; private set; } // Time period is the number of minutes per game "Cycle" representative of a 4 hour period IRL. Set to 1 for testing
        public TimeCycle CurrentCycle { get; private set; }

        public Time()
        {
            CurrentCycle = TimeCycle.Dawn;
            TimePeriod = 30;
            m_Time = new Timer(1000 * 60 * TimePeriod);
            m_Time.Enabled = true;
            m_Time.AutoReset = true;
            m_Time.Elapsed += OnTimeEvent;
        }
        private void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            if (((byte)CurrentCycle) == 3)
                CurrentCycle = TimeCycle.Dawn;
            else
                CurrentCycle = (TimeCycle)(CurrentCycle + 1);

#if DEBUG
            Console.WriteLine("The Cycle has changed to {0}", CurrentCycle);
#endif
        }
    }
}
