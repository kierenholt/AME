using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Office.Interop.Outlook;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using AME_base;

namespace AME_base
{
    public class GroupViewModel : INotifyPropertyChanged 
    {
        public Group _entity { get; private set; }

        public GroupViewModel(Group g)
        {
            _entity = g;
        }

        public void updateGrids(IEnumerable<Response> responses)
        {
            //attempts -> groupviewmodel
            if (_grids != null && _grids[0] != null && responses.Any())
                _grids[0].updateCell(responses.First().Attempt);

            //responses -> assignmentviewmodel
            if (_assignments != null)
                foreach (var a in _assignments)
                    a.updateGrids(responses.Where(r => r.AssignmentNum == a._entity.AssignmentNum));
        }

        //NEW GRID - col: deadline, row: Seat, cell: marks (from attempts)

        private ObservableCollection<AssignmentViewModel> _assignments;
        public ObservableCollection<AssignmentViewModel> assignments
        {
            get
            {
                if (_assignments == null)
                {
                    _assignments = new ObservableCollection<AssignmentViewModel>(_entity.Assignments.Select(s => new AssignmentViewModel(s)));
                }
                return _assignments;
            }
        }

        private ObservableCollection<SeatViewModel> _seats;
        public ObservableCollection<SeatViewModel> seats
        {
            get
            {
                if (_seats == null)
                    _seats = new ObservableCollection<SeatViewModel>(_entity.Seats.Select(s => new SeatViewModel(s)));
                return _seats;
            }
        }

        public string name
        {
            get { return _entity.Name; }
            set
            {
                _entity.Name = value;
                RaisePropertyChanged("name");
            }
        }


        private ObservableCollection<GridViewModelBase<Assignment, Seat, Attempt>> _grids;
        public ObservableCollection<GridViewModelBase<Assignment, Seat, Attempt>> grids
        {
            get
            {
                if (_grids == null)
                {
                    _grids = new ObservableCollection<GridViewModelBase<Assignment, Seat, Attempt>>() { };

                    //SINGLE ATTEMPTS
                    _grids.Add(new GridViewModelBase<Assignment, Seat, Attempt>(
                        null,
                        "Attempts",
                        "Student",
                        () => { return _entity.Assignments.OrderBy(a => a.AssignmentNum); },
                        () => { return _entity.Seats.OrderBy(s => s.SeatNum); },
                        () => { return _entity.Attempts; },
                        (Attempt a) => { return new[] { (int)a.AssignmentNum - 1, (int)a.SeatNum -1 }; },
                        new List<ColumnSettings<Assignment, Attempt>>()
                        { 
                            new ColumnSettings<Assignment, Attempt>()
                            {
                                colHeaderValue = d => d.Name,
                                isVisible = true,//__entity.isGridVisible(0),
                                listboxDescriptionName = "Score",
                                colorOperator = d => System.Windows.Media.Brushes.Black,
                                valueOperator  = (Attempt entity) => {
                                    return !entity.Responses.Any() ? string.Empty  : entity.Score.ToString(); 
                                },
                                isImage = false,
                                columnViewModelConstructor = e => e == null ? null : new AssignmentViewModel(e)
                            },
                            new ColumnSettings<Assignment, Attempt>()
                            {
                                colHeaderValue = d => d.Name,
                                isVisible = true, //__entity.isGridVisible(1),
                                listboxDescriptionName = "Days Late",
                                colorOperator = (Attempt entity)  => 
                                {
                                    return entity.daysLate > 0  ?
                                        System.Windows.Media.Brushes.Red :
                                        System.Windows.Media.Brushes.Black;
                                },
                                valueOperator  = (Attempt entity) => { 
                                    return entity.whenReturned == null ? string.Empty : entity.daysLate.ToString(); 
                                },
                                isImage = false,
                                columnViewModelConstructor = e => e == null ? null : new AssignmentViewModel(e)
                            }
                        },
                        new RowSettings<Seat, Attempt>()
                        {
                            rowHeaderOperator = p => p.LastName + " " + p.FirstName,
                            rowViewModelConstructor = s => s == null ? null : new SeatViewModel(s)
                        })
                        );

                    //thresholds.PropertyChanged += (sender, e) =>
                    //{
                    //    //when thresholds change, update grade cell value        
                    //    if (e.PropertyName == "number" || e.PropertyName == "gradeLetterOrNumber")
                    //        grids[0].readOnlySettings[3].RaisePropertyChanged("cellValue");
                    //};


                    //grids[0].VisibilityChanged += (object sender, VisibilityChangedEventArgs e) =>
                    //{
                    //    //when cell visibility changes, update stored TCS value
                    //    __entity.setGridVisibility(e.IndexOfCellSettings, e.isVisible);
                    //};

                    //OUTCOMES
                    //_grids.Add(new GridViewModelBase<Outcome, Seat, Mark>(__entity.marksGrid)
                    //{
                    //    legend = DeadlineViewModel.legend,
                    //    cornerText = "Seat name",
                    //    _columnSettings = new List<ColumnSettings<Outcome, Mark>>()
                    //    { 
                    //        new ColumnSettings<Outcome, Mark>()
                    //        {
                    //            colHeaderValue = d => d.Description,
                    //            isVisible = __entity.isGridVisible(0),
                    //            listboxDescriptionName = "Best Completion",
                    //            colorOperator = d => "black",
                    //            cellCurrentItemOperator = p => p.Any() ? new MarkViewModel(p.Last()) : null,
                    //            valueOperator  = delegate(IEnumerable<Mark> entities) 
                    //            {
                    //                return entities.Any() ? (int?)entities.Max(a => a.CompletionZeroBased) : (int?)null;
                    //            },
                    //            isImage = false,
                    //            columnViewModelConstructor = e => e == null ? null : new OutcomeViewModel(e)
                    //        }
                    //    },
                    //    rowSettings = new RowSettings<Seat, Mark>()
                    //    {
                    //        rowHeaderOperator = p => p.LastName + " " + p.FirstName, //Seat name
                    //        currentItemOperator = s => s == null ? null : new SeatViewModel(s)
                    //    }
                    //});

                    

                }
                return _grids;
            }
        } //end of grids property

