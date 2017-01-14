using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Timetable
    {
        private List<Day> days;
        private Group group;
        private Teacher teacher;
        private Room room;
        private int numberOfDays = 5;
        private int numberOfSlots = 8;

        public Timetable()
        {
            days = new List<Day>();
            for (int i = 0; i < numberOfDays; ++i) {
                days.Add(new Day("day"+i+1, numberOfSlots));
            }
            group = null;
            teacher = null;
            room = null;
        }

        public Timetable(int numberOfDays, int numberOfSlots) 
        {
            this.numberOfDays = numberOfDays;
            this.numberOfSlots = numberOfSlots;
            days = new List<Day>();
            for (int i = 0; i < this.numberOfDays; ++i)
            {
                days.Add(new Day("day" + i + 1, this.numberOfSlots));
            }
            group = null;
            teacher = null;
            room = null;
        }

        public Timetable(ref Group group): this()
        {
            this.group = group;
            this.group.setTimetable(this);
        }

        public Timetable(ref Teacher teacher): this()
        {
            this.teacher = teacher;
            this.teacher.setTimetable(this);
        }

        public Timetable(ref Room room): this()
        {
            this.room = room;
            this.room.setTimetable(this);
        }

        public Timetable(ref Group group, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.group = group;
            this.group.setTimetable(this);
        }

        public Timetable(ref Teacher teacher, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.teacher = teacher;
            this.teacher.setTimetable(this);
        }

        public Timetable(ref Room room, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.room = room;
            this.room.setTimetable(this);
        }

        public Lesson getLesson(int day, int slot)
        {
            return days[day].getSlot(slot).getLesson(0);
        }

        public List<Lesson> getLessons(int day, int slot) {
            return days[day].getSlot(slot).getLessons();
        }

        public void addLesson(ref Lesson lesson, int day, int slot) {
            days[day].getSlot(slot).addLesson(lesson);
        }

        public void removeLesson(ref Lesson lesson, int day, int slot)
        {
            days[day].getSlot(slot).removeLesson(lesson);
        }

        public void removeAllLessons(int day, int slot)
        {
            days[day].getSlot(slot).remoweAllLessons();
        }

        public override string ToString()
        {
            String tmp = "";
            if (group != null) {
                tmp += "\n======================";
            } else if (teacher != null) {
                tmp += "\n======================";
            } else if (room != null) {
                tmp += "\n======================";
            } else {
                tmp += "\n============================================\n";
            }
            tmp += "\t";
            for (int d = 0; d < numberOfDays; ++d)
            {
                tmp += "|______"+days[d].getName()+"______";
            }
            tmp += "|\n";
            for (int h = 0; h < numberOfSlots; ++h)
            {
                tmp += h + "\t";
                for (int d = 0; d < numberOfDays; ++d) {
                    if (getLessons(d, h).Count > 0) {
                        tmp += "|";
                        for (int i = 0; i < getLessons(d, h).Count; i++) {
                            tmp += " " + getLesson(d, h).ToString() + " ";
                        }
                    } else {
                        tmp += " ------------ ";
                    }
                }
                tmp += "|\n";
            }
            tmp += "============================================\n";
            return tmp;
        }
    }
}