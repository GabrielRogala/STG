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
        private int numberOfDays = 5;
        private int numberOfSlots = 8;
        private List<Timetable> teachersTimetables;
        private List<Timetable> groupsTimetables;
        private List<Timetable> roomsTimetables;
        private SubjectType subjectTypes;
        private const int NUMBER_OF_LESSONS_TO_POSITIONING = 5;

        public SchoolTimetable() {
            teachers = new List<Teacher>();
            groups = new List<Group>();
            rooms = new List<Room>();
            lessons = new List<Lesson>();
            teachersTimetables = new List<Timetable>();
            groupsTimetables = new List<Timetable>();
            roomsTimetables = new List<Timetable>();
            subjectTypes = SubjectType.getInstance();
        }

        public SchoolTimetable(List<Teacher> teachers, List<Group> groups, List<Room> rooms, List<Lesson> lessons, List<String> subjectTypes) : this()
        {
            this.teachers = teachers;
            this.groups = groups;
            this.rooms = rooms;
            this.lessons = lessons;
            this.subjectTypes.getTypes().Clear();
            foreach (String s in subjectTypes) {
                this.subjectTypes.addTypes(s);
            }
        }

        public SchoolTimetable(List<Teacher> teachers, List<Group> groups, List<Room> rooms, List<Lesson> lessons, List<String> subjectTypes, int numberOfDays, int numberOfSlots) : this(teachers, groups, rooms, lessons, subjectTypes)
        {
            this.numberOfDays = numberOfDays;
            this.numberOfSlots = numberOfSlots;
        }

        private void generateTeachersTimetables(List<Teacher> teachers) {
            foreach (Teacher t in teachers) {
                teachersTimetables.Add(new Timetable(t, numberOfDays,numberOfSlots));
            }
        }

        private void generateGroupsTimetables(List<Group> groups)
        {
            foreach (Group g in groups)
            {
                groupsTimetables.Add(new Timetable(g, numberOfDays, numberOfSlots));
            }
        }

        private void generateRoomsTimetables(List<Room> rooms)
        {
            foreach (Room r in rooms)
            {
                roomsTimetables.Add(new Timetable(r, numberOfDays, numberOfSlots));
            }
        }

        public void generateSchoolTimetable() {
            generateGroupsTimetables(groups);
            generateRoomsTimetables(rooms);
            generateTeachersTimetables(teachers);
            genereteTimetable(lessons);
        }

        private void genereteTimetable(List<Lesson> lessons)
        {
            List<Lesson> tmpLessons = new List<Lesson>();
            List<Lesson> choosenLesson = new List<Lesson>();

            foreach (Lesson l in lessons) {
                tmpLessons.Add(l);
            }

            sortLessons(tmpLessons);

            foreach (Lesson l in tmpLessons){
                Console.WriteLine(l.ToString());
            }

            while (tmpLessons.Count > 0) {
                choosenLesson.AddRange(findDifferentSubjectTheSameGroup(tmpLessons.Last(), tmpLessons));
                Console.WriteLine(tmpLessons.Count);
                findAndSetBestPositionToLessons(choosenLesson);

                foreach (Lesson l in choosenLesson) {
                    tmpLessons.Remove(l);
                }
                choosenLesson.Clear();
            }

        }

        private void findAndSetBestPositionToLessons(List<Lesson> choosenLesson)
        {
            Console.WriteLine("=================================");
            foreach (Lesson l in choosenLesson) {
                Console.WriteLine(l.ToString());
            }
            //TO DO
        }

        private List<Lesson> findDifferentSubjectTheSameGroup(Lesson lesson, List<Lesson> lessons)
        {
            List<Lesson> tmpLessons = new List<Lesson>();
            tmpLessons.Add(lesson);

            for (int i = lessons.Count - 1; i >= 0; i--)
            {
                if (tmpLessons.Count < NUMBER_OF_LESSONS_TO_POSITIONING)
                {
                    if (lessons[i].getGroup().Equals(tmpLessons[0].getGroup()))
                    {
                        if (!isContainInLessonsList(tmpLessons, lessons[i]))
                        {
                            tmpLessons.Add(lessons[i]);
                        }
                    }
                } else {
                    break;
                }
            }

            return tmpLessons;
        }

        private bool isContainInLessonsList(List<Lesson> tmpLessons, Lesson lesson)
        {
            foreach (Lesson l in tmpLessons)
            {
                if (!l.isDifferent(lesson))
                {
                    return true;
                }
            }
            return false;
        }

        private void sortLessons(List<Lesson> tmpLessons)
        {
            tmpLessons.Sort(new Comparison<Lesson>(lessonComparator));
        }

        public int lessonComparator(Lesson l1, Lesson l2)
        {
            return sizeLessonComparator(l1, l2);
        }

        public int sizeLessonComparator(Lesson l1, Lesson l2)
        {
            if (l1.getSize() > l2.getSize())
            {
                return 1;
            }
            else if (l1.getSize() < l2.getSize())
            {
                return -1;
            }
            else
            {
                return amountLessonComparator(l1, l2);
            }
        }

        public int typeLessonComparator(Lesson l1, Lesson l2)
        {
            if (subjectTypes.getIndexOf(l1.getSubject().getSubjectType()) < subjectTypes.getIndexOf(l2.getSubject().getSubjectType()))
            {
                return 1;
            }
            else if (subjectTypes.getIndexOf(l1.getSubject().getSubjectType()) > subjectTypes.getIndexOf(l2.getSubject().getSubjectType()))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public int amountLessonComparator(Lesson l1, Lesson l2)
        {
            if (l1.getAmount() > l2.getAmount())
            {
                return 1;
            }
            else if (l1.getAmount() < l2.getAmount())
            {
                return -1;
            }
            else
            {
                return typeLessonComparator(l1, l2);
            }
        }

        public void print()
        {
            foreach (Teacher t in teachers)
            {
                Console.WriteLine(t.getTimetable().ToString());
            }

            foreach (Group g in groups)
            {
                Console.WriteLine(g.getTimetable().ToString());
            }

            foreach (Room r in rooms)
            {
                Console.WriteLine(r.getTimetable().ToString());
            }
        }

    }
}