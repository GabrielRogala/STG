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
        private List<Timetable> teacherTimetable;
        private List<Timetable> groupTimetable;
        private List<Timetable> roomTimetable;
    }
}