using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Lesson
    {
        private Subject subject;
        private int amount;
        private int size;
        private Group group;
        private Teacher teacher;
        private Room room;
        private List<TimeSlot> slots;

        public Lesson()
        {
            this.subject = null;
            this.amount = 0;
            this.size = 0;
            this.group = null;
            this.teacher = null;
            slots = new List<TimeSlot>();
        }

        public Lesson(Teacher teacher, Group group, Subject subject, int amount) : this(teacher, group, subject, amount,1) {}

        public Lesson(Teacher teacher, Group group, Subject subject, int amount, int size): this()
        {
            this.subject = subject;
            this.amount = amount;
            this.size = size;
            this.group = group;
            this.teacher = teacher;
        }

        //// getters

        public Subject getSubject()
        {
            return subject;
        }

        public int getAmount()
        {
            return amount;
        }

        public int getSize()
        {
            return size;
        }

        public Group getGroup() {
            return group;
        }

        public Teacher getTeacher() {
            return teacher;
        }

        public Room getRoom() {
            return room;
        }

        public List<TimeSlot> getSlots()
        {
            return slots;
        }

        //// setters
        
        public void setRoom(Room room)
        {
            this.room = room;
        }

        public void addSlot(TimeSlot slot) {
            slots.Add(slot);
        }

        public void removeAllSlots() {
            slots.Clear();
        }

        //// others

        public bool isDifferent(Lesson lesson)
        {
            return !this.subject.Equals(lesson.getSubject());
        }

        //public void addLessonToTimetable(int day, int hour)
        //{
        //    this.getGroup().addLesson(this, day,hour);
        //    this.getTeacher().getTimetable().addLesson(this, day, hour);
        //    this.getRoom().getTimetable().addLesson(this, day, hour);
        //}

        //public void removeLessonFromTimetable(int day, int hour) {
        //    this.getGroup().removeLesson(this, day, hour);
        //    this.getTeacher().getTimetable().removeLesson(this, day, hour);
        //    this.getRoom().getTimetable().removeLesson(this, day, hour);
        //}

        //// overrides

        public override string ToString()
        {
            String tmp = "";
            if (room != null) {
                tmp = ":" + room.ToString();
            }
            return group.ToString() + "/" + teacher.ToString() + "/" + subject.ToString() + "(" + amount + "/" + size + ")" + tmp;
        }

        public override bool Equals(object obj)
        {
            return this.subject.Equals(((Lesson)obj).subject) && this.group.Equals(((Lesson)obj).group) && this.teacher.Equals(((Lesson)obj).teacher) && this.amount.Equals(((Lesson)obj).amount) && this.size.Equals(((Lesson)obj).size);
        }

    }
}