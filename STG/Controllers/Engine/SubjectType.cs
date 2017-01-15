using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class SubjectType
    {
        private static SubjectType instance;
        private List<String> types;

        private SubjectType() {
            types = new List<String>();
        }

        public static SubjectType getInstance() {
            if (instance == null) {
                instance = new SubjectType();
            } 
            return instance;
        }

        public void addTypes(String type) {
            types.Add(type);
        }

        public List<String> getTypes() {
            return types;
        }

        public int getIndexOf(String type) {
            return types.IndexOf(type);
        }
    }
}