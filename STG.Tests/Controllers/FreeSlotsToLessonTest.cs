using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STG.Controllers.Engine;
using System.Collections.Generic;

namespace STG.Tests.Controllers
{
    [TestClass]
    public class FreeSlotsToLessonTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            timeSlots.Add(new TimeSlot(0, 0));
            timeSlots.Add(new TimeSlot(0, 2));
            timeSlots.Add(new TimeSlot(0, 4));
            timeSlots.Add(new TimeSlot(1, 3));
            timeSlots.Add(new TimeSlot(1, 5));
            timeSlots.Add(new TimeSlot(1, 7));

            Lesson lesson = new Lesson(new Teacher("t"), new Group("g",10), new Subject("s","T","A"), 1,1);

            FreeSlotsToLesson fstl = new FreeSlotsToLesson(timeSlots,lesson);

            List<Room> rooms = new List<Room>();
            rooms.Add(new Room("nr" + 0, 5, "A"));
            rooms.Add(new Room("nr" + 1, 10, "A"));
            rooms.Add(new Room("nr" + 2, 15, "A"));
            rooms.Add(new Room("nr" + 3, 20, "A"));


        }
    }
}
