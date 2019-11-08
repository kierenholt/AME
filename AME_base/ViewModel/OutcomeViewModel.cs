using AME_base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AME_base
{
    public class OutcomeViewModel : INotifyPropertyChanged 
    {

        public string description
        {
            get
            {
                return _entity.Description;
            }
            //set
            //{
            //    _entity.Description = value; readonly
            //}
        }

        private Outcome _entity;
        public OutcomeViewModel(Outcome o) 
        {
            _entity = o;
        }

        //outcome -> rows -> rowassignments -> marks

        //private ObservableCollection<MarkViewModel, Mark> _marks;
        //public ObservableCollection<MarkViewModel, Mark> marks
        //{
        //    get
        //    {
        //        if (_marks == null)
        //            _marks = new ObservableCollection<MarkViewModel,Mark>(entity.Marks);
        //        return _marks;
        //    }
        //}


        //public int scoreAll { get { return _entity.s.scoreAll; } }
        //public int outOfAll { get { return _entity.outOfAll; } }

        //if a related question is selected, the mark can be changed
        private bool _isEnabled;
        public bool isEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("isEnabled");
            } //set from assignment when selectedquestion is changed
        }

        //private ObservableCollection<AssignmentViewModel,Assignment> _assignments;
        //public ObservableCollection<AssignmentViewModel,Assignment> assignments
        //{
        //    get
        //    {
        //        if (_assignments == null)
        //            _assignments = new ObservableCollection<AssignmentViewModel,Assignment>(entity.Assignments);
        //        return _assignments;
        //    }
        //}

        //each row
        public string descriptionHTML
        {
            get
            {
                return String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                    _entity.Description,
                    radio(0),
                    radio(1),
                    radio(2));
            }
        }


        //radio buttons for each column
        private string radio(int col)
        {
            return string.Format(@"<input type=""radio"" name=""{1}{2}0"" value=""{0}"">",
                col,
                _entity.Hash,
                ":"); //NO ID - USE ZERO INSTEAD
        }


        //private IEnumerable<PDFMark> _marks = new List<PDFMark>();
        //public IEnumerable<PDFMark> marks 
        //{
        //    get
        //    {
        //        if (_marks == null)
        //            _marks = new List<PDFMark>();
        //        if (!_marks.Any() || _marks.Last().hasBeenAnnotated)
        //            _marks = _marks.Concat(new [] {new PDFMark(outOfAll, scoreAll)});
        //        return _marks;
        //    }
        //}

        //public int outOfAll { get { return questions.Sum(q => q.OutOfAll); } }
        //public int scoreAll { get { return questions.Sum(q => q.ScoreAll); } }



        //public void addAnnotation()
        //{
        //    foreach (PDFMark m in marks)
        //        m.hasBeenAnnotated = true;
        //    if (field != null)
        //    {
        //        field.addAnnotation(index.ToString(),
        //            CommentBase.Decision.decided, 1, 1, //a bit like 'correct' - so it is a tick
        //            feedback,
        //            !string.IsNullOrEmpty(feedback),
        //            completionIndex, //radio button corresponding to level index
        //            10,
        //            5);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
