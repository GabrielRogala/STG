using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class TimeSlot
    {
        public int day;
        public int hour;

        public TimeSlot(int day, int hour)
        {
            this.day = day;
            this.hour = hour;
        }

        public override bool Equals(object obj)
        {
            return (this.day == ((TimeSlot)obj).day) && (this.hour == ((TimeSlot)obj).hour);
        }

        public override string ToString()
        {
            return "["+day + "," + hour +"]" ;
        }
    }
}