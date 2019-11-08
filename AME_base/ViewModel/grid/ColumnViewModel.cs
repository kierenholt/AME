using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AME_base
{
    public abstract class ColumnSettings: INotifyPropertyChanged
    {
        public int index { get; set; }

        private bool _isVisible = true;
        public bool isVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged("isVisible");
                RaisePropertyChanged("colWidth");
                if (this.VisibilityChanged != null)
                    this.VisibilityChanged(this, new VisibilityChangedEventArgs(index, value));
            }
        }
        public double colWidth
        {
            get
            {
                return isVisible ? double.NaN : 0;
            }
        }
        public event EventHandler<VisibilityChangedEventArgs> VisibilityChanged;

        public string listboxDescriptionName { get; set; }
        public bool isImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class ColumnSettings<TCellEntity> : ColumnSettings
    {
        //CELL
        public Func<TCellEntity, string> imageOperator; //if isImage is true, this takes the cellValue as input and converts to image
        public Func<TCellEntity, dynamic> valueOperator;
        //https://msdn.microsoft.com/en-us/library/system.windows.media.colors(v=vs.110).aspx
        public Func<TCellEntity, System.Windows.Media.Brush> colorOperator;
        public Func<TCellEntity, INotifyPropertyChanged> cellViewModelConstructor;
    }

    public class ColumnSettings<TColumnEntity, TCellEntity> : ColumnSettings<TCellEntity>
    {
        //COLUMN
        public Func<TColumnEntity,string> colHeaderValue;
        public Func<TColumnEntity,INotifyPropertyChanged> columnViewModelConstructor;
    }

    public class VisibilityChangedEventArgs : EventArgs
    {
        public int IndexOfCellSettings;
        public bool isVisible;

        public VisibilityChangedEventArgs(int paramIndexOfCellSettings, bool paramIsVisible)
        {
            IndexOfCellSettings = paramIndexOfCellSettings;
            isVisible = paramIsVisible;
        }
    }

    public interface IColumnViewModel
    {
        string value { get; }
        ICommand cellClick { get; }
        ColumnSettings settings { get; }
    }

    public class ColumnViewModel<TColumnEntity> : IColumnViewModel
    {
        public TColumnEntity _entity; //deadline or question
        public ColumnViewModel(Func<TColumnEntity, string> paramColHeaderOperator) //USED FOR 'Seat NAME' SORT SETTINGS
        {
            valueOperator = paramColHeaderOperator;
        }

        public ColumnViewModel(TColumnEntity paramEntity,
            Action<INotifyPropertyChanged> setClickObject,
            ColumnSettings paramColumnSettings,
            Func<TColumnEntity,string> paramColHeaderOperator, 
            Func<TColumnEntity, INotifyPropertyChanged> columnViewModelConstructor)
        {
            _entity = paramEntity;
            valueOperator = paramColHeaderOperator;
            _settings = paramColumnSettings;
            _onClick = (o) => { setClickObject(columnViewModelConstructor(paramEntity)); };
        }


        private ColumnSettings _settings;
        public ColumnSettings settings
        {
            get
            {
                return _settings;
            }
            set
            {
                //when cellsettings property changes trigger value propertychange 
                _settings = value;
                _settings.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "cellValue")
                        RaisePropertyChanged("value");
                };

            }
        }

        public Func<TColumnEntity, string> valueOperator;
        public string value
        {
            get
            {
                return valueOperator(_entity);
            }
            set
            {
                //nothing
            }
        }


        //BOUND TO DATAGRIDS/LISTVIEWS
        private Action<object> _onClick;
        protected ICommand _cellClickCommand;
        public ICommand cellClick
        {
            get
            {
                if (_cellClickCommand == null)
                    this._cellClickCommand = new DelegateCommand(_onClick);
                return this._cellClickCommand;
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
