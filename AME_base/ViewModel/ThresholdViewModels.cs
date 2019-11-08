using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AME_base
{
    public class ThresholdViewModel : INotifyPropertyChanged
    {
        private Threshold _entity;
        public ThresholdViewModel(Threshold t) 
        {
            _entity = t;
        }

        public string description
        {
            get { return _entity.description; }
            set 
            { 
                _entity.description = value;
                RaisePropertyChanged("description");
            }
        }
        public string number
        {
            get { return _entity.value.ToString(); }
            set 
            {
                decimal trialResult;
                if (decimal.TryParse(value,
                    System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out trialResult) && trialResult > 0)
                {
                    _entity.value = trialResult;
                    RaisePropertyChanged("number");
                }
            }
        }
        public string gradeLetterOrNumber
        {
            get { return _entity.gradeLetterOrNumber.ToString(); }
            set 
            {
                if (value.Length > 0)
                {
                    _entity.gradeLetterOrNumber = value[0];
                    RaisePropertyChanged("gradeLetterOrNumber");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ThresholdsCollectionViewModel: ObservableCollection<ThresholdViewModel>
    {
        public Thresholds _entity;
        public ThresholdsCollectionViewModel(Thresholds thresholds) 
        {
            _entity = thresholds;
            foreach ( var p in thresholds )
                Add(new ThresholdViewModel(p));
        }


        public new void Add(ThresholdViewModel t)
        {
            t.PropertyChanged += (sender, e) =>
                { };//CollectionChanged(e.PropertyName); };
            base.Add(t);
        }

        public new bool Remove(ThresholdViewModel t)
        {
            t.PropertyChanged -= (sender, e) =>
                { }; //RaisePropertyChanged(e.PropertyName); };
            return base.Remove(t);
        }

        private ICommand _createCommand;
        public ICommand create
        {
            get
            {
                if (_createCommand == null)
                    this._createCommand = new DelegateCommand(this._createMethod);
                return this._createCommand;
            }
        }
        public void _createMethod(object obj) //null parameter
        {
            Threshold newT = new Threshold();
            _entity.Add(newT);
            Add(new ThresholdViewModel(newT));
        }

        //private ICommand _deleteCommand;
        //public ICommand delete
        //{
        //    get
        //    {
        //        if (_deleteCommand == null)
        //            this._deleteCommand = new DelegateCommand(this._deleteMethod);
        //        return this._deleteCommand;
        //    }
        //}
        //public void _deleteMethod(object obj) //Thresholdviewmodel parameter
        //{
        //    if (_entity.Remove((obj as ThresholdViewModel).entity))
        //        Remove((ThresholdViewModel)obj);
        //}




    }
}
