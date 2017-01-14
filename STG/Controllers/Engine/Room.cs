using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Room
    {
        private String name;
        private Timetable timetable;
        private int amount;
        private String roomType;

        public Room()
        {
            this.name = "NULL";
            this.amount = 0;
            this.roomType = "NONE";
        }

        public Room(string name, int amount, string roomType) : this()
        {
            this.name = name;
            this.amount = amount;
            this.roomType = roomType;
        }

        public String getName()
        {
            return name;
        }

        public int getAmount()
        {
            return amount;
        }

        public String getRoomType() {
            return roomType;
        }

        public void setTimetable(Timetable timetable)
        {
            this.timetable = timetable;
        }

        public Timetable getTimetable()
        {
            return timetable;
        }

        public override string ToString()
        {
            return name+"("+amount+"/"+roomType+")";
        }
    }
}