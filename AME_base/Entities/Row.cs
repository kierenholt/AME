
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace AME_base
{
    [Serializable]
    public class SerializableRow
    {
        public string title { get; set; }
        public string comment { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public Purpose purpose { get; set; }
        public long hash { get; set; }

        public SerializableRow(string paramTitle,
            string paramComment,
            string paramLeft,
            string paramRight,
            Purpose paramPurpose) 
        {
            title = paramTitle;
            comment = paramComment;
            left = paramLeft;
            right = paramRight;
            purpose = paramPurpose;
            hash = AME_base.Helpers.Hash(new[] { left, right, comment });
        }

        public Row toRowOrQuestion()
        {
            if (purpose == Purpose.nonQuestion)
                return new Row().init(title, comment, left, right, hash) ;
            return new Question().init(purpose, title, comment, left, right, hash );
        }
    }

    public interface ISerializableRow //used for new authored work 
    {
        SerializableRow toSerializableRow();
    }


    public class Row : ISerializableRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Hash { get; set; }

        //[Required] could be blank
        public string Title { get; set; }
        //[Required] could be blank
        public string Comment { get; set; }

        //[Required] could be blank
        public string Left { get; set; }
        //[Required]  could be blank
        public string Right { get; set; }



        [Required]
        public virtual ICollection<RowAssignment> RowAssignments { get; set; }
        //[Required]
        //public virtual Outcome Outcome { get; set; }

        //NEEDED FOR EF
        public Row()
        {
            RowAssignments = new Collection<RowAssignment>();
        }


        public Row init(string paramTitle,
            string paramComment,
            string paramLeft,
            string paramRight,
            long paramHash) 
        {
            Title = paramTitle;
            Comment = paramComment;
            Left = paramLeft;
            Right = paramRight;
            Hash = paramHash;

            RowAssignments = new Collection<RowAssignment>();

            return this;
        }
        
        [NotMapped]
        public string[] leftRight
        {
            get
            {
                return new[] { Left, Right };

            }
            set
            {
                Left = value[0];
                Right = value[1];
            }
        }

        SerializableRow ISerializableRow.toSerializableRow()
        {
            return new SerializableRow(Title, Comment, Left, Right, Purpose.nonQuestion);
        }

    }
}
