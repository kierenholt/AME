
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    public class QuestionAssignment // : RowAssignment DO NOT INHERIT DUE TO  USE OF QUESTIONNUM
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte AssignmentNum { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        public byte QuestionNum { get; set; }

        //[Required]
        //public virtual ICollection<Mark> Marks { get; set; }


        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
        [ForeignKey("GroupID, AssignmentNum")]
        public virtual Assignment Assignment { get; set; }

        public long RowHash { get; set; }
        [ForeignKey("RowHash")]
        public virtual Row Row { get; set; }
        [Required]
        public virtual ICollection<Response> Responses { get; set; }


        //[Required] not required since it could be set to null when a duplicate row is detached (ignored) from the context
        [NotMapped]
        public Question Question 
        { 
            get { return (Question)Row; }
            set { Row = value; }
        }

        //NEEDED FOR EF
        public QuestionAssignment() : base()
        {
            Responses = new Collection<Response>();
        }

        //dont pass in higher data structures
        public QuestionAssignment(byte paramQuestionNum,
            byte paramGroupId,
            byte paramAssignmentNum,
            long paramRowHash)
        {
            RowHash = paramRowHash;
            QuestionNum = paramQuestionNum;
            GroupID = paramGroupId;
            AssignmentNum = paramAssignmentNum;

            Responses = new Collection<Response>();
        }


    }
}
