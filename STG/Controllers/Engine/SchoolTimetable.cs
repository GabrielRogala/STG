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
        private int numberOfSlots = 9;
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
        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>removeLessonsAndFindNewPosition");
            /////1
            Timetable groupTT = lesson.getGroup().getTimetable();
            List<TimeSlot> groupFreeSlots = groupTT.getFreeSlots(lesson.getSize());
            /////1

            /////2
            List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();

            foreach (Timetable tt in roomsTimetables) {
                if (tt.getRoom().getRoomType().Equals(lesson.getSubject().getRoomType()) && tt.getRoom().getAmount() >= lesson.getGroup().getAmount()) {
                    freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(lesson.getSize()), tt.getRoom()));
                }
            }
            /////2

            /////3
            FreeSlotsToLesson freeSlotsToLesson = new FreeSlotsToLesson(groupFreeSlots, lesson, freeSlotsInRoomToLesson);
            /////3
            bool result = false;
            if (freeSlotsToLesson.slots.Count == 0) {
                TimeSlot tmpSlot = freeSlotsToLesson.slots[rand.Next()];
                Lesson tmpLesson = groupTT.getLesson(tmpSlot.day, tmpSlot.hour);
                removeLessonsAndFindNewPosition(tmpLesson);

                tmpLesson.getGroup().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
                tmpLesson.getTeacher().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
                tmpLesson.getRoom().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);

                freeSlotsToLesson.slots.Add(new TimeSlot(tmpSlot.day, tmpSlot.hour));
            }
            /////4
            Timetable teacherTT = lesson.getGroup().getTimetable();
            List<TimeSlot> teacherSlotsWithLesson = teacherTT.getSlotsWithLesson(1);
            /////4

            /////5
            List<TimeSlot> slotsToChange = getTheSameSlots(teacherSlotsWithLesson, freeSlotsToLesson.getSlots());
            /////5

            /////6
            List<Lesson> choosenLesson = new List<Lesson>();
            foreach (TimeSlot ts in slotsToChange)
            {
                choosenLesson.Add(teacherTT.getLesson(ts.day,ts.hour));
            }
            /////6

            /////7
            Group group;
            Teacher teacher;
            Timetable currentTimeTable = new Timetable();
            List<TimeSlot> freeSlots = new List<TimeSlot>();
            List<TimeSlot> groupSlots = new List<TimeSlot>();
            List<TimeSlot> teacherSlots = new List<TimeSlot>();
            List<TimeSlot> theSameSlots = new List<TimeSlot>();
            List<FreeSlotsToLesson> fstl = new List<FreeSlotsToLesson>();
            int i = 0;
            foreach (Lesson l in choosenLesson)
            {
                group = l.getGroup();
                teacher = l.getTeacher();

                List<FreeSlotsInRoomToLesson> fsrtl = new List<FreeSlotsInRoomToLesson>();
                foreach (Timetable tt in roomsTimetables)
                {
                    if (tt.getRoom().getRoomType().Equals(l.getSubject().getRoomType()) && tt.getRoom().getAmount() >= group.getAmount())
                    {
                        fsrtl.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(l.getSize()), tt.getRoom()));
                    }
                }

                groupSlots = group.getTimetable().getFreeSlots(l.getSize());
                teacherSlots = teacher.getTimetable().getFreeSlots(l.getSize());
                theSameSlots = getTheSameSlots(groupSlots, teacherSlots);

                fstl.Add(new FreeSlotsToLesson(theSameSlots, l, fsrtl, slotsToChange[i++]));
            }

            fstl.Sort(new Comparison<FreeSlotsToLesson>(BFSComparator));
            /////7

            /////8

            foreach (TimeSlot ts in slotsToChange)
            {
                choosenLesson.Add(teacherTT.getLesson(ts.day, ts.hour));
            }

            TimeSlot bestSlot = null;
            TimeSlot prevSlot = null;
            Room bestRoom = null;
            FreeSlotsToLesson tmpFstl = null;
            Lesson lessonToMove = null;

            if (fstl.Count == 0) {
                Console.WriteLine("ERROR LVL 3");
                return false;
            }

            tmpFstl = fstl[fstl.Count - 1];
            lessonToMove = tmpFstl.lesson;

            if (tmpFstl.slots.Count > 0) {
                bestSlot = getBestTimeSlot(tmpFstl.slots, tmpFstl.roomSlots, ref bestRoom);
                tmpFstl.lesson.getSlots().Add(new TimeSlot(bestSlot.day, bestSlot.hour));
                tmpFstl.lesson.setRoom(bestRoom);
                tmpFstl.lesson.setLessonsToTimetable(bestSlot.day, bestSlot.hour);
            } else {
                //remove
                Console.WriteLine("ERROR lvl1");
            }

            lessonToMove = groupTT.getLessons(bestSlot.day, bestSlot.hour)[0];

            groupTT.removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
            teacherTT.removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
            lessonToMove.getRoom().getTimetable().removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);

            List<Room> freeRooms = new List<Room>();
            foreach (FreeSlotsInRoomToLesson ro in tmpFstl.roomSlots)
            {
                if (ro.slots.Contains(bestSlot))
                {
                    freeRooms.Add(ro.room);
                }
            }
            bestRoom = freeRooms[rand.Next(freeRooms.Count - 1)];

            groupTT.addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
            teacherTT.addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
            lesson.setRoom(bestRoom);
            bestRoom.getTimetable().addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);

            Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<removeLessonsAndFindNewPosition");
            return true;       
        }

        public Boolean removeLessonsAndFindNewPosition2(Lesson lesson)
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>removeLessonsAndFindNewPosition");

            bool result = false;

            Timetable teacherTT = lesson.getTeacher().getTimetable();

            Console.WriteLine(lesson.ToString());
            Console.WriteLine(teacherTT.ToString());
            Console.WriteLine(lesson.getGroup().getTimetable().ToString());

            List<TimeSlot> teacherFreeSlots = teacherTT.getFreeSlots(lesson.getSize());

            Console.WriteLine("#free slots :");
            foreach (TimeSlot ts in teacherFreeSlots)
            {
                Console.WriteLine(ts.ToString());
            }
            Console.WriteLine("------------");

            Console.WriteLine("check room :");
            List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();
            foreach (Timetable tt in roomsTimetables)
            {
                Console.WriteLine(tt.getRoom().ToString());
                if (tt.getRoom().getRoomType().Equals(lesson.getSubject().getRoomType()) && tt.getRoom().getAmount() >= lesson.getGroup().getAmount())
                {
                    Console.WriteLine(tt.getRoom().getTimetable().ToString());
                    freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(lesson.getSize()), tt.getRoom()));
                }
            }
            FreeSlotsToLesson freeSlotsToLesson = new FreeSlotsToLesson(teacherFreeSlots, lesson, freeSlotsInRoomToLesson);
            Console.WriteLine("------------");

            Console.WriteLine("----------------------------");
            foreach (FreeSlotsInRoomToLesson fsi in freeSlotsToLesson.roomSlots)
            {
                Console.WriteLine("------------");
                foreach (TimeSlot ts in fsi.slots)
                {
                    Console.WriteLine(ts.ToString());
                }
            }
            Console.WriteLine("------------ : --");
            foreach (TimeSlot ts in freeSlotsToLesson.slots)
            {
                Console.WriteLine(ts.ToString());
            }
            Console.WriteLine("----------------------------");

            Timetable groupTT = lesson.getGroup().getTimetable();
            Console.WriteLine(groupTT.ToString());
            List<TimeSlot> groupSlotsWithLesson = groupTT.getSlotsWithLesson(1);
            Console.WriteLine("#slots with lesson :");
            foreach (TimeSlot ts in groupSlotsWithLesson)
            {
                Console.WriteLine(ts.ToString());
            }
            Console.WriteLine("------------");
            List<TimeSlot> slotsToChange = getTheSameSlots(groupSlotsWithLesson, freeSlotsToLesson.getSlots());
            Console.WriteLine("#slots to change :");
            foreach (TimeSlot ts in slotsToChange)
            {
                Console.WriteLine(ts.ToString());
            }
            Console.WriteLine("------------");
            //////////////////333333333/////////////////

            List<Lesson> choosenLesson = new List<Lesson>();
            foreach (TimeSlot ts in slotsToChange)
            {
                choosenLesson.Add(groupTT.getLesson(ts.day, ts.hour));
            }

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

                Console.WriteLine("check : " + l.ToString());

                group = l.getGroup();
                teacher = l.getTeacher();

                Console.WriteLine(group.getTimetable().ToString());
                Console.WriteLine(teacher.getTimetable().ToString());

                List<FreeSlotsInRoomToLesson> fsrtl = new List<FreeSlotsInRoomToLesson>();
                foreach (Timetable tt in roomsTimetables)
                {
                    if (tt.getRoom().getRoomType().Equals(l.getSubject().getRoomType()) && tt.getRoom().getAmount() >= group.getAmount())
                    {
                        fsrtl.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlots(l.getSize()), tt.getRoom()));
                    }
                }

                groupSlots = group.getTimetable().getFreeSlots(l.getSize());
                teacherSlots = teacher.getTimetable().getFreeSlots(l.getSize());
                theSameSlots = getTheSameSlots(groupSlots, teacherSlots);

                fstl.Add(new FreeSlotsToLesson(theSameSlots, l, fsrtl));

                Console.WriteLine("# avaliable slots to move :");
                foreach (FreeSlotsToLesson f in fstl)
                {
                    Console.WriteLine(f.ToString());
                }
                Console.WriteLine("---------------------------");

            }

            fstl.Sort(new Comparison<FreeSlotsToLesson>(BFSComparator));
            //////////////////33333333333//////////////

            //////////////////444444444444444//////////////
            TimeSlot bestSlot = null;
            Room bestRoom = null;
            FreeSlotsToLesson tmpFstl = null;
            Lesson lessonToMove = null;

            if (fstl.Count > 0)
            { // czy sa dostepne lekcje
                tmpFstl = fstl.Last();
                lessonToMove = fstl.Last().lesson;

                if (tmpFstl.slots.Count > 0)
                {
                    bestSlot = getBestTimeSlot(tmpFstl.slots, tmpFstl.roomSlots, ref bestRoom);

                    //if (tmpFstl.lesson.getGroup().getTimetable().getLessons(bestSlot.day, bestSlot.hour).Count == 0)
                    //{
                    tmpFstl.lesson.getSlots().Add(new TimeSlot(bestSlot.day, bestSlot.hour));
                    tmpFstl.lesson.setRoom(bestRoom);
                    tmpFstl.lesson.setLessonsToTimetable(bestSlot.day, bestSlot.hour);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("ERROR!!!!!!!!!!!!!!!!");
                    //    result = removeLessonsAndFindNewPosition(groupTT.getLessons(bestSlot.day, bestSlot.hour)[0]);
                    //}
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
                bestRoom = freeRooms[rand.Next(freeRooms.Count - 1)];

                groupTT.addLesson(lesson, bestSlot.day, bestSlot.hour);
                teacherTT.addLesson(lesson, bestSlot.day, bestSlot.hour);
                lesson.setRoom(bestRoom);
                bestRoom.getTimetable().addLesson(lesson, bestSlot.day, bestSlot.hour);

                Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<removeLessonsAndFindNewPosition");
                return result;


            }
            else
            {
                Console.WriteLine("##############################");
                return false;
            }
        }

        public List<TimeSlot> getTheSameSlots(List<TimeSlot> freeSlots1, List<TimeSlot> freeSlots2)
        {
            List<TimeSlot> freeSlot = new List<TimeSlot>();

            foreach (TimeSlot ts in freeSlots1)
            {
                if (freeSlots2.Contains(ts))
                {
                    freeSlot.Add(new TimeSlot(ts.day, ts.hour));
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
            bestRoom = freeRooms[rand.Next(freeRooms.Count - 1)]; // wybranie najlepszej sali
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