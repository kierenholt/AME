
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AME_base
{
    public class Assignment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte AssignmentNum { get; set; }

        [Required]
        public byte AttemptCount { get; set; }

        [Required]
        public string Name { get; set; } //from email subject
        //[Required]
        //public bool ShuffleEnabled { get; set; }
        [Required]
        public int OutOf { get; set; } //SAME AS ATTEMPT OUTOF

        [Required]
        public virtual ICollection<Attempt> Attempts { get; set; }
        [Required]
        public virtual ICollection<RowAssignment> RowAssignments { get; set; }
        [Required]
        public virtual ICollection<QuestionAssignment> QuestionAssignments { get; set; }
        
        [Required]
        public virtual ICollection<Response> Responses { get; set; }
        //[Required]
        //public ICollection<Mark> Marks { get; set; }

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        //for EF
        public Assignment()
        {
            //Name = paramName;
            //OutOf = paramOutOf;

            Responses = new Collection<Response>();
            RowAssignments = new Collection<RowAssignment>();
            QuestionAssignments = new Collection<QuestionAssignment>();
            Attempts = new Collection<Attempt>();
            Responses = new Collection<Response>();

        }


        public Assignment init(IEnumerable<Row> paramRepeatedRows,
            Group paramGroup, 
            byte paramAssignmentNum,
            string paramNameOptional,
            bool paramShuffleEnabled,
            int paramMarkLimit
            )
        {
            Group = paramGroup;
            GroupID = paramGroup.GroupID;
            AssignmentNum = paramAssignmentNum;


            Row withTitle = paramRepeatedRows.Where(r => !string.IsNullOrEmpty(r.Title))
                .FirstOrDefault();
            Name = (string.IsNullOrEmpty(paramNameOptional)) ? 
                (withTitle != null ? withTitle.Title : "untitled") : 
                paramNameOptional;

            AttemptCount = 0;


            //ShuffleEnabled = paramShuffleEnabled;
            
            List<Row> rowsAddedSoFar = new List<Row>();

            byte RowCount = 0;
            byte QuestionCount = 0;


            //question
            foreach (Row newRow in paramRepeatedRows)
            {
                Row rowFound = rowsAddedSoFar.FirstOrDefault(er => er.Hash == newRow.Hash) ?? newRow;

                //QUESTIONNM COUNTS NUMBER OF QUESTIONS ONLY
                //ROWNUM COUNTS ROWS AND QUESTIONS

                if (rowFound is Question)
                {
                    QuestionAssignment newQA = new QuestionAssignment(++QuestionCount, GroupID, AssignmentNum, rowFound.Hash);
                    ++RowCount;
                    newQA.Row = rowFound;

                    newQA.Group = Group;
                    newQA.GroupID = GroupID;

                    newQA.Assignment = this;
                    newQA.AssignmentNum = AssignmentNum;

                    newQA.Question = (rowFound as Question);
                    
                    QuestionAssignments.Add(newQA);
                }
                else
                {
                    RowAssignment newRA = new RowAssignment(++RowCount, GroupID, AssignmentNum, rowFound.Hash);
                    newRA.Row = rowFound;

                    newRA.Group = Group;
                    newRA.GroupID = GroupID;

                    newRA.Assignment = this;
                    newRA.AssignmentNum = AssignmentNum;

                    newRA.Assignment = this;
                    newRA.AssignmentNum = AssignmentNum;

                    RowAssignments.Add(newRA);
                }
                if (!rowsAddedSoFar.Contains(rowFound))
                    rowsAddedSoFar.Add(rowFound);

            }


            OutOf = paramMarkLimit;
            if (OutOf == 0)
                OutOf = QuestionAssignments.Sum(qa => qa.Question.OutOf);


            return this;
            //outcome
            //Outcome newOutcome = null;
            //if (!string.IsNullOrEmpty(r.Cells[1, paramOutcomeCol].Value2))
            //{
            //    newOutcome = new Outcome(r.Cells[1, paramOutcomeCol].Value2);

            //    if (!_Outcomes.Any(er => er.Hash == newOutcome.Hash))
            //        _Outcomes.Add(newOutcome);
            //    else
            //        newOutcome = _Outcomes.Single(er => er.Hash == newOutcome.Hash);
            //}


            ////add outcome to row
            //if (newOutcome != null)
            //    newOutcome.Rows.Add(newRow);
            //newRow.Outcome = newOutcome;
        }
        
        //public List<byte> getRowNumsOfShuffledQuestions(int shuffledSeed) //needs to match required number of marks
        //{
        //    int assignmentQuestionCount = QuestionAssignments.Count();
        //    IEnumerable<byte> rowNums = QuestionAssignments.Select(qa => qa.QuestionNum) ;

        //    Random _random = new Random(shuffledSeed);

        //    //shuffle
        //    if (ShuffleEnabled)
        //        rowNums = rowNums.Shuffle(_random);
            
        //    //take only so you can reach marklimit
        //    if (OutOf < QuestionAssignments.Sum(qa => qa.Question.OutOf))
        //    {
        //        List<int> marks = QuestionAssignments.Select(qa => qa.Question.OutOf).ToList();

        //        IEnumerable<string> possibleCombinations = Helpers.getIndices(marks, 0, OutOf);
        //        if (!possibleCombinations.Any())
        //            throw new System.Exception("no possible combinations that add to " + OutOf);
        //        IEnumerable<int> indices = possibleCombinations.First().Split(new[] {','},StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str));

        //        return indices.Select((i) => rowNums.ElementAt(i)).ToList();
        //    }
        //    else
        //    {
        //        return rowNums.ToList();
        //    }

        //}
     
        



    }
}
