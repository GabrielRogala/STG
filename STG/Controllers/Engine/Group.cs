using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Group
    {
        private String name;
        private Timetable timetable;
        private int amount;

        public Group()
        {
            this.name = "NULL";
            this.amount = 0;
        }

        public Group(string name, int amount): this()
        {
            this.name = name;
            this.amount = amount;
        }

        public String getName() {
            return name;
        }

        public int getAmount() {
            return amount;
        }

        public void setTimetable(Timetable timetable) {
            this.timetable = timetable;
        }

        public Timetable getTimetable() {
            return timetable;
        }

        public override string ToString()
        {
            return name + "(" + amount + ")";
        }

        public override bool Equals(object obj)
        {
            return this.name.Equals(((Group)obj).name);
        }
    }
}