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
        private int numberOfSlots = 9;

        public Timetable()
        {
            days = new List<Day>();
            for (int i = 0; i < numberOfDays; ++i) {
                days.Add(new Day("day"+(i+1), numberOfSlots));
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
                days.Add(new Day("day" + (i + 1), this.numberOfSlots));
            }
            group = null;
            teacher = null;
            room = null;
        }

        public Timetable(Group group): this()
        {
            this.group = group;
            this.group.setTimetable(this);
        }

        public Timetable(Teacher teacher): this()
        {
            this.teacher = teacher;
            this.teacher.setTimetable(this);
        }

        public Timetable(Room room): this()
        {
            this.room = room;
            this.room.setTimetable(this);
        }

        public Timetable(Group group, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.group = group;
            this.group.setTimetable(this);
        }

        public Timetable(Teacher teacher, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.teacher = teacher;
            this.teacher.setTimetable(this);
        }

        public Timetable(Room room, int numberOfDays, int numberOfSlots) : this(numberOfDays, numberOfSlots)
        {
            this.room = room;
            this.room.setTimetable(this);
        }

        public Group getGroup() {
            return group;
        }

        public Teacher getTeacher()
        {
            return teacher;
        }

        public Room getRoom()
        {
            return room;
        }

        public List<Day> getDays() {
            return days;
        }

        public Lesson getLesson(int day, int slot)
        {
            return days[day].getSlot(slot).getLesson(0);
        }

        public List<Lesson> getLessons(int day, int slot) {
            return days[day].getSlot(slot).getLessons();
        }

        public void addLesson(Lesson lesson, int day, int slot) {
            days[day].getSlot(slot).addLesson(lesson);
        }

        public void removeLesson(Lesson lesson, int day, int slot)
        {
            days[day].getSlot(slot).removeLesson(lesson);
        }

        public void removeAllLessons(int day, int slot)
        {
            days[day].getSlot(slot).remoweAllLessons();
        }

        public void lockSlot(int day, int slot) {
            days[day].getSlot(slot).lockSlot();
        }

        public void unlockSlot(int day, int slot)
        {
            days[day].getSlot(slot).unlockSlot();
        }

        public override string ToString()
        {
            String tmp = "";
            if (group != null) {
                tmp += "\n======================" + group.getName() + "======================\n";
            } else if (teacher != null) {
                tmp += "\n======================" + teacher.getName() + "======================\n";
            } else if (room != null) {
                tmp += "\n======================" + room.getName() + "======================\n";
            } else {
                tmp += "\n============================================\n";
            }
            tmp += "\t";
            for (int d = 0; d < numberOfDays; ++d)
            {
                tmp += "|________________" + days[d].getName()+ "_________________";
            }
            tmp += "|\n";
            for (int h = 0; h < numberOfSlots; ++h)
            {
                tmp += h + "\t";
                for (int d = 0; d < numberOfDays; ++d) {
                    if (getLessons(d, h).Count > 0) {
                        tmp += "|";
                        for (int i = 0; i < getLessons(d, h).Count; i++) {
                            tmp += " " + getLesson(i, d, h).ToString() + " ";
                        }
                    } else {
                        tmp += "| ----------------------------------- ";
                    }
                }
                tmp += "|\n";
            }
            tmp += "============================================\n";
            return tmp.Replace("\n",Environment.NewLine); ;
        }

        private Lesson getLesson(int i, int day, int slot)
        {
            return days[day].getSlot(slot).getLesson(i);
        }

        //public List<TimeSlot> getFreeSlots(int size = 1) {
        //    List<TimeSlot> freeSlots = new List<TimeSlot>();

        //    for (int h = 0; h < numberOfSlots - (size - 1); ++h)
        //    {
        //        for (int d = 0; d < numberOfDays; ++d)
        //        {

        //            bool result = true;

        //            for (int i = 0; i < size; ++i)
        //            {
        //                result = result && days[d].getSlot(h + i).isEmpty() && !days[d].getSlot(h + i).isLocked();
        //            }

        //            if (result)
        //            {
        //                freeSlots.Add(new TimeSlot(d, h));
        //            }
        //        }
        //    }

        //    return freeSlots;
        //}

        public List<TimeSlot> getFreeSlotsToLesson(Lesson lesson)
        {
            List<TimeSlot> freeSlots = new List<TimeSlot>();
            
            if (group != null && group.getParent() != null) {

                Console.WriteLine(group.ToString() + " / " + group.getParent().ToString());

                freeSlots.Add(group.getSubGroupFreeSlotToLesson(lesson));
                if (freeSlots.Count > 0 && freeSlots[0] != null) {

                    foreach (TimeSlot ts in freeSlots) {
                        Console.WriteLine(ts.ToString());
                    }
                    
                    return freeSlots;
                }
            }

            for (int h = 0; h < numberOfSlots - (lesson.getSize() - 1); ++h)
            {
                for (int d = 0; d < numberOfDays; ++d)
                {

                    bool result = true;

                    for (int i = 0; i < lesson.getSize(); ++i)
                    { 
                        result = result && days[d].getSlot(h + i).isEmpty() && !days[d].getSlot(h + i).isLocked();
                    }

                    if (result)
                    {
                        freeSlots.Add(new TimeSlot(d, h));
                    }
                }
            }

            return freeSlots;
        }

        public int fitness()
        {
            int value = 0;
            value += fitnessFreeSlots();

            if (group != null)
            {
                value += fitnessType();
            }

            return value;
        }

        public int fitnessFreeSlots()
        {
            int value = 0;
            foreach (Day d in days)
            {
                int lastIndex = -1;
                for (int i = 0; i < d.getSlots().Count; i++)
                {
                    if (!d.getSlot(i).isEmpty() || d.getSlot(i).isLocked())
                    {
                        value += (int)Math.Pow((i - lastIndex - 1), 3);
                        lastIndex = i;
                    }

                }
            }

            return value;
        }

        public int fitnessType()
        {
            int value = 0;

            //int sizeOfSubjectType = Enum.GetNames(typeof(SubjectType)).Length;
            //if (this.group != null)
            //{
            //    foreach (Day d in days)
            //    {
            //        int[] numberOfType = new int[sizeOfSubjectType];
            //        for (int i = 0; i < sizeOfSubjectType; i++)
            //        {
            //            numberOfType[i] = 0;
            //        }
            //        int count = 0;

            //        for (int i = 0; i < d.getSlots().Count; i++)
            //        {
            //            if (!d.getSlots()[i].isEmpty())
            //            {
            //                count++;
            //                numberOfType[(int)d.getSlots()[i].getLesson(0).getSubject().getSubjectType()]++;
            //            }
            //        }

            //        count = count / sizeOfSubjectType;
            //        for (int i = 0; i < sizeOfSubjectType; i++)
            //        {
            //            numberOfType[i] = numberOfType[i] - count;
            //            if (numberOfType[i] > 0)
            //            {
            //                value += (int)(Math.Pow(numberOfType[i], 2));
            //            }
            //        }
            //    }
            //}
            return value;
        }

        public List<TimeSlot> getSlotsWithLesson(int size = 1)
        {
            List<TimeSlot> slots = new List<TimeSlot>();

            for (int h = 0; h < numberOfSlots; ++h)
            {
                for (int d = 0; d < numberOfDays; ++d)
                {
                    if (!days[d].getSlot(h).isEmpty() && !days[d].getSlot(h).isLocked() && days[d].getSlot(h).getLesson(0).getSize() == 1)
                    {
                        slots.Add(new TimeSlot(d, h));
                    }
                }
            }

            return slots;
        }

        public List<Lesson> getAllLessonsWithTimetable()
        {
            List<Lesson> tmpLessons = new List<Lesson>();

            for (int h = 0; h < numberOfSlots; ++h)
            {
                for (int d = 0; d < numberOfDays; ++d)
                {
                    if (!days[d].getSlot(h).isEmpty())
                    {
                        //tylko jedna lekcja w slocie
                        //tmpLessons.Add(days[d].getSlot(h).getLesson(0));
                        //wszystkie lekcje w slocie
                        tmpLessons.AddRange(days[d].getSlot(h).getLessons());
                    }
                }
            }

            return tmpLessons;
        }
    }
}