        //private ThresholdsCollectionViewModel _thresholds;
        //public ThresholdsCollectionViewModel thresholds
        //{
        //    get
        //    {
        //        if (_thresholds == null)
        //            _thresholds = new ThresholdsCollectionViewModel(__entity.thresholds);

        //            //if the threshold properties are changed trigger update of thresholdsstring 
        //            _thresholds.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        //            {
        //                //IF PROPERTYNAME
        //                    __entity.ThresholdsAsString = _thresholds.__entity.toString();
        //            };
        //        return _thresholds;
        //    }
        //}

        //public bool thresholdsAreAbsolute
        //{
        //    get
        //    {
        //        return __entity.ThresholdsAreAbsolute;
        //    }
        //    set
        //    {
        //        __entity.ThresholdsAreAbsolute = value;
        //        __entity.thresholds.isAbsolute = value;
        //    }
        //}

        protected ICommand _emailProgressCommand;
        public ICommand emailProgress
        {
            get
            {
                if (_emailProgressCommand == null)
                    this._emailProgressCommand = new DelegateCommand(this._emailProgressMethod);
                return this._emailProgressCommand;
            }
        }
        protected virtual void _emailProgressMethod(object o) //NULL PARAMETER
        {
            //IChart<Seat> lineChart = grids[0].chart;

            //foreach (Seat s in grids[0].selectedRowEntities)
            //{
            //    MailItem mailItem = (MailItem)Globals.ThisAddIn.Application.CreateItem(OlItemType.olMailItem);
            //    mailItem.Subject = string.Format(@"{0} Progress report",__entity.Course.Name);
            //    mailItem.To = s.Email;
            //    mailItem.Body = string.Format("This is an automated email. A chart showing your progress in {0} is included as an attachment"
            //        ,__entity.Course.Name);
            //    mailItem.Attachments.Add(lineChart.saveSeatProgressChartReturnsTempFileName(s));
            //    mailItem.Send();
            //}
            //MessageBox.Show(string.Format("a progress report has been emailed to {0} Seats", grids[0].selectedRowEntities.Count() ));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
