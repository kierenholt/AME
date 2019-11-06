
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AME_base
{
    //PURPOSES ENUMERATION
    public enum Purpose
    {
        simple = 0,
        template = 1,
        //hidden = 2,
        nonQuestion = 2,
        //condition = 2 NO LONGER USED
    }

    public class Question : Row, ISerializableRow 
    {
        //Hash stored in Row
        [Required]
        public int OutOf { get; set; }
        [Required]
        public Purpose Purpose { get; set; }

        [NotMapped]
        public IEnumerable<QuestionAssignment> QuestionAssignments 
        {
            get { return RowAssignments.Cast<QuestionAssignment>(); } 
        }
        
        //NEEDED FOR EF
        public Question() : base()
        {
        }


        //dont pass in higher data structures
        public Question init(Purpose paramPurpose, 
            string paramTitle,
            string paramComment,
            string paramLeft,
            string paramRight,
            long paramHash)
        {
            base.init(paramTitle,
             paramComment,
             paramLeft,
             paramRight,
             paramHash);

            //left and right already done
            Purpose = paramPurpose;
            QuestionAssignment dummy = new QuestionAssignment(0, 0, 0, 0);
            dummy.Question = this;
            dummy.Row = this;
            OutOf = new QuestionAssignmentHTML(dummy,false,0).outOf;

            return this;
        }


        SerializableRow ISerializableRow.toSerializableRow()
        {
            return new SerializableRow(Title, Comment, Left, Right, Purpose);
        }

        //protected void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //    info.AddValue("Purpose", Purpose);
        //}

        //protected Question(SerializationInfo info, StreamingContext context) : 
        //    base(info, context)
        //{
        //    Purpose = (Purpose)info.GetValue("Purpose", typeof(Purpose));
        //}


    }
}
