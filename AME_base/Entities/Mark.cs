using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
 

using System.Runtime.Serialization;

namespace AME_base
{
    public class Mark
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte AssignmentNum { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        public byte RowAssignmentNum { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 3)]
        public byte AttemptNum { get; set; }
        

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
        [ForeignKey("GroupID, AssignmentNum")]
        public virtual Assignment Assignment { get; set; }
        [ForeignKey("GroupID, AssignmentNum, RowAssignmentNum")]
        public virtual RowAssignment RowAssignment { get; set; }
        [ForeignKey("GroupID, AssignmentNum, AttemptNum")]
        public virtual Attempt Attempt { get; set; }

        [Required]
        public byte CompletionZeroBased { get; set; }

        //NEEDED FOR EF
        public Mark()
        {
        }


        public Mark(byte paramCompletionZeroBased,
            byte paramGroupId, 
            byte paramAssignmentNum, 
            byte paramRowNum)
        {
            CompletionZeroBased = paramCompletionZeroBased;
            GroupID = paramGroupId;
            AssignmentNum = paramAssignmentNum;
            RowAssignmentNum = paramRowNum;
        }
        
        private static readonly decimal COMPLETION_THRESHOLD_1 = 0.2m; //% marks needed for completion level 1 //belongs to outcome NOT mark
        private static readonly decimal COMPLETION_THRESHOLD_2 = 0.8m; //% marks needed for completion level 2
        
    }
}
