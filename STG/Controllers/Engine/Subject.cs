using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Subject
    {
        private String name;
        private String subjectType;
        private String roomType;

        public Subject()
        {
            this.name = "name";
            this.subjectType = "subjectType";
            this.roomType = "roomType";
        }

        public Subject(string name, string subjectType, string roomType) : this()
        {
            this.name = name;
            this.subjectType = subjectType;
            this.roomType = roomType;
        }

        public String getName() {
            return name;
        }

        public String getSubjectType() {
            return subjectType;
        }

        public String getRoomType() {
            return roomType;
        }

        public override string ToString()
        {
            return name + "(" +subjectType+ "/" +roomType+ ")";
        }
    }
}