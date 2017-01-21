using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.Controllers.Engine
{
    public class Population
    {
        private List<SchoolTimetable> schoolTimeTables;
        private List<Lesson> lessons;
        private List<Teacher> teachers;
        private List<Group> groups;
        private int size;
        private Random rand = new Random();

        public Population()
        {
            schoolTimeTables = new List<SchoolTimetable>();
            size = 0;
        }

        public void generatePopulation(int size, List<Lesson> lessons, List<Teacher> teachers, List<Group> groups, List<Room> rooms, List<String> subjectTypes, int numberOfDays, int numberOfSlots)
        {
            this.size = size;
            this.groups = groups;
            this.lessons = lessons;
            this.teachers = teachers;

            for (int i = 0; i < this.size; ++i)
            {
                schoolTimeTables.Add(new SchoolTimetable(teachers, groups, rooms, lessons, subjectTypes, numberOfDays, numberOfSlots));
                schoolTimeTables[i].generateSchoolTimetable();
                //schoolTimeTables[i].setId(i);
                //Console.WriteLine(i + ": " + "[" + schoolTimeTables[i].getId() + "] " + schoolTimeTables[i].fitness());
            }
        }

        public void generateNewPopulation()
        {
            List<SchoolTimetable> schoolTimeTables_tmp = new List<SchoolTimetable>();

            schoolTimeTables.Sort(new Comparison<SchoolTimetable>(schoolTimeTableComparator));
            //Console.WriteLine("gnp---------------------------v");
            for (int i = 0; i < this.size; ++i)
            {
                //  Console.WriteLine(i + ": " + "[" + schoolTimeTables[i].getId() + "] " + schoolTimeTables[i].getValue());
            }
            //Console.WriteLine("------------------------------");
            for (int i = 0; i < this.size; ++i)
            {

                if (i < size * 0.2)
                {
                    //schoolTimeTables_tmp.Add(new SchoolTimetable(lessons, teachers, groups, schoolTimeTables[i]));
                    if (rand.NextDouble() > 0.1)
                    {

                        //schoolTimeTables_tmp.Add(new SchoolTimetable(lessons, teachers, groups, schoolTimeTables[i]));
                        //schoolTimeTables_tmp[i + 1].mutate();

                        if (schoolTimeTables_tmp[i].fitness() <= schoolTimeTables_tmp[i + 1].fitness())
                        {
                            schoolTimeTables_tmp.RemoveAt(i + 1);
                        }
                        else
                        {
                            schoolTimeTables_tmp.RemoveAt(i);
                        }
                    }
                }
                else if (i < size * 0.8)
                {
                    //schoolTimeTables_tmp.Add(new SchoolTimetable(lessons, teachers, groups));
                    //schoolTimeTables_tmp[i].crossSchoolTimeTables(schoolTimeTables[i], schoolTimeTables[rand.Next((int)(size * 0.2))]);
                    if (rand.NextDouble() > 0.1)
                    {
                       // schoolTimeTables_tmp[i].mutate();
                    }
                }
                else
                {
                    //schoolTimeTables_tmp.Add(new SchoolTimeTable(lessons, teachers, groups));
                    //schoolTimeTables_tmp[i].generateSchoolTimeTable();
                }

                //schoolTimeTables_tmp[i].setId(i);
                schoolTimeTables_tmp[i].fitness();
            }

            for (int i = 0; i < this.size; ++i)
            {
                //  Console.WriteLine(i + ": "+"["+ schoolTimeTables_tmp[i].getId()+ "] " + schoolTimeTables_tmp[i].fitness());
            }

            schoolTimeTables = schoolTimeTables_tmp;

            //Console.WriteLine("gnp---------------------------^");
        }

        public void start(int epoch)
        {
            for (int i = 0; i < epoch; ++i)
            {
                Console.WriteLine("E:" + i + "-------------------------");
                this.generateNewPopulation();
                Console.WriteLine(getBestSchoolTimeTable().fitness());
            }
        }

        public int schoolTimeTableComparator(SchoolTimetable l1, SchoolTimetable l2)
        {
            return 0;
            //return l1.getValue().CompareTo(l2.getValue());
        }

        public SchoolTimetable getBestSchoolTimeTable()
        {
            schoolTimeTables.Sort(new Comparison<SchoolTimetable>(schoolTimeTableComparator));
            return schoolTimeTables[0];
        }
    }
}