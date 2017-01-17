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
        private const int BOTTOM_BORDER_OF_BEST_SLOTS = 1;
        private const int TOP_BORDER_OF_BEST_SLOTS = 7;
        private static Random rand = new Random();

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
            foreach (Lesson l in choosenLesson)
            {
                Console.WriteLine(l.ToString());
            }

            List<FreeSlotsToLesson> freeSlotsToLesson = new List<FreeSlotsToLesson>();
            Timetable currentTimeTable = new Timetable();
            List<TimeSlot> freeSlots = new List<TimeSlot>();
            List<TimeSlot> groupSlots = new List<TimeSlot>();
            List<TimeSlot> teacherSlots = new List<TimeSlot>();
            List<TimeSlot> theSameSlots = new List<TimeSlot>();
            Group group;
            Teacher teacher;

            foreach (Lesson l in choosenLesson)
            {
                group = l.getGroup();
                teacher = l.getTeacher();

                // przygotowanie listy wolnych slotów dla odpowiednich sal
                List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();
                foreach (Timetable tt in roomsTimetables)
                {
                    if (tt.getRoom().getRoomType().Equals(l.getSubject().getRoomType()) && tt.getRoom().getAmount() >= group.getAmount())
                    {
                        freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(l.getSize()), tt.getRoom()));
                    }
                }

                groupSlots = group.getTimetable().getFreeSlots(l.getSize());
                teacherSlots = teacher.getTimetable().getFreeSlots(l.getSize());
                theSameSlots = getTheSameSlots(groupSlots, teacherSlots);

                freeSlotsToLesson.Add(new FreeSlotsToLesson(theSameSlots, l, freeSlotsInRoomToLesson));

                // przygotowanie planu lekcji z wszystkimi wolnymi slotami dla wybranych lekcji
                foreach (TimeSlot ts in theSameSlots)
                {
                    currentTimeTable.addLesson(l, ts.day, ts.hour);
                    if (!freeSlots.Contains(ts))
                    {
                        freeSlots.Add(ts);
                    }
                }

            }

            // sortowanie lekcji pod wzgledem wolnych slotów
            freeSlotsToLesson.Sort(new Comparison<FreeSlotsToLesson>(BFSComparator));

            foreach (FreeSlotsToLesson fstl in freeSlotsToLesson)
            {
                if (fstl.slots.Count > 0) {
                    List<int> indexOfSlotsWithMaxCount = new List<int>();
                    int max = 0;
                    // znajdywanie maksymalnej wartości
                    foreach (TimeSlot ts in fstl.slots)
                    {
                        int currentTiteTableSlotCount = currentTimeTable.getLessons(ts.day,ts.hour).Count;
                        if (max < currentTiteTableSlotCount)
                        {
                            max = currentTiteTableSlotCount;
                        }
                    }
                    // znajdywanie wszystki maksymalnych
                    foreach (TimeSlot ts in fstl.slots)
                    {
                        int currentTiteTableSlotCount = currentTimeTable.getLessons(ts.day, ts.hour).Count;
                        if (max == currentTiteTableSlotCount)
                        {
                            indexOfSlotsWithMaxCount.Add(fstl.slots.IndexOf(ts));
                        }
                    }

                    ////////////////////////
                    Room bestRoom = null;
                    TimeSlot bestSlot = getBestTimeSlot(fstl.slots, fstl.roomSlots, ref bestRoom);
                    // znajdywanie najlepszego slotu
                    for (int i = 0; i < fstl.lesson.getSize(); ++i)
                    {
                        if (fstl.lesson.getGroup().getTimetable().getLessons(bestSlot.day,bestSlot.hour + i).Count == 0) // czy oby na pewno jest wolny slot
                        {
                            fstl.lesson.getSlots().Add(new TimeSlot(bestSlot.day, bestSlot.hour + i));
                            fstl.lesson.setRoom(bestRoom);
                            fstl.lesson.setLessonsToTimetable(bestSlot.day, bestSlot.hour + i);
                        }
                        else
                        {
                            Console.WriteLine("ERROR!!!!!!!!!!!!!!!!"); ////////////////////////wystąpił???!
                        }
                    }

                    // usuwanie wolnych slotów z listy ustawionych lekcji
                    foreach (FreeSlotsToLesson tmp in freeSlotsToLesson)
                    {
                        for (int i = 0; i < fstl.lesson.getSize(); ++i)
                        {
                            TimeSlot tmpSlot = new TimeSlot(bestSlot.day, bestSlot.hour + i);

                            if (tmp.slots.Contains(tmpSlot)) {
                                tmp.slots.Remove(tmpSlot);
                            }
                        }
                    }

                } else {
                    Console.WriteLine("NO FREE SLOT"+fstl.lesson.ToString());
                    removeLessonsAndFindNewPosition(fstl.lesson);
                }
            }

        }

        public Boolean removeLessonsAndFindNewPosition(Lesson lesson)
        {
            bool result = true;

            Timetable teacherTT = lesson.getTeacher().getTimetable();
            List<TimeSlot> teacherFreeSlots = teacherTT.getFreeSlots(lesson.getSize());
            List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();
            foreach (Timetable tt in roomsTimetables) {
                if (tt.getRoom().getRoomType().Equals(lesson.getSubject().getRoomType()) && tt.getRoom().getAmount() >= lesson.getGroup().getAmount()) {
                    freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(lesson.getSize()), tt.getRoom()));
                }
            }
            FreeSlotsToLesson freeSlotsToLesson = new FreeSlotsToLesson(teacherFreeSlots, lesson, freeSlotsInRoomToLesson);

            Timetable groupTT = lesson.getGroup().getTimetable();
            List<TimeSlot> groupSlotsWithLesson = groupTT.getSlotsWithLesson();
            List<TimeSlot> slotsToChange = getTheSameSlots(groupSlotsWithLesson, freeSlotsToLesson.getSlots());
            
            //////////////////333333333/////////////////
            List<Lesson> choosenLesson = new List<Lesson>();
            Group group;
            Teacher teacher;
            Timetable currentTimeTable = new Timetable();
            List<TimeSlot> freeSlots = new List<TimeSlot>();
            List<TimeSlot> groupSlots = new List<TimeSlot>();
            List<TimeSlot> teacherSlots = new List<TimeSlot>();
            List<TimeSlot> theSameSlots = new List<TimeSlot>();
            List<FreeSlotsToLesson> fstl = new List<FreeSlotsToLesson>();

            foreach (Lesson l in choosenLesson)
            {
                group = l.getGroup();
                teacher = l.getTeacher();

                List<FreeSlotsInRoomToLesson> fsrtl = new List<FreeSlotsInRoomToLesson>();
                foreach (Timetable tt in roomsTimetables) {
                    if (tt.getRoom().getRoomType().Equals(l.getSubject().getRoomType()) && tt.getRoom().getAmount() >= group.getAmount()) {
                       fsrtl.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(l.getSize()), tt.getRoom()));
                    }
                }

                groupSlots = group.getTimetable().getFreeSlots(l.getSize());
                teacherSlots = teacher.getTimetable().getFreeSlots(l.getSize());
                theSameSlots = getTheSameSlots(groupSlots, teacherSlots);

                fstl.Add(new FreeSlotsToLesson(theSameSlots, l, fsrtl));

                foreach (TimeSlot ts in theSameSlots) {
                    currentTimeTable.addLesson(l, ts.day, ts.hour);
                    if (!freeSlots.Contains(ts)) {
                        freeSlots.Add(ts);
                    }
                }

            }

            fstl.Sort(new Comparison<FreeSlotsToLesson>(BFSComparator));
            //////////////////33333333333//////////////

            //////////////////444444444444444//////////////
            TimeSlot bestSlot = null;
            Room bestRoom = null;
            FreeSlotsToLesson tmpFstl = fstl.Last();
            Lesson lessonToMove = fstl.Last().lesson;

            if (tmpFstl.slots.Count > 0)
            {
                //bestSlot = tmpFstl.slots[rand.Next(tmpFstl.slots.Count-1)];
                bestSlot = getBestTimeSlot(tmpFstl.slots, tmpFstl.roomSlots, ref bestRoom);

                if (tmpFstl.lesson.getGroup().getTimetable().getLessons(bestSlot.day, bestSlot.hour).Count == 0)
                {
                    tmpFstl.lesson.getSlots().Add(new TimeSlot(bestSlot.day, bestSlot.hour ));
                    tmpFstl.lesson.setRoom(bestRoom);
                    tmpFstl.lesson.setLessonsToTimetable(bestSlot.day, bestSlot.hour);
                }
                else
                {
                    Console.WriteLine("ERROR!!!!!!!!!!!!!!!!");
                    result = removeLessonsAndFindNewPosition(groupTT.getLessons(bestSlot.day, bestSlot.hour)[0]);
                }
            }

            lessonToMove = groupTT.getLessons(bestSlot.day, bestSlot.hour)[0];

            groupTT.removeLesson(lessonToMove, bestSlot.day, bestSlot.hour);
            teacherTT.removeLesson(lessonToMove, bestSlot.day, bestSlot.hour);
            lessonToMove.getRoom().getTimetable().removeLesson(lessonToMove, bestSlot.day, bestSlot.hour);

            List<Room> freeRooms = new List<Room>();
            foreach (FreeSlotsInRoomToLesson ro in tmpFstl.roomSlots)
            {
                if (ro.slots.Contains(bestSlot))
                {
                    freeRooms.Add(ro.room);
                }
            }
            bestRoom = freeRooms[0];

            groupTT.addLesson(lesson, bestSlot.day, bestSlot.hour);
            teacherTT.addLesson(lesson, bestSlot.day, bestSlot.hour);
            lesson.setRoom(bestRoom);
            bestRoom.getTimetable().addLesson(lesson, bestSlot.day, bestSlot.hour);
            
            return result;
        }

        public List<TimeSlot> getTheSameSlots(List<TimeSlot> freeSlotsGroup, List<TimeSlot> freeSlotsTeacher)
        {
            List<TimeSlot> freeSlot = new List<TimeSlot>();

            int indexTeacher = 0;

            for (int indexGroup = 0; indexGroup < freeSlotsGroup.Count() && indexTeacher < freeSlotsTeacher.Count();)
            {
                if (freeSlotsGroup[indexGroup].Equals(freeSlotsTeacher[indexTeacher]))
                {
                    freeSlot.Add(new TimeSlot(freeSlotsGroup[indexGroup].day, freeSlotsGroup[indexGroup].hour));
                    indexTeacher++;
                    indexGroup++;
                }
                else
                {
                    if (freeSlotsGroup[indexGroup].day > freeSlotsTeacher[indexTeacher].day)
                    {
                        indexTeacher++;
                    }
                    else if (freeSlotsGroup[indexGroup].day < freeSlotsTeacher[indexTeacher].day)
                    {
                        indexGroup++;
                    }
                    else
                    {
                        if (freeSlotsGroup[indexGroup].hour > freeSlotsTeacher[indexTeacher].hour)
                        {
                            indexTeacher++;
                        }
                        else
                        {
                            indexGroup++;
                        }
                    }
                }
            }

            return freeSlot;
        }

        private TimeSlot getBestTimeSlot(List<TimeSlot> slots, List<FreeSlotsInRoomToLesson> roomSlots, ref Room bestRoom)
        {
            List<TimeSlot> bestTimeSlots = new List<TimeSlot>();//lista najlepszych slotów
            List<TimeSlot> worstTimeSlots = new List<TimeSlot>();//lista najgorszych slotów
            //podział slotów na 2 kategorie
            foreach (TimeSlot ts in slots)
            {
                if (ts.hour > BOTTOM_BORDER_OF_BEST_SLOTS && ts.hour < TOP_BORDER_OF_BEST_SLOTS) {
                    bestTimeSlots.Add(ts);
                } else {
                    worstTimeSlots.Add(ts);
                }
            }
            TimeSlot slot = null;
            //zwraca losowy slot z listy najlepszych o ile taki istnieje
            if (bestTimeSlots.Count > 0)
            {
                slot = bestTimeSlots[rand.Next(0, bestTimeSlots.Count - 1)];
            }
            else
            {
                slot = worstTimeSlots[rand.Next(0, worstTimeSlots.Count - 1)];
            }

            List<Room> freeRooms = new List<Room>();
            foreach (FreeSlotsInRoomToLesson ro in roomSlots)
            {
                if (ro.slots.Contains(slot))
                {
                    freeRooms.Add(ro.room);
                }
            }
            ///////////////////////////////
            bestRoom = freeRooms[0]; // wybranie najlepszej sali
            ///////////////////////////////

            return slot;
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

        public int BFSComparator(FreeSlotsToLesson o1, FreeSlotsToLesson o2)
        {
            if (o1.lesson.getSize() > o2.lesson.getSize())
            {
                return -1;
            }
            else if (o1.lesson.getSize() < o2.lesson.getSize())
            {
                return 1;
            }
            else
            {
                return o1.size.CompareTo(o2.size);
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