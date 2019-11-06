using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;
using PDFQuiz;
using System.Data.Entity;

using System.Collections.ObjectModel;
using AME_base;
using System.Collections.Specialized;

namespace EF
{
    public class CourseStudentGroup : ITeachingGroup
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual Course Course { get; set; }
        [Required]
        public virtual StudentGroup StudentGroup { get; set; }
        [Required]
        public virtual CovariantObservableCollection<Deadline> Deadlines { get; set; }

        //grid data
        [Required]
        [Index]
        public virtual CovariantObservableCollection<Attempt> Attempts { get; set; }
        [Required]
        [Index]
        public virtual CovariantObservableCollection<Mark> Marks { get; set; }

#region Thresholds

        [Required]
        public virtual string ThresholdsAsString { get; set; }
        [Required] 
        public virtual bool ThresholdsAreAbsolute { get; set; }
        private Thresholds _thresholds;
        [NotMapped]
        public Thresholds thresholds
        {
            get
            {
                if (_thresholds == null)
                    _thresholds = new Thresholds(ThresholdsAreAbsolute, ThresholdsAsString);
                return _thresholds;
            }
            set
            {
                ThresholdsAsString = value.toString();
            }
        }

#endregion Thresholds

#region grid visibility
        [Required]
        public virtual int visibleGrids { get; set; }
        public bool isGridVisible(int index)
        {
            return (visibleGrids & (int)Math.Pow(2, index)) == Math.Pow(2, index);
        }
        public void setGridVisibility(int index, bool isVisible)
        {
            visibleGrids = isVisible ? 
                visibleGrids | (int)Math.Pow(2,index) :
                visibleGrids & ~(int)Math.Pow(2,index) ;
        }
#endregion

        //interface contains no data 
        //public CourseStudentGroup(ICourseStudentGroup ics, Course c, StudentGroup sg)
        //{
        //    Course = c;
        //    StudentGroup = sg;
        //    DO NOT ADD CHILD ENTITIES HERE, ADD IN DB GETCOURSE
        //    Deadlines = new CovariantObservableCollection<Deadline>();
        //    Attempts = new CovariantObservableCollection<Attempt>();
        //    Marks = new CovariantObservableCollection<Mark>();
        //    thresholds = new Thresholds(true);
        //    visibleGrids = 1;
        //}

        public CourseStudentGroup()
        {
            Deadlines = new CovariantObservableCollection<Deadline>();
            Attempts = new CovariantObservableCollection<Attempt>();
            Marks = new CovariantObservableCollection<Mark>();
            thresholds = new Thresholds(true);
            visibleGrids = 1;
        }

        //private CourseStudentGroup()
        //{
        //    Deadlines = new CovariantObservableCollection<Deadline>();
        //    Attempts = new CovariantObservableCollection<Attempt>();
        //    Marks = new CovariantObservableCollection<Mark>();
        //    thresholds = new Thresholds(true);
        //    visibleGrids = 1;
        //}


        private GridData<Deadline, Student, Attempt> _attemptsGrid;
        public GridData<Deadline, Student, Attempt> attemptsGrid
        {
            get
            {
                if (_attemptsGrid == null)
                _attemptsGrid = new GridData<Deadline, Student, Attempt>(
                    delegate()
                    {
                        return new ObservableCollection<Deadline>
                        (
                            from Deadline d in Deadlines
                            orderby d.TCSDeadlineColumn
                            select d
                        );
                    },
                    delegate(IEnumerable<Deadline> deadlines)
                    {
                        ObservableCollection<CovariantObservableGrouping<Student, ObservableCollection<Attempt>>> _gridRows =
                            new ObservableCollection<CovariantObservableGrouping<Student, ObservableCollection<Attempt>>>
                            (
                                from Student s in this.StudentGroup.Students
                                orderby s.TCSStudentRow
                                select new CovariantObservableGrouping<Student, ObservableCollection<Attempt>> 
                                (
                                    s,
                                    deadlines.Select(d => new ObservableCollection<Attempt>())
                                )
                            );

                        //add attempts
                        foreach (Attempt a in this.Attempts)
                            _gridRows[a.TCSStudentRow][a.TCSDeadlineColumn].Add(a);

                        return _gridRows;
                    },
                    "attempts",
                    delegate(GridData<Deadline, Student, Attempt> gridData)
                    {
                        return new LineChart<Deadline, Student, Attempt>
                            (
                                thresholds,
                                gridData,
                                delegate(Deadline d) { return d.Assignment.Name + " " + d.whenHandedOut; },
                                delegate(Attempt a) { return a.PercentScore; },
                                delegate(Attempt a) { return a.Deadline; }
                            );
                    }
                    );
                return _attemptsGrid;
            }
        }

        private GridData<Outcome, Student, IMark> _marksGrid;
        public GridData<Outcome, Student, IMark> marksGrid
        {
            get
            {
                if (_marksGrid == null)
                    _marksGrid = new GridData<Outcome, Student, IMark>(
                        delegate()
                        {
                            return new ObservableCollection<Outcome>
                            (
                                (
                                from d in this.Deadlines
                                from o in d.Assignment.Outcomes
                                orderby o.TCSOutcomeColumn
                                select o
                                ).Distinct()
                            );
                        },
                        delegate(IEnumerable<Outcome> outcomes)
                        {
                            ObservableCollection<CovariantObservableGrouping<Student, ObservableCollection<IMark>>> _gridRows =
                                new ObservableCollection<CovariantObservableGrouping<Student, ObservableCollection<IMark>>>
                                (
                                    from Student s in this.StudentGroup.Students
                                    orderby s.TCSStudentRow
                                    select new CovariantObservableGrouping<Student, ObservableCollection<IMark>>
                                    (
                                        s,
                                        outcomes.Select(d => new ObservableCollection<IMark>())
                                    )
                                );

                            //add attempts
                            foreach (Mark m in this.Marks)
                            {
                                _gridRows[m.TCSStudentRow][m.TCSOutcomeColumn].Add(m);
                            }
                            return _gridRows;
                        },
                        "outcomes",
                        null
                        );
                return _marksGrid;
            }
        }
        
        public bool myEquals(CourseStudentGroup other)
        {
            throw new NotImplementedException();
            //return Course.myEquals(other.Course) && StudentGroup.myEquals(other.StudentGroup);
        }


        ICovariantObservableCollection<IDeadline> ITeachingGroup.Deadlines
        {
            get { return Deadlines; }
        }

        ICovariantObservableCollection<IAttempt> ITeachingGroup.Attempts
        {
            get { return Attempts; }
        }

        ICovariantObservableCollection<IMark> ITeachingGroup.Marks
        {
            get { return Marks; }
        }

        ICourse ITeachingGroup.Course
        {
            get { return Course; }
        }

        IStudentGroup ITeachingGroup.StudentGroup
        {
            get { return StudentGroup; }
        }

        public bool myEquals(ITeachingGroup other)
        {
            return Course.Hash == other.Course.Hash && StudentGroup.Hash == other.StudentGroup.Hash;
        }
    }
}
