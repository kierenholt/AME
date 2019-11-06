using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
 

using System.Runtime.Serialization;

namespace AME_base
{
    public class Group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }

        [Required]
        public byte AssignmentCount { get; set; } //increments AssignmentNum
        [Required]
        public byte SeatCount { get; set; } 

        [Required]
        public string Name { get; set; }


        [Required]
        public virtual ICollection<Seat> Seats { get; set; }
        [Required]
        public virtual ICollection<Assignment> Assignments { get; set; }
        [Required]
        public virtual ICollection<Attempt> Attempts { get; set; }
        //[Required]
        //public ICollection<Mark> Marks { get; set; }
        [Required]
        public virtual ICollection<Response> Responses { get; set; }
        [Required]
        public virtual ICollection<QuestionAssignment> QuestionAssignments { get; set; }
        [Required]
        public virtual ICollection<RowAssignment> RowAssignments { get; set; }

        //NEEDED FOR EF
        public Group()
        {
            Assignments = new List<Assignment>();
            Attempts = new List<Attempt>();
            Seats = new List<Seat>();
            Responses = new List<Response>();
            QuestionAssignments = new List<QuestionAssignment>();
            RowAssignments = new List<RowAssignment>();
        }
        
        public void init(IEnumerable<Seat> paramISeats, 
            string paramName, byte paramGroupId)
        {
            Name = paramName;
            GroupID = paramGroupId;
            SeatCount = paramISeats == null ? (byte)0 : (byte)paramISeats.Count();
            Seats = paramISeats == null ? new Collection<Seat>() : new Collection<Seat>(paramISeats.ToList());
            AssignmentCount = 0;
        }

        #region Thresholds

        //[Required]
        [NotMapped]
        public   string ThresholdsAsString { get; set; }
        //[Required]
        [NotMapped]
        public   bool ThresholdsAreAbsolute { get; set; }
        [NonSerialized]
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
        public   int visibleGrids { get; set; }
        public bool isGridVisible(int index)
        {
            return (visibleGrids & (int)Math.Pow(2, index)) == Math.Pow(2, index);
        }
        public void setGridVisibility(int index, bool isVisible)
        {
            visibleGrids = isVisible ?
                visibleGrids | (int)Math.Pow(2, index) :
                visibleGrids & ~(int)Math.Pow(2, index);
        }
        #endregion

         
    }
}
