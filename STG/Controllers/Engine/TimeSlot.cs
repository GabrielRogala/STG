﻿using System;
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
    }
}