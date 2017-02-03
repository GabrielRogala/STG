using System;
using System.Collections.Generic;
using System.IO;
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

        public SchoolTimetable(SchoolTimetable s) : this(s.getTeachers(), s.getGroups(), s.getRooms(), s.getLessons(), SubjectType.getInstance().getTypes(), s.getNumberOfDays(), s.getNumberOfSlots())
        {
              // wygenerowanie takiego samego planu jak w s               
        }

        public List<Teacher> getTeachers() {
            return teachers;
        }

        public List<Group> getGroups()
        {
            return groups;
        }

        public List<Room> getRooms()
        {
            return rooms;
        }

        public List<Lesson> getLessons()
        {
            return lessons;
        }

        public int getNumberOfDays() {
            return numberOfDays;
        }

        public int getNumberOfSlots() {
            return numberOfSlots;
        }

        public void generateTeachersTimetables(List<Teacher> teachers) {
            foreach (Teacher t in teachers) {
                teachersTimetables.Add(new Timetable(t, numberOfDays,numberOfSlots));
            }
        }

        public void generateGroupsTimetables(List<Group> groups)
        {
            foreach (Group g in groups)
            {
                if (g.getSubGroup().Count > 0) {
                    foreach (Group sg in g.getSubGroup())
                    {
                        groupsTimetables.Add(new Timetable(sg, numberOfDays, numberOfSlots));
                    }
                }
                groupsTimetables.Add(new Timetable(g, numberOfDays, numberOfSlots));
            }
        }

        public void generateRoomsTimetables(List<Room> rooms)
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

        public void genereteTimetable(List<Lesson> lessons)
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
                findAndSetBestPositionToLessons(choosenLesson, tmpLessons);

                foreach (Lesson l in choosenLesson) {
                    tmpLessons.Remove(l);
                }
                choosenLesson.Clear();
            }

        }

        private void findAndSetBestPositionToLessons(List<Lesson> choosenLesson, List<Lesson> allLesson)
        {
            Console.WriteLine("=================================");
            //foreach (Lesson l in choosenLesson)
            //{
            //    Console.WriteLine(l.ToString());
            //}

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
                        freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlotsToLesson(l), tt.getRoom()));
                    }
                }

                groupSlots = group.getTimetable().getFreeSlotsToLesson(l);
                teacherSlots = teacher.getTimetable().getFreeSlotsToLesson(l);
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
                Console.WriteLine(fstl.lesson.ToString());

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
                            //fstl.lesson.addLessonToTimetable(bestSlot.day, bestSlot.hour + i);

                            fstl.lesson.getGroup().addLesson(fstl.lesson, bestSlot.day, bestSlot.hour + i);
                            fstl.lesson.getTeacher().getTimetable().addLesson(fstl.lesson, bestSlot.day, bestSlot.hour + i);
                            fstl.lesson.getRoom().getTimetable().addLesson(fstl.lesson, bestSlot.day, bestSlot.hour + i);
                        }
                        else
                        {
                            allLesson.Add(fstl.lesson);
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

                    Console.WriteLine("REMOVE " + fstl.lesson);
                    //removeLessonsAndFindNewPosition3(fstl.lesson, ref allLesson);
                }
            }

        }

        //public Boolean removeLessonsAndFindNewPosition3(Lesson lesson, ref List<Lesson> allLesson)
        //{
        //    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>removeLessonsAndFindNewPosition");
        //    Console.WriteLine("start "+lesson.ToString());

            

        //    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<removeLessonsAndFindNewPosition");
        //    return true;
        //}

        //public Boolean removeLessonsAndFindNewPosition(Lesson lesson)
        //{
        //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>removeLessonsAndFindNewPosition");
        //    Console.WriteLine(lesson.ToString());
            
        //    /////1
        //    Timetable groupTT = lesson.getGroup().getTimetable();
        //    List<TimeSlot> groupFreeSlots = groupTT.getFreeSlotsToLesson(lesson);
        //    /////1

        //    /////2
        //    List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();

        //    foreach (Timetable tt in roomsTimetables) {
        //        if (tt.getRoom().getRoomType().Equals(lesson.getSubject().getRoomType()) && tt.getRoom().getAmount() >= lesson.getGroup().getAmount()) {
        //            freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlotsToLesson(lesson), tt.getRoom()));
        //        }
        //    }
        //    /////2

        //    /////3
        //    FreeSlotsToLesson freeSlotsToLesson = new FreeSlotsToLesson(groupFreeSlots, lesson, freeSlotsInRoomToLesson);
        //    /////3

        //    /////9
        //    bool result = false;
        //    if (freeSlotsToLesson.slots.Count == 0) {
        //        Console.WriteLine("ERROR LVL 1");
        //        //wolny slot grupy zwalniany jest w losowej sali
        //        TimeSlot tmpSlot = groupFreeSlots[rand.Next(groupFreeSlots.Count -1)];
        //        Lesson tmpLesson = freeSlotsToLesson.roomSlots[rand.Next(freeSlotsToLesson.roomSlots.Count - 1)].room.getTimetable().getLesson(tmpSlot.day, tmpSlot.hour);

        //        tmpLesson.getGroup().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);

        //        removeLessonsAndFindNewPosition(tmpLesson);

        //        tmpLesson.getGroup().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);

        //        tmpLesson.getGroup().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);

        //        freeSlotsToLesson.slots.Add(new TimeSlot(tmpSlot.day, tmpSlot.hour));
        //    }
        //    /////9

        //    /////4
        //    Timetable teacherTT = lesson.getGroup().getTimetable();
        //    List<TimeSlot> teacherSlotsWithLesson = teacherTT.getSlotsWithLesson(1);
        //    /////4

        //    /////5
        //    List<TimeSlot> slotsToChange = getTheSameSlots(teacherSlotsWithLesson, freeSlotsToLesson.getSlots());
        //    /////5

        //    /////10
        //    if (slotsToChange.Count == 0) {
        //        Console.WriteLine("ERROR LVL 2");

        //        TimeSlot tmpSlot = teacherSlotsWithLesson[rand.Next(teacherSlotsWithLesson.Count - 1)];
        //        //sprawdzic czy w slocie jedy odpowiednia lekcja
        //        Lesson tmpLesson = freeSlotsToLesson.roomSlots[rand.Next(freeSlotsToLesson.roomSlots.Count - 1)].room.getTimetable().getLesson(tmpSlot.day, tmpSlot.hour);

        //        tmpLesson.getGroup().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);

        //        removeLessonsAndFindNewPosition(tmpLesson);

        //        tmpLesson.getGroup().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().unlockSlot(tmpSlot.day, tmpSlot.hour);

        //        tmpLesson.getGroup().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);


        //        teacherSlotsWithLesson = teacherTT.getSlotsWithLesson(1);

        //        slotsToChange = getTheSameSlots(teacherSlotsWithLesson, freeSlotsToLesson.getSlots());
        //    }
        //    /////10

        //    /////6
        //    List<Lesson> choosenLesson = new List<Lesson>();
        //    foreach (TimeSlot ts in slotsToChange)
        //    {
        //        choosenLesson.Add(teacherTT.getLesson(ts.day,ts.hour));
        //    }
        //    /////6

        //    /////7
        //    Group group;
        //    Teacher teacher;
        //    Timetable currentTimeTable = new Timetable();
        //    List<TimeSlot> freeSlots = new List<TimeSlot>();
        //    List<TimeSlot> groupSlots = new List<TimeSlot>();
        //    List<TimeSlot> teacherSlots = new List<TimeSlot>();
        //    List<TimeSlot> theSameSlots = new List<TimeSlot>();
        //    List<FreeSlotsToLesson> fstl = new List<FreeSlotsToLesson>();
        //    int i = 0;
        //    foreach (Lesson l in choosenLesson)
        //    {
        //        group = l.getGroup();
        //        teacher = l.getTeacher();

        //        List<FreeSlotsInRoomToLesson> fsrtl = new List<FreeSlotsInRoomToLesson>();
        //        foreach (Timetable tt in roomsTimetables)
        //        {
        //            if (tt.getRoom().getRoomType().Equals(l.getSubject().getRoomType()) && tt.getRoom().getAmount() >= group.getAmount())
        //            {
        //                fsrtl.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlotsToLesson(l), tt.getRoom()));
        //            }
        //        }

        //        groupSlots = group.getTimetable().getFreeSlotsToLesson(l);
        //        teacherSlots = teacher.getTimetable().getFreeSlotsToLesson(l);
        //        theSameSlots = getTheSameSlots(groupSlots, teacherSlots);

        //        fstl.Add(new FreeSlotsToLesson(theSameSlots, l, fsrtl, slotsToChange[i++]));
        //    }

        //    fstl.Sort(new Comparison<FreeSlotsToLesson>(BFSComparator));
        //    /////7

        //    /////11
        //    if (fstl.Count == 0)
        //    {
        //        Console.WriteLine("ERROR LVL 3");
        //        return false;
        //        //losowanie lekcji z 6:
        //        //int index = rand.Next(choosenLesson.Count - 1);
        //        //Lesson tmpLesson = choosenLesson[index];
        //        //TimeSlot tmpSlot = slotsToChange[index];

        //        //removeLessonsAndFindNewPosition(tmpLesson);

        //        //tmpLesson.getGroup().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        //tmpLesson.getTeacher().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        //tmpLesson.getRoom().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);

        //        //freeSlotsToLesson.slots.Add(new TimeSlot(tmpSlot.day, tmpSlot.hour));

        //    }
        //    /////11

        //    /////8

        //    foreach (TimeSlot ts in slotsToChange)
        //    {
        //        choosenLesson.Add(teacherTT.getLesson(ts.day, ts.hour));
        //    }

        //    TimeSlot bestSlot = null;
        //    TimeSlot prevSlot = null;
        //    Room bestRoom = null;
        //    FreeSlotsToLesson tmpFstl = null;
        //    Lesson lessonToMove = null;

        //    tmpFstl = fstl[fstl.Count - 1];
        //    lessonToMove = tmpFstl.lesson;

        //    if (tmpFstl.slots.Count > 0) {
        //        bestSlot = getBestTimeSlot(tmpFstl.slots, tmpFstl.roomSlots, ref bestRoom);
        //        tmpFstl.lesson.getSlots().Add(new TimeSlot(bestSlot.day, bestSlot.hour));
        //        tmpFstl.lesson.setRoom(bestRoom);
        //        tmpFstl.lesson.setLessonsToTimetable(bestSlot.day, bestSlot.hour);
        //    } else {
        //        //remove
        //        Console.WriteLine("ERROR LVL 4");
        //        return false;
        //    }

        //    lessonToMove = groupTT.getLessons(bestSlot.day, bestSlot.hour)[0];

        //    groupTT.removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
        //    teacherTT.removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
        //    lessonToMove.getRoom().getTimetable().removeLesson(lessonToMove, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);

        //    List<Room> freeRooms = new List<Room>();
        //    foreach (FreeSlotsInRoomToLesson ro in tmpFstl.roomSlots)
        //    {
        //        if (ro.slots.Contains(bestSlot))
        //        {
        //            freeRooms.Add(ro.room);
        //        }
        //    }
        //    bestRoom = freeRooms[rand.Next(freeRooms.Count - 1)];

        //    groupTT.addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
        //    teacherTT.addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);
        //    lesson.setRoom(bestRoom);
        //    bestRoom.getTimetable().addLesson(lesson, tmpFstl.pravSlot.day, tmpFstl.pravSlot.hour);

        //    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<removeLessonsAndFindNewPosition");
        //    return true;       
        //}

        //public Boolean removeLessonsAndFindNewPosition2(Lesson lesson,ref List<Lesson> allLesson)
        //{
        //    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>removeLessonsAndFindNewPosition");
        //    Console.WriteLine(lesson.ToString());

        //    Timetable groupTT = lesson.getGroup().getTimetable();
        //    Timetable teacherTT = lesson.getTeacher().getTimetable();
        //    List<TimeSlot> groupFreeSlots = groupTT.getFreeSlotsToLesson(lesson);
        //    List<TimeSlot> teacherFreeSlots = teacherTT.getFreeSlotsToLesson(lesson);
        //    Timetable tmpTT;
        //    FreeSlotsToLesson tmpFreeSlots;

        //    List<FreeSlotsInRoomToLesson> freeSlotsInRoomToLesson = new List<FreeSlotsInRoomToLesson>();

        //    foreach (Timetable tt in roomsTimetables)
        //    {
        //        if (tt.getRoom().getRoomType().Equals(lesson.getSubject().getRoomType()) && tt.getRoom().getAmount() >= lesson.getGroup().getAmount())
        //        {
        //            freeSlotsInRoomToLesson.Add(new FreeSlotsInRoomToLesson(tt.getFreeSlotsToLesson(lesson), tt.getRoom()));
        //        }
        //    }

        //    FreeSlotsToLesson freeSlotsToLessonG = new FreeSlotsToLesson(groupFreeSlots, lesson, freeSlotsInRoomToLesson);
        //    FreeSlotsToLesson freeSlotsToLessonT = new FreeSlotsToLesson(teacherFreeSlots, lesson, freeSlotsInRoomToLesson);

        //    Console.WriteLine(freeSlotsToLessonG.slots.Count + " < "+ freeSlotsToLessonT.slots.Count);

        //    // losuje slot z sal, zwalnia slot z nauczyciela

        //    if (freeSlotsToLessonG.slots.Count < freeSlotsToLessonT.slots.Count) {
        //        tmpTT = groupTT;
        //        tmpFreeSlots = freeSlotsToLessonT;
        //    } else {
        //        tmpTT = teacherTT;
        //        tmpFreeSlots = freeSlotsToLessonG;
        //    }

        //    if (freeSlotsToLessonG.slots.Count == 0) {
        //        List<TimeSlot> tmpRoomSlots = freeSlotsToLessonG.getSlotsFromRoomLists();
        //        TimeSlot ts = tmpRoomSlots[rand.Next(tmpRoomSlots.Count)];
        //        Lesson tmpL = groupTT.getLesson(ts.day, ts.hour);

        //        allLesson.Add(tmpL);

        //        tmpL.getGroup().getTimetable().removeLesson(tmpL, ts.day, ts.hour);
        //        tmpL.getTeacher().getTimetable().removeLesson(tmpL, ts.day, ts.hour);
        //        tmpL.getRoom().getTimetable().removeLesson(tmpL, ts.day, ts.hour);

        //        freeSlotsToLessonG.slots.Add(ts);
        //        tmpTT = teacherTT;
        //        tmpFreeSlots = freeSlotsToLessonG;
        //    }

        //    Room bestRoom = null;
        //    Lesson tmpLesson = null;
        //    TimeSlot tmpSlot = getBestTimeSlot(tmpFreeSlots.slots, tmpFreeSlots.roomSlots, ref bestRoom);

        //    if (tmpTT.getLessons(tmpSlot.day, tmpSlot.hour).Count > 0) { // w przypadku gdyby wczesniej usunieta lecka zwolnila slot dla lekcji do wstawienia
        //        tmpLesson = tmpTT.getLesson(tmpSlot.day, tmpSlot.hour);

        //        Console.WriteLine(tmpSlot.ToString());
        //        Console.WriteLine(tmpLesson.ToString());

        //        tmpLesson.getGroup().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getTeacher().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);
        //        tmpLesson.getRoom().getTimetable().removeLesson(tmpLesson, tmpSlot.day, tmpSlot.hour);

        //        allLesson.Add(tmpLesson);
        //    }
                      
        //    lesson.setRoom(bestRoom);

        //    lesson.getGroup().getTimetable().addLesson(lesson, tmpSlot.day, tmpSlot.hour);
        //    lesson.getTeacher().getTimetable().addLesson(lesson, tmpSlot.day, tmpSlot.hour);
        //    lesson.getRoom().getTimetable().addLesson(lesson, tmpSlot.day, tmpSlot.hour);

        //    lesson.getGroup().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //    lesson.getTeacher().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);
        //    lesson.getRoom().getTimetable().lockSlot(tmpSlot.day, tmpSlot.hour);

        //    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<removeLessonsAndFindNewPosition");
        //    return true;
        //}

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
                slot = bestTimeSlots[rand.Next(bestTimeSlots.Count - 1)];
            }
            else
            {
                slot = worstTimeSlots[rand.Next(worstTimeSlots.Count - 1)];
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
            return subGroupLessonComparator(l1, l2);
        }

        public int subGroupLessonComparator(Lesson l1, Lesson l2)
        {
            if (l1.getGroup().getParent() != null) {
                if (l2.getGroup().getParent() != null) {
                    return l1.getGroup().getParent().GetHashCode() - l2.getGroup().getParent().GetHashCode();
                } else {
                    return 1;
                }
                
            } else if (l2.getGroup().getParent() != null) {
                return -1;
            } else {
                return sizeLessonComparator(l1, l2);
            }
           
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
                if (g.getSubGroup().Count > 0) {
                    foreach (Group sg in g.getSubGroup())
                    {
                        Console.WriteLine(sg.getTimetable().ToString());
                    }
                }
            }

            foreach (Room r in rooms)
            {
                Console.WriteLine(r.getTimetable().ToString());
            }
        }

        public int fitness() {
            return 0;
        }

        public List<Lesson> getLessonsForGroup(Group gr) {
            List<Lesson> tmpLessons = new List<Lesson>();
            foreach (Lesson l in lessons) {
                if (l.getGroup().Equals(gr) || (gr.getParent() != null  && l.getGroup().Equals(gr.getParent()))) {
                    tmpLessons.Add(l);
                }
            }
            return tmpLessons;
        }

        public bool isCorrect() {
            List<Lesson> tmpLessons = new List<Lesson>();

            foreach (Group g in groups) {
                List<Lesson> lessonsInGroupTimetable = g.getTimetable().getAllLessonsWithTimetable();
                foreach (Lesson l in getLessonsForGroup(g)) {
                    if (lessonsInGroupTimetable.Contains(l)) {
                        lessonsInGroupTimetable.Remove(l);
                    } else {
                        Console.WriteLine(l.ToString() + ": does not exist in timetable");
                        return false;
                    }
                }
            }
            
            return true;
        }

        public void cross(SchoolTimetable s1, SchoolTimetable s2) {

        }

        public void mutate() {

        }

        public void genWeb(string fileName = "output")
        {

            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\INZ\STG\STG\webExport";
            string beginStr = "";
            string endStr = "";
            string contentStr = "";

            beginStr += "<!DOCTYPE html><html><head>\n";
            beginStr += "<link href = \""+ mydocpath + "/Content/bootstrap.css\" rel = \"stylesheet\" />\n";
            beginStr += "<link href = \"" + mydocpath + "/Content/site.css\" rel = \"stylesheet\" />\n";
            beginStr += "<script src = \"" + mydocpath + "/Scripts/modernizr-2.6.2.js\"></script>\n";
            beginStr += "</head><body>\n";
            beginStr += "<div class=\"container body-content\">\n";
        
            endStr += "</div>\n<script src = \"" + mydocpath + "/Scripts/jquery-1.10.2.js\"></script>\n";
            endStr += "<script src= \"" + mydocpath + "/Scripts/bootstrap.js\"></script>\n";
            endStr += "<script src= \"" + mydocpath + "/Scripts/respond.js\"></script>\n";
            endStr += "</body></html>\n";

            
            foreach (Timetable tt in this.teachersTimetables) {
                contentStr += "<div class=\"row\"> \n <div class=\"panel panel-default\"><div class=\"panel-body\"> <table class=\"table\">\n";
                ///////////////////////////////////////////////
                contentStr += "<h3>" + tt.getTeacher().getName() + "</h3>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                contentStr += "<td>Hour:</td>\n";
                foreach (Day d in tt.getDays()) {
                    contentStr += "<td>" +d.getName() + "</td>\n";
                }
                contentStr += "</tr>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                for (int h=0 ; h < numberOfSlots; h++ ) {
                    contentStr += "<td>" + h + "</td>\n";
                    for (int d = 0; d < numberOfDays; d++)
                    {
                        contentStr += "<td>";
                        foreach (Lesson l in tt.getLessons(d,h)) {
                            contentStr += "<ul>";
                            contentStr += "<li>" + l.getSubject().getName() + "</li> \n";
                            contentStr += "<li>" + l.getGroup().getName() + "</li> \n";
                            contentStr += "<li>" + l.getTeacher().getName() + "</li> \n";
                            contentStr += "<li>" + l.getRoom().getName() + "</li> \n";
                            contentStr += "<hr>\n";
                            contentStr += "</ul>";
                        }
                        contentStr += "</td>\n";
                    }
                    contentStr += "</tr>\n";
                }
                
                //=============================================

                ///////////////////////////////////////////////
                contentStr += "</table> \n </div></div></div>";
            }

            foreach (Timetable tt in this.groupsTimetables)
            {
                contentStr += "<div class=\"row\"> \n <div class=\"panel panel-default\"><div class=\"panel-body\"> <table class=\"table\">\n";
                ///////////////////////////////////////////////
                contentStr += "<h3>" + tt.getGroup().getName() + "</h3>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                contentStr += "<td>Hour:</td>\n";
                foreach (Day d in tt.getDays())
                {
                    contentStr += "<td>" + d.getName() + "</td>\n";
                }
                contentStr += "</tr>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                for (int h = 0; h < numberOfSlots; h++)
                {
                    contentStr += "<td>" + h + "</td>\n";
                    for (int d = 0; d < numberOfDays; d++)
                    {
                        contentStr += "<td>";
                        foreach (Lesson l in tt.getLessons(d, h))
                        {
                            contentStr += "<li>" + l.getSubject().getName() + "</li> \n";
                            contentStr += "<li>" + l.getGroup().getName() + "</li> \n";
                            contentStr += "<li>" + l.getTeacher().getName() + "</li> \n";
                            contentStr += "<li>" + l.getRoom().getName() + "</li> \n";
                            contentStr += "<hr>\n";
                        }
                        contentStr += "</td>\n";
                    }
                    contentStr += "</tr>\n";
                }
                //=============================================

                ///////////////////////////////////////////////
                contentStr += "</table> \n </div></div></div>";
            }

            foreach (Timetable tt in this.roomsTimetables)
            {
                contentStr += "<div class=\"row\"> \n <div class=\"panel panel-default\"><div class=\"panel-body\"> <table class=\"table\">\n";
                ///////////////////////////////////////////////
                contentStr += "<h3>" + tt.getRoom().getName() + "</h3>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                contentStr += "<td>Hour:</td>\n";
                foreach (Day d in tt.getDays())
                {
                    contentStr += "<td>" + d.getName() + "</td>\n";
                }
                contentStr += "</tr>\n";
                //--------------------------------------------
                contentStr += "<tr class=\"info\">\n";
                for (int h = 0; h < numberOfSlots; h++)
                {
                    contentStr += "<td>" + h + "</td>\n";
                    for (int d = 0; d < numberOfDays; d++)
                    {
                        contentStr += "<td>";
                        foreach (Lesson l in tt.getLessons(d, h))
                        {
                            contentStr += "<li>" + l.getSubject().getName() + "</li> \n";
                            contentStr += "<li>" + l.getGroup().getName() + "</li> \n";
                            contentStr += "<li>" + l.getTeacher().getName() + "</li> \n";
                            contentStr += "<li>" + l.getRoom().getName() + "</li> \n";
                            contentStr += "<hr>\n";
                        }
                        contentStr += "</td>\n";
                    }
                    contentStr += "</tr>\n";
                }
               
                //=============================================

                ///////////////////////////////////////////////
                contentStr += "</table> \n </div></div></div>";
            }

            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\" + fileName+ ".html"))
            {
                outputFile.WriteLine(beginStr + contentStr + endStr);
            }


        }

    }
}