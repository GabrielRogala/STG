using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Group
    {
        private String name;
        private Timetable timetable;
        private int amount;

        private List<Group> subGroup;
        private Group parent;
        private int subGroupsIndex;
        private int subGroupId;

        public Group()
        {
            this.name = "NULL";
            this.amount = 0;
            this.subGroup = new List<Group>();
            this.subGroupsIndex = 0;
            this.subGroupId = 0;
        }

        public Group(string name, int amount): this()
        {
            this.name = name;
            this.amount = amount;
        }

        public Group(string name, int amount, int subGroupsIndex, int subGroupId) : this(name, amount)
        {
            this.subGroupsIndex = subGroupsIndex;
            this.subGroupId = subGroupId;
        }

        public Group(string name, int amount, List<Group> subGroup): this(name,amount) {
            this.subGroup = subGroup;
            foreach (Group g in this.subGroup) {
                g.setParent(this);
            }
        }

        //// getters

        public String getName() {
            return name;
        }

        public Timetable getTimetable()
        {
            return timetable;
        }

        public int getAmount()
        {
            return amount;
        }

        public List<Group> getSubGroup() {
            return subGroup;
        }

        public Group getParent()
        {
            return parent;
        }

        public int getSubGroupsIndex()
        {
            return subGroupsIndex;
        }

        public int getSubGroupId() {
            return subGroupId;
        }

        //// setters

        public void setTimetable(Timetable timetable) {
            this.timetable = timetable;
        }
        
        public void setParent(Group group) {
            this.parent = group;
        }

        //// others
        // tr
        public void addLesson(Lesson lesson,int day , int slot) {
            if (parent != null) {
                parent.getTimetable().addLesson(lesson, day, slot);
            } 
            this.timetable.addLesson(lesson,day,slot);
        }
        // tr
        public void removeLesson(Lesson lesson, int day, int slot)
        {
            if (parent != null)
            {
                parent.getTimetable().removeLesson(lesson, day, slot);
            }
            else if (subGroup.Count > 0)
            {
                foreach (Group g in subGroup)
                {
                    g.getTimetable().removeLesson(lesson, day, slot);
                }
            }
            this.timetable.removeLesson(lesson, day, slot);
        }
        // tr
        public TimeSlot getSubGroupFreeSlotToLesson(Lesson lesson) {
            if (parent != null)
            {
                foreach (Group g in parent.getSubGroup()) {
                    if (lesson.getGroup().getSubGroupsIndex() == g.getSubGroupsIndex() && !g.Equals(lesson.getGroup())) {
                        if (g.getTimetable().getFreeSlotsToLesson(lesson).Count > 0) {
                            foreach(TimeSlot ts in g.getTimetable().getFreeSlotsToLesson(lesson))
                            {
                                if (lesson.getGroup().getTimetable().getLessons(ts.day,ts.hour).Count == 0) {
                                    return ts;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
      
        //// overrides

        public override string ToString()
        {
            String tmp = "";
            if (parent != null) {
                tmp += subGroupsIndex + ":" + subGroupId;
            }
            return name + "(" + amount + ")" + tmp;
        }

        public override bool Equals(object obj)
        {
            return this.name.Equals(((Group)obj).name);
        }
    }
}