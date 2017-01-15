using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Teacher
    {
        private String name;
        private Timetable timetable;

        public Teacher()
        {
            this.name = "NULL";
        }

        public Teacher(string name) : this()
        {
            this.name = name;
        }

        public String getName()
        {
            return name;
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
            return name;
        }

        public override bool Equals(object obj)
        {
            return this.name.Equals(((Teacher)obj).name);
        }
    }
}