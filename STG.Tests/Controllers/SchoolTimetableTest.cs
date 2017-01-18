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
            List<String> subjectTypes = new List<String>();

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

            subjectTypes.Add("HUM");
            subjectTypes.Add("SCI");
            subjectTypes.Add("JEZ");
            subjectTypes.Add("SPE");
            subjectTypes.Add("SPO");
            subjectTypes.Add("INN");

            int i = 0;
            subjects.Add(new Subject("pol", subjectTypes[0], "A"));
            subjects.Add(new Subject("ang", subjectTypes[2], "A"));
            subjects.Add(new Subject("mat", subjectTypes[1], "A"));
            subjects.Add(new Subject("his", subjectTypes[0], "A"));
            subjects.Add(new Subject("wos", subjectTypes[0], "A"));
            subjects.Add(new Subject("fiz", subjectTypes[1], "A"));
            subjects.Add(new Subject("bio", subjectTypes[1], "A"));
            subjects.Add(new Subject("geo", subjectTypes[1], "A"));
            subjects.Add(new Subject("w-f", subjectTypes[4], "C"));
            subjects.Add(new Subject("rel", subjectTypes[5], "A"));
            subjects.Add(new Subject("inf", subjectTypes[3], "B"));
            subjects.Add(new Subject("PRO", subjectTypes[3], "B"));

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

            SchoolTimetable stt = new SchoolTimetable(teachers, groups, rooms,lessons,subjectTypes,5,9);
            stt.generateSchoolTimetable();
            stt.print();
        }
    }
}
