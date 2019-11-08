
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Windows.Input;
using System.Diagnostics;
using AME_base;
using Microsoft.Office.Interop.Outlook;



namespace AME_base
{
    
    public delegate void ResponsesMarkedEventHandler(object sender, ResponsesMarkedEventArgs e);
    public class ResponsesMarkedEventArgs : EventArgs
    {
        public IEnumerable<Response> responses;

        public ResponsesMarkedEventArgs(IEnumerable<Response> paramResponses)
        {
            responses = paramResponses;
        }
    }

    public class AssignmentViewModel : INotifyPropertyChanged
    {
        public Assignment _entity { get; private set; }
        public AssignmentViewModel(Assignment a) 
        {
            _entity = a;
        }


        //private ObservableCollection<QuestionAssignmentViewModel> _questions;
        //public ObservableCollection<QuestionAssignmentViewModel> questions
        //{
        //    get
        //    {
        //        if (_questions == null)
        //        {
        //            _questions = new ObservableCollection<QuestionAssignmentViewModel>(
        //                _entity.QuestionAssignments.Select((q,s) => new QuestionAssignmentViewModel(new QuestionAssignmentHTML(q, s, false, 0))));
        //        }
        //        return _questions;
        //    }
        //}

        public int outOf { get { return _entity.OutOf; } }



        //public static readonly string[] lateness = { "early", "late" };
        //public static readonly string[] latenessImages = { "View/icons/bell.png", "View/icons/hourglass.png" };
        //public static List<LegendItem> legend
        //{
        //    get
        //    {
        //        List<LegendItem> retVal = new List<LegendItem>();
        //        for (int i = 0; i < DeadlineViewModel.lateness.Count(); i++)
        //            retVal.Add(new LegendItem() { descriptor = DeadlineViewModel.lateness[i], icon = DeadlineViewModel.latenessImages[i] });
        //        return retVal;
        //    }
        //}



        //protected ObservableCollection<AttemptViewModel> _attempts;
        //public ObservableCollection<AttemptViewModel> attempts
        //{
        //    get
        //    {
        //        if (_attempts == null)
        //            _attempts = new ObservableCollection<AttemptViewModel>(_entity.Attempts.Select(a => new AttemptViewModel(a)));
        //        return _attempts;
        //    }
        //}

        
        

        public string name
        {
            get { return _entity.Name; }
            set { }
        }

        public int selectedGridIndex { get; set; }


        #region grids

        //needed when selecting charts
        //public List<IGridData<object,Seat,object>> gridDatas { get { return new List<IGridData<object,Seat,object>>() { responsesGrid, responseElementsGrid, marksGrid }; } }

        //adds attempt to live griddata
        //public void addLiveAttempt(Attempt attempt)
        //{
        //    if (responsesGrid.initialized)
        //    {
        //        //update each column in the Seats' specific row
        //        ObservableGrouping<Seat, ObservableCollection<Response>> SeatRow = responsesGrid.rows.SingleOrDefault(r => r.Key.Seat.Email == attempt.Seat.Seat.Email);
        //        foreach (Response newResponse in attempt.Responses)
        //        {
        //            int columnIndex = responsesGrid.columns.IndexOf(newResponse.QuestionAssignment);
        //            SeatRow[columnIndex].Add(newResponse);
        //        }
        //    }
        //if (responseElementsGrid.initialized)
        //{
        //    ObservableGrouping<Seat, ObservableCollection<ResponseElement>> SeatRow = responseElementsGrid.rows.SingleOrDefault(r => r.Key.Seat.Email == attempt.Seat.Seat.Email);
        //    foreach (Response newResponse in attempt.Responses)
        //        foreach (ResponseElement newResponseElement in newResponse.responseElements)
        //        {
        //            int responseElementIndex = newResponseElement.Index;
        //            int columnIndex = responseElementsGrid.columns.IndexOf(newResponse.QuestionAssignment.CorrectResponseElements [responseElementIndex]);
        //            SeatRow[columnIndex].Add(newResponseElement);
        //        }
        //}
        //}

        public void updateGrids(IEnumerable<Response> responses)
        {
            if (_grids != null && _grids[0] != null)
                foreach (var r in responses)
                    _grids[0].updateCell(r);
        }

