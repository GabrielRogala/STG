﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class FreeSlotsToLesson
    {
        public List<TimeSlot> slots;
        public int size;
        public Lesson lesson;
        public List<FreeSlotsInRoomToLesson> roomSlots;

        public FreeSlotsToLesson(List<TimeSlot> slots, Lesson lesson)
        {
            this.slots = slots;
            this.size = slots.Count;
            this.lesson = lesson;
            roomSlots = new List<FreeSlotsInRoomToLesson>();
        }

        public FreeSlotsToLesson(List<TimeSlot> slots, Lesson lesson, List<FreeSlotsInRoomToLesson> roomSlots) : this(slots, lesson)
        {
            this.roomSlots = roomSlots;
            this.slots = getTheSameSlots(slots, getSlotsFromSlotsLists(roomSlots));
        }

        public override string ToString()
        {
            String tmp = lesson.ToString() + " : " + size;
            foreach (FreeSlotsInRoomToLesson fr in roomSlots) {
                tmp += "(" + fr.ToString() + "),";
            }
            return tmp;
        }

        public List<TimeSlot> getSlotsFromSlotsLists(List<FreeSlotsInRoomToLesson> roomSlots)
        {
            List<TimeSlot> tmp = new List<TimeSlot>();

            foreach (FreeSlotsInRoomToLesson fs in roomSlots)
            {
                foreach (TimeSlot ts in fs.slots)
                {
                    if (!tmp.Contains(ts))
                    {
                        tmp.Add(ts);
                    }
                }
            }

            return tmp;
        }

        public List<TimeSlot> getSlots() {
            return slots;
        }

        public static List<TimeSlot> getTheSameSlots(List<TimeSlot> freeSlots1, List<TimeSlot> freeSlots2)
        {
            List<TimeSlot> freeSlot = new List<TimeSlot>();

            foreach (TimeSlot ts in freeSlots1) {
                if (freeSlots2.Contains(ts)) {
                    freeSlot.Add( new TimeSlot(ts.day, ts.hour));
                }
            }

            //int indexTeacher = 0;
            //
            //for (int indexGroup = 0; indexGroup < freeSlotsGroup.Count() && indexTeacher < freeSlotsTeacher.Count();)
            //{
            //    if (freeSlotsGroup[indexGroup].Equals(freeSlotsTeacher[indexTeacher]))
            //    {
            //        freeSlot.Add(new TimeSlot(freeSlotsGroup[indexGroup].day, freeSlotsGroup[indexGroup].hour));
            //        indexTeacher++;
            //        indexGroup++;
            //    }
            //    else
            //    {
            //        if (freeSlotsGroup[indexGroup].day > freeSlotsTeacher[indexTeacher].day)
            //        {
            //            indexTeacher++;
            //        }
            //        else if (freeSlotsGroup[indexGroup].day < freeSlotsTeacher[indexTeacher].day)
            //        {
            //            indexGroup++;
            //        }
            //        else
            //        {
            //            if (freeSlotsGroup[indexGroup].hour > freeSlotsTeacher[indexTeacher].hour)
            //            {
            //                indexTeacher++;
            //            }
            //            else
            //            {
            //                indexGroup++;
            //            }
            //        }
            //    }
            //}

            return freeSlot;
        }
    }
}