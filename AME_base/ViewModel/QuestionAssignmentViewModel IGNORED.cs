using AME_base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    
    /// <summary>
    /// this stuff is all in viewmodel because HTML relies on the selectedResponse and HTML will change just like a viewmodel
    /// </summary>
    public class QuestionAssignmentViewModel INotifyPropertyChanged
    {
        private QuestionAssignmentHTML _questionAssignmentHTML;
        
        public QuestionAssignmentViewModel(
            QuestionAssignmentHTML parmaQA
            )
            : base(parmaQA)
        {
            _questionAssignmentHTML = parmaQA;
        }

        private int _shuffledQuestionNumber;
        public int shuffledQuestionNumber
        {
            get { return _questionAssignmentHTML.questionNumber; }
        }
        
        //private ResponseViewModel _selectedResponse;
        //public ResponseViewModel selectedResponse
        //{
        //    get { return _selectedResponse; }
        //    set 
        //    { 
        //        _selectedResponse = value;
        //        RaisePropertyChanged("previewHTML");
        //    }
        //}

        //private OutcomeViewModel _outcome;
        //public OutcomeViewModel outcome
        //{
        //    get
        //    {
        //        return _outcome;
        //    }
        //    set
        //    {
        //        _outcome = value; //set to distinct outcomes of parent assignment
        //        //question scoreall changed triggers outcome scoreall change
        //        PropertyChanged += (sender, e) =>
        //        {
        //            if (e.PropertyName == "scoreAll")
        //            {
        //                _outcome.RaisePropertyChanged("scoreAll");
        //                _outcome.RaisePropertyChanged("completionIndex");
        //                //_skipToNextUndecidedMethod(null);
        //            }
        //        };
        //    }
        //}



        //private ObservableCollection<ResponseViewModel> _responses;
        //public ObservableCollection<ResponseViewModel> responses
        //{
        //    get
        //    {
        //        if (_responses == null)
        //            _responses = new ObservableCollection<ResponseViewModel>(_questionAssignment.Responses.Select(r => new ResponseViewModel(r)));
        //        return _responses;
        //    }
        //}


        #region ResponseElements
        //these are the correct responses and total scores 
        //[Required]
        //public string CorrectResponseElementsAsString { get; set; }
        //private CorrectResponseElementCollection _correctResponseElements;
        //[NotMapped]
        //public CorrectResponseElementCollection CorrectResponseElements
        //{
        //    get
        //    {
        //        if (_correctResponseElements == null)
        //        {
        //            _correctResponseElements = new CorrectResponseElementCollection(CorrectResponseElementsAsString, RowNumber, Responses);
        //            //if the responseElements are changed update the string
        //            _correctResponseElements.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        //            {
        //                CorrectResponseElementsAsString = _correctResponseElements.toString();
        //            };
        //        }
        //        return _correctResponseElements;
        //    }
        //    set
        //    {
        //        CorrectResponseElementsAsString = value.toString();
        //    }
        //}

        #endregion ResponseElements

        //#region ResponseSummaries

        //private ResponseElementSummaries _responseSummaries;
        //[NotMapped]
        //public ResponseElementSummaries responseSummaries
        //{
        //    get
        //    {
        //        return _responseSummaries ?? (_responseSummaries = new ResponseElementSummaries(Responses));
        //    }
        //}

        //#endregion ResponseSummaries



        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
