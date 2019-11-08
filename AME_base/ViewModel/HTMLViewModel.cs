using AME_base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{

    public class HTMLViewModel : INotifyPropertyChanged
    {
        //protected RowAssignment _entity { get { return _HTML._rowAssignment ; }  }
        protected HTML _HTML;

        public HTMLViewModel(HTML ra) 
        {
            _HTML = ra;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        //public ResponseElementSummaries responseSummaries
        //{
        //    get
        //    {
        //        return _entity.esponseSummaries;
        //    }
        //}
    }
}
