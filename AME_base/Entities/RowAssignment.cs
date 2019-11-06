
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    public class RowAssignment 
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte AssignmentNum { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        public byte RowNum { get; set; }

        //[Required]
        //public virtual ICollection<Mark> Marks { get; set; }


        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
        [ForeignKey("GroupID, AssignmentNum")]
        public virtual Assignment Assignment { get; set; }

        public long RowHash { get; set; }
        [ForeignKey("RowHash")]
        public virtual Row Row { get; set; }

        //NEEDED FOR EF
        public RowAssignment()
        {
            //Marks = new Collection<Mark>();
        }

        //dont pass in higher data structures
        public RowAssignment(byte paramRowNum,
            byte paramGroupId,
            byte paramAssignmentNum, 
            long paramRowHash)
        {
            RowHash = paramRowHash;
            RowNum = paramRowNum;
            GroupID = paramGroupId;
            AssignmentNum = paramAssignmentNum;

            //Marks = new Collection<Mark>();
        }
        

    }
}
