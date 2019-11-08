using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace AME_base
{
    public interface ICellViewModel: INotifyPropertyChanged //needed for bindings to work
    {
        dynamic value { get; }
        ColumnSettings settings { get; }
    }

    public class CellViewModel<TCellEntity> : ICellViewModel 
    {
        
        public CellViewModel(ColumnSettings<TCellEntity> paramSettings)
        {
            _settings = paramSettings;
        }


        public void setEntity(TCellEntity value, Action<INotifyPropertyChanged> setClickObject)
        {
            _onClick = (o) => { setClickObject(_settings.cellViewModelConstructor(value)); };
            _entity = value;
            RaisePropertyChanged("value");
            RaisePropertyChanged("image");
            RaisePropertyChanged("color");
        }

        private TCellEntity _entity;
        public TCellEntity entity { get { return _entity; } }
        public string image { get { return _entity == null ? string.Empty : _settings.imageOperator(_entity); } }
        public dynamic value { get { return _entity == null ? string.Empty : _settings.valueOperator(_entity); } }               
        //EXPOSED TO DATAGRID, WHICH THEN BINDS DIRECT TO VIEWMODEL
        public System.Windows.Media.Brush color { get { return _entity == null ? System.Windows.Media.Brushes.Black : _settings.colorOperator(_entity); } }


        ColumnSettings ICellViewModel.settings { get { return _settings; } }
        private ColumnSettings<TCellEntity> _settings;
        public ColumnSettings<TCellEntity> settings
        {
            get { return _settings; }
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

        //BOUND TO DATAGRIDS/LISTVIEWS
        private Action<object> _onClick;
        public ICommand cellClick
        {
            get
            {
                return entity == null ? null : new DelegateCommand(_onClick);
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
