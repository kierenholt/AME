using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AME_base
{
    public class Response
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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 3)]
        public byte AttemptNum { get; set; }




        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
        [ForeignKey("GroupID, AssignmentNum")]
        public virtual Assignment Assignment { get; set; } 
        [ForeignKey("GroupID, AssignmentNum, QuestionNum")]
        public virtual QuestionAssignment QuestionAssignment { get; set; }
        [ForeignKey("GroupID, AssignmentNum, AttemptNum")]
        public virtual Attempt Attempt { get; set; }

        [Required]
        public byte Score { get; set; }
        [Required]
        public byte Retries { get; set; } 

        //NEEDED FOR EF
        public Response()
        {
        }

        public Response init(IEnumerable<Field> paramFields, 
            Attempt paramAttempt,
            QuestionAssignment paramRowAssignment)
        {
            Score = 0;
            Retries = 0;
            subResponses = new StringableCollection<SubResponse>(paramFields.Select(f => new SubResponse(f.Value, f.subQuestionIndex)));

            Group = paramAttempt.Group;
            GroupID = paramAttempt.GroupID;
            Assignment = paramRowAssignment.Assignment;
            AssignmentNum = paramRowAssignment.AssignmentNum;
            Attempt = paramAttempt;
            QuestionAssignment = paramRowAssignment;
            QuestionNum = paramRowAssignment.QuestionNum;

            return this;
        }


        //dont pass in higher data structures
        //used by SEED method in schoolcontext
        public Response(IEnumerable<SubResponse> paramSubResponses)
        {
            subResponses = new StringableCollection<SubResponse>(paramSubResponses); //so the subResponsesstring is also set
            Score = 0;
            Retries = 0;
        }

        public bool update(IEnumerable<Field> paramFields) //returns number of retries
        {
            //update if subresponses have changed
            bool changed = false;
            foreach (Field f in paramFields)
            {
                SubResponse found = subResponses.SingleOrDefault(s => s.index == f.subQuestionIndex);
                if (found == null)
                {
                    subResponses.Add(new SubResponse(f.Value, f.subQuestionIndex));
                    changed = true;
                }
                else
                {
                    if (f.Value != found.Text)
                    {
                        found.Text = f.Value;
                        changed = true;
                    }
                }
            }

            if (changed)
                Retries++;
            return changed;
        }

        public void updateScore()
        {
            Score = (byte)subResponses.Sum(sr => sr.Score);
        }

        #region subresponses
        
        public string _SubResponsesAsString { get; set; } //not mapped by EF unless it is public

        //serialise this because _SubResponsesAsString is not serialised
        [NonSerialized]
        private StringableCollection<SubResponse> _subResponses;
        [NotMapped]
        public StringableCollection<SubResponse> subResponses
        {
            get
            {
                if (_subResponses == null)
                {
                    _subResponses = new StringableCollection<SubResponse>(_SubResponsesAsString);
                    //do something if the responseElement properties are changed
                    _subResponses.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
                    {
                        _SubResponsesAsString = _subResponses.toString();
                    };
                }
                return _subResponses;
            }
            set
            {
                _subResponses = value;
                _subResponses.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
                {
                    _SubResponsesAsString = _subResponses.toString();
                };
                _SubResponsesAsString = value.toString();
            }
        }
        #endregion


    }
}