        private ObservableCollection<GridViewModelBase<QuestionAssignment, Attempt, Response>> _grids;
        public ObservableCollection<GridViewModelBase<QuestionAssignment, Attempt, Response>> grids
        {
            get
            {
                if (_grids == null)
                {
                    _grids = new ObservableCollection<GridViewModelBase<QuestionAssignment, Attempt, Response>>() { };

                    //RESPONSES AND OUTCOMES
                    _grids.Add(new GridViewModelBase<QuestionAssignment, Attempt, Response>(
                        ResponseViewModel.legend,
                        "responses",
                        "student",
                        () => { return _entity.QuestionAssignments.OrderBy(q => q.QuestionNum); },
                        () => { return _entity.Attempts.OrderBy(s => s.AttemptNum); },
                        () => { return _entity.Responses; },
                        (Response r) => { return new[] { (int)r.QuestionNum - 1, (int)r.AttemptNum - 1 }; }, //seatnum starts with 1, row starts with 1
                        new List<ColumnSettings<QuestionAssignment, Response>>()
                        {
                            new ColumnSettings<QuestionAssignment, Response>()
                            {
                                colHeaderValue = p => "Q" + p.QuestionNum.ToString(),
                                isVisible = true,
                                listboxDescriptionName = "Score",
                                colorOperator = p => System.Windows.Media.Brushes.Black,
                                valueOperator = (Response r) =>  { return r.Score; },
                                isImage = false,
                                columnViewModelConstructor = p => { return p == null ? null : 
                                    new HTMLViewModel(new QuestionAssignmentHTML(p, false, 0)); },
                                cellViewModelConstructor = c => c == null ? null : new ResponseViewModel(new ResponseHTML(false,c))
                            }
                        },
                        new RowSettings<Attempt, Response>()
                        {
                            rowHeaderOperator = p => p.Seat.LastName + " " + p.Seat.FirstName ,
                            ifRowFilterEnabledVisibleWhen = delegate(IGridRowViewModel rvm)
                            {
                                return !rvm.readOnlyCells.All(c => c.value == string.Empty);
                            },
                            rowViewModelConstructor = s => s == null ? null : new AttemptViewModel(new AttemptHTML(s))
                        }
                        ));


                    //RESPONSEELEMENTS
                    //_grids.Add(new GridViewModelBase<CorrectResponseElement, Seat, ResponseElement>(_entity.responseElementsGrid)
                    //{
                    //    legend = null,
                    //    cornerText = "Seat name",
                    //    columnSettings = new List<ColumnSettings<CorrectResponseElement, ResponseElement>>()
                    //    {
                    //        new ColumnSettings<CorrectResponseElement, ResponseElement>()
                    //        {
                    //            colHeaderValue = p => "Q" + p.columnHeader,
                    //            isVisible = true,
                    //            listboxDescriptionName = "best score",
                    //            colorOperator = p => "Black",
                    //            cellCurrentItemOperator = p => p.Any() ? 
                    //                new SubResponseViewModel(p.OrderBy(r => r.Score).Last()) : 
                    //                null,
                    //            valueOperator  = delegate(IEnumerable<ResponseElement> entities) 
                    //            {
                    //                int? score = null;
                    //                if (entities != null && entities.Any())
                    //                    score = entities.Max(r => r.Score);
                    //                return score;
                    //            },
                    //            isImage = false,
                    //            columnCurrentItemOperator = p => p == null ? null : new CorrectResponseElementViewModel(p)
                    //        }
                    //    },
                    //    rowSettings = new RowSettings<Seat, ResponseElement>()
                    //    {
                    //        rowHeaderOperator = p => p.Seat.LastName + " " + p.Seat.FirstName,
                    //        ifRowFilterEnabledVisibleWhen = delegate(GridRowViewModel rvm)
                    //        {
                    //            return !rvm.readOnlyCells.All(c => c.value == string.Empty);
                    //        },
                    //        currentItemOperator = s => s == null ? null : new SeatViewModel(s)
                    //    }
                    //});

                }
                return _grids;
            }
        }
        #endregion



        //protected ICommand _emailResultsCommand;
        //public ICommand emailResults
        //{
        //    get
        //    {
        //        if (_emailResultsCommand == null)
        //            this._emailResultsCommand = new DelegateCommand(this._emailResultsMethod);
        //        return this._emailResultsCommand;
        //    }
        //}
//        protected virtual void _emailResultsMethod(object o) //NULL PARAMETER
//        {
//            //gets currently selected gridrows into chart
//            IChart<Seat> ResultsChart = grids[selectedGridIndex].chart;

//            foreach (Seat s in grids[selectedGridIndex].selectedRowEntities)
//            {
//                MailItem mailItem = (MailItem)Globals.ThisAddIn.Application.CreateItem(OlItemType.olMailItem);
//                mailItem.Subject = string.Format(@"{0} results", _entity.Assignment.Name);
//                mailItem.To = s.Seat.Email;
//                mailItem.Body = string.Format(@"
//This is an automated email. A chart showing your responses from {0} is included as an attachment.
//", _entity.Assignment.Name);
//                mailItem.Attachments.Add(ResultsChart.saveSeatProgressChartReturnsTempFileName(s));
//                mailItem.Send();
//            }
//            MessageBox.Show(string.Format("{0} emails have been sent to the Seats in this class", grids[selectedGridIndex].selectedRowEntities.Count()));
//        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
