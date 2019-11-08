using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.ComponentModel;
using AME_base;

namespace AME_base
{
    public class SeatViewModel : INotifyPropertyChanged
    {
        public Seat _entity;
        public SeatViewModel(Seat s) 
        {
            _entity = s;
        }

        
        public string firstName
        {
            get { return _entity.FirstName; }
            set
            {
                _entity.FirstName = value;
                RaisePropertyChanged("firstName");
            }
        }

        public string lastName
        {
            get { return _entity.LastName; }
            set
            {
                _entity.LastName = value;
                RaisePropertyChanged("lastName");
            }
        }

        public string email { get { return _entity.Email; } }

        public string alternateEmail 
        {
            get { return _entity.AlternateEmail; }
            set
            {
                _entity.AlternateEmail = value;
                RaisePropertyChanged("alternateEmail");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
