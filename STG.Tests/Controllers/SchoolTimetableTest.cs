using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STG.Controllers.Engine;
using System.Collections.Generic;

namespace STG.Tests.Controllers
{
    [TestClass]
    public class SchoolTimetableTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Lesson> lessons = new List<Lesson>();
            List<Teacher> teachers = new List<Teacher>();
            List<Group> groups = new List<Group>();
            List<Subject> subjects = new List<Subject>();
            List<Room> rooms = new List<Room>();

            for (int j = 0; j < 7; j++)
            {
                teachers.Add(new Teacher("t" + j));
            }
            for (int j = 0; j < 6; j++)
            {
                groups.Add(new Group("g" + j, 25 + j));
            }
            int nr = 0;
            rooms.Add(new Room("nr" + nr++, 25, "A"));
            rooms.Add(new Room("nr" + nr++, 40, "A"));
            rooms.Add(new Room("nr" + nr++, 40, "A"));
            rooms.Add(new Room("nr" + nr++, 40, "A"));
            rooms.Add(new Room("nr" + nr++, 40, "B"));
            rooms.Add(new Room("nr" + nr++, 40, "B"));
            rooms.Add(new Room("nr" + nr++, 40, "C"));

            int i = 0;
            subjects.Add(new Subject("pol", "HUM", "A"));
            subjects.Add(new Subject("ang", "JEZ", "A"));
            subjects.Add(new Subject("mat", "SCI", "A"));
            subjects.Add(new Subject("his", "HUM", "A"));
            subjects.Add(new Subject("wos", "HUM", "A"));
            subjects.Add(new Subject("fiz", "SCI", "A"));
            subjects.Add(new Subject("bio", "SCI", "A"));
            subjects.Add(new Subject("geo", "SCI", "A"));
            subjects.Add(new Subject("w-f", "SPO", "C"));
            subjects.Add(new Subject("rel", "INN", "A"));
            subjects.Add(new Subject("inf", "SPE", "B"));
            subjects.Add(new Subject("PRO", "SPE", "B"));

            foreach (Group g in groups)
            {
                int tI = 0;
                int sI = 0;
                int amount = 0;
                //max = 45
                //----------pol----------
                tI = 0; sI = 0; amount = 5;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount, 2));
                //----------ang----------
                tI = 1; sI = 1; amount = 4;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------mat----------
                tI = 2; sI = 2; amount = 5;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------his----------
                tI = 3; sI = 3; amount = 1;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------wos----------
                tI = 3; sI = 4; amount = 2;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------fiz----------
                tI = 5; sI = 5; amount = 2;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------bio----------
                tI = 3; sI = 6; amount = 1;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------geo----------
                tI = 3; sI = 7; amount = 2;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------w-f----------
                tI = 4; sI = 8; amount = 3;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------rel----------
                tI = 5; sI = 9; amount = 2;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------inf----------
                tI = 4; sI = 10; amount = 2;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount));
                //----------pro----------
                tI = 6; sI = 11; amount = 5;
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount, 2));
                lessons.Add(new Lesson(teachers[tI], g, subjects[sI], amount, 3));

            }

            SchoolTimetable stt = new SchoolTimetable(teachers, groups, rooms,lessons,5,8);
            stt.generateSchoolTimetable();
            stt.print();
        }
    }
}
