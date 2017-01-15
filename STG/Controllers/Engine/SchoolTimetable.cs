using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class SchoolTimetable
    {
        private List<Teacher> teachers;
        private List<Group> groups;
        private List<Room> rooms;
        private List<Lesson> lessons;
        private int numberOfDays = 5;
        private int numberOfSlots = 8;
        private List<Timetable> teachersTimetables;
        private List<Timetable> groupsTimetables;
        private List<Timetable> roomsTimetables;

        public SchoolTimetable() {
            teachers = new List<Teacher>();
            groups = new List<Group>();
            rooms = new List<Room>();
            lessons = new List<Lesson>();
            teachersTimetables = new List<Timetable>();
            groupsTimetables = new List<Timetable>();
            roomsTimetables = new List<Timetable>();
        }

        public SchoolTimetable(List<Teacher> teachers, List<Group> groups, List<Room> rooms, List<Lesson> lessons) : this()
        {
            this.teachers = teachers;
            this.groups = groups;
            this.rooms = rooms;
            this.lessons = lessons;
        }

        public SchoolTimetable(List<Teacher> teachers, List<Group> groups, List<Room> rooms, List<Lesson> lessons, int numberOfDays, int numberOfSlots) : this(teachers, groups, rooms, lessons)
        {
            this.numberOfDays = numberOfDays;
            this.numberOfSlots = numberOfSlots;
        }

        public void generateTeachersTimetables(List<Teacher> teachers) {
            foreach (Teacher t in teachers) {
                teachersTimetables.Add(new Timetable(t, numberOfDays,numberOfSlots));
            }
        }

        public void generateGroupsTimetables(List<Group> groups)
        {
            foreach (Group g in groups)
            {
                groupsTimetables.Add(new Timetable(g, numberOfDays, numberOfSlots));
            }
        }

        public void generateRoomsTimetables(List<Room> rooms)
        {
            foreach (Room r in rooms)
            {
                roomsTimetables.Add(new Timetable(r, numberOfDays, numberOfSlots));
            }
        }

        public void generateSchoolTimetable() {
            generateGroupsTimetables(groups);
            generateRoomsTimetables(rooms);
            generateTeachersTimetables(teachers);
        }

        public void print()
        {
            foreach (Teacher t in teachers)
            {
                Console.WriteLine(t.getTimetable().ToString());
            }

            foreach (Group g in groups)
            {
                Console.WriteLine(g.getTimetable().ToString());
            }

            foreach (Room r in rooms)
            {
                Console.WriteLine(r.getTimetable().ToString());
            }
        }

    }
}