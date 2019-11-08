using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data.Entity;
using Microsoft.Office.Interop.Outlook;
using System.Windows.Media;

using System.IO;
using System.ComponentModel;
using AME_base;



namespace AME_base
{

    public enum MarkLimitEnabledState
    {
        disabled = 0,
        percent = 1,
        absolute = 2
    }

    //NOT A VIEWMODEL TO MARK RESPONSES BY - USE COMPLETEDWORKVIDEMODEL INSTEAD

    public class AttemptViewModel : INotifyPropertyChanged
    {
        private Attempt _entity { get { return attemptHTML.attempt; } }
        public AttemptHTML attemptHTML { get; private set; }
        public AttemptViewModel(AttemptHTML a) 
        {
            attemptHTML = a;
        }


        private ObservableCollection<MarkViewModel> _marks;
        public ObservableCollection<MarkViewModel> marks
        {
            get { return _marks ?? (_marks = new ObservableCollection<MarkViewModel>(_entity.Marks.Select(m => new MarkViewModel(m)))); }
        }

        
        private ObservableCollection<ResponseViewModel> _responses;
        public ObservableCollection<ResponseViewModel> responses
        {
            get 
            { 
                if (_responses == null)
                {
                    _responses = new ObservableCollection<ResponseViewModel>(attemptHTML.responseHTMLs.Select(r => new ResponseViewModel(r)));
                    foreach (ResponseViewModel r in _responses)
                        r.PropertyChanged += (s, e) =>
                        {
                            _entity.updateScore();
                            RaisePropertyChanged("score");
                        };
                }
                return _responses;
            }
        }



        public String whenHandedOutString
        {
            get { return _entity.whenAssigned.ToShortDateString(); }
            //set { entity.whenHandedOut = DateTime.Parse(value); }
        }

        public DateTime whenHandedOut
        {
            get { return _entity.whenAssigned; }
            //set { entity.whenHandedOut = value; }
        }


        //public DateTime whenHandedIn
        //{
        //    get
        //    {
        //        if (_entity.whenHandedIn == DateTime.MinValue)
        //            _entity.whenHandedIn = OutlookProvider.selectedMailItem.ReceivedTime;
        //        return _entity.whenHandedIn;
        //    }
        //    set
        //    {
        //        _entity.whenHandedIn = value;
        //    }
        //}

        public int daysLate
        {
            get { return 0; }// _entity.getDaysLate(); }
        }

        //NECESSARY FOR DETAIL VIEW BECAUSE IF YOU DELETE THE NUMBER IT THROWS AN EXCEPTION
        public string daysLateString
        {
            get { return daysLate.ToString(); }
        }


        public int score
        {
            get 
            {
                return _entity.Score; 
            }
        }

        //NECESSARY FOR DETAIL VIEW BECAUSE IF YOU DELETE THE NUMBER IT THROWS AN EXCEPTION
        public string scoreAsString
        {
            get { return score.ToString(); }
        }

        public Brush latenessColor { get { return daysLate < 0 ? Brushes.LightGreen : Brushes.LightPink; } }
        //public string latenessImage { get { return DeadlineViewModel.latenessImages[daysLate < 1 ? 0 : 1]; } }
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}


