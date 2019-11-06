using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AME_base
{
    [Serializable]
    public class AssignedWork// :avoid inherit   AuthoredWork 
    {
        public byte AssignmentNum { get; private set; }
        public byte GroupId { get; private set; }
        public byte ShuffleSeed { get; private set; }
        public string teacherEmailAddress { get; private set; }
        
        //seatnum cannot be included since each assignedwork is generated for up to six students
        //this assignedwork could match up to six different attempts
        public AssignedWork (
            byte paramGroupID,
            byte paramAssignmentNum,
            byte paramShuffleSeed,
            string paramTeacherEmailAddress)
        {
            GroupId = paramGroupID;
            AssignmentNum = paramAssignmentNum;
            ShuffleSeed = paramShuffleSeed;
            teacherEmailAddress = paramTeacherEmailAddress;
        }

    }
}
