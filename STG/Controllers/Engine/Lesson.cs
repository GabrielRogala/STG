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
        private int idGroup;
        private int idTeacher;
        private int idRoom;
        private List<TimeSlot> slots;
    }
}