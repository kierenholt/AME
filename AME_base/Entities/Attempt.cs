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

    public class Attempt
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None )]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte AssignmentNum { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        public byte AttemptNum { get; set; }


        public byte SeatNum { get; set; }
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column(Order = 3)]
        public byte ShuffleSeed { get; set; }

        [Required]
        public DateTime whenAssigned { get; set; }
        //[Required]
        public DateTime? whenReturned { get; set; }

        public int? daysLate
        {
            get
            {
                if (whenReturned == null)
                    return null;// DateTime.Today.Subtract(whenAssigned).Days;
                return whenReturned.Value.Subtract(whenAssigned).Days;
            }
        }

        [Required]
        public byte Score { get; set; }
        [Required]
        public byte Retries { get; set; }

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; } 
        [ForeignKey("GroupID, AssignmentNum")]
        public virtual Assignment Assignment { get; set; } 
        [ForeignKey("GroupID, SeatNum")]
        public virtual Seat Seat { get; set; }

        [Required]
        public virtual ICollection<Mark> Marks { get; set; }
        [Required]
        public virtual ICollection<Response> Responses { get; set; }

        //NEEDED FOR EF
        public Attempt()
        {
            Marks = new Collection<Mark>();
            Responses = new Collection<Response>();
        }


        public Attempt(
            byte paramShuffleSeed, 
            byte paramSeatNum,
            byte paramAssignmentNum,
            byte paramGroupID,
            DateTime paramWhenAssigned,
            byte paramAttemptNum)
        {
            init(
             paramShuffleSeed,
             paramSeatNum,
             paramAssignmentNum,
             paramGroupID,
             paramWhenAssigned,
             paramAttemptNum);
        }

        public void init(
            byte paramShuffleSeed, 
            byte paramSeatNum,
            byte paramAssignmentNum,
            byte paramGroupID,
            DateTime paramWhenAssigned,
            byte paramAttemptNum)
        {
            //OutOf = paramAssignmentOutOf;
            AttemptNum = paramAttemptNum;
            SeatNum = paramSeatNum;
            AssignmentNum = paramAssignmentNum;
            GroupID = paramGroupID;
            whenAssigned = paramWhenAssigned;
            ShuffleSeed = paramShuffleSeed;
            Score = 0;
            Retries = 0;
            Marks = new Collection<Mark>();
            Responses = new Collection<Response>();
        }


        public IEnumerable<Response> UpdateResponseValuesFromPDF(IEnumerable<Field> paramFields, SchoolContext paramDb)
        {
            Collection<Response> retVal = new Collection<Response>(); 
            foreach (var groupedByRowNum in paramFields.GroupBy(f => f.QuestionNum))
            {
                //check if response exists
                Response found = Responses.SingleOrDefault(r => (int)r.QuestionNum == groupedByRowNum.Key);

                if (found == null) //create new response
                {
                    Response response = paramDb.Responses.Create().init(groupedByRowNum, this, Assignment.QuestionAssignments.Single(r => r.QuestionNum == groupedByRowNum.Key));
                    Responses.Add(response);
                    retVal.Add(response);
                }
                else //updateresponse
                {
                    if (found.update(groupedByRowNum))
                        retVal.Add(found);
                }
            }

            return retVal;
        }

        public void updateScore()
        {
            foreach (Response r in Responses)
                r.updateScore();
            Score = (byte)Responses.Sum(r => (int)r.Score);
            Retries = (byte)Responses.Sum(r => (int)r.Retries);
        }



    }
}
