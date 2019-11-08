using AME_base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AME_base
{
    public class RowViewModel: INotifyPropertyChanged
    {
        Row _entity;
        public RowViewModel(Row q) 
        {
            _entity = q;
        }


        //public ResponseElementSummaries responseSummaries
        //{
        //    get
        //    {
        //        return _entity.responseSummaries;
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
