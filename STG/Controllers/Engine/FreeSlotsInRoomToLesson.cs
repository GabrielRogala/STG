using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class FreeSlotsInRoomToLesson
    {
        public Room room;
        public List<TimeSlot> slots;
        public int size;

        public FreeSlotsInRoomToLesson(List<TimeSlot> slots, Room room)
        {
            this.room = room;
            this.slots = slots;
            this.size = slots.Count;
        }

        public override string ToString()
        {
            return room.ToString() + " : " + size;
        }
    }
}