using AME_base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AME_base
{
    public class RowSettings<TRowEntity, TCellEntity> : INotifyPropertyChanged
    {
        public Func<TRowEntity, string> rowHeaderOperator;
        public Func<TRowEntity, INotifyPropertyChanged> rowViewModelConstructor;

        public Func<IGridRowViewModel,bool> ifRowFilterEnabledVisibleWhen;
        private bool _filterActive = false;
        public bool filterActive
        {
            get { return _filterActive; }
            set 
            {
                _filterActive = value;
                RaisePropertyChanged("filterActive");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface IGridRowViewModel
    {
        string value { get; }
        ICommand cellClick { get; }
        IEnumerable<ICellViewModel> readOnlyCells { get; }
    }

    public class GridRowViewModel<TRowEntity, TCellEntity> : IGridRowViewModel
    {
        private RowSettings<TRowEntity, TCellEntity> _rowSettings;
        public IEnumerable<ICellViewModel> readOnlyCells { get { return _observableGrouping.ToArray(); } }
        public bool isSelected = false;
        private ObservableGrouping<TRowEntity, CellViewModel<TCellEntity>> _observableGrouping;
        public ObservableGrouping<TRowEntity, CellViewModel<TCellEntity>> observableGrouping { get { return _observableGrouping; } }
        public int colSettingsCount;

        public GridRowViewModel(TRowEntity paramKey,
            RowSettings<TRowEntity, TCellEntity> paramRowSettings,
            int capacity,
            IEnumerable<ColumnSettings<TCellEntity>> paramColumnSettings
            )
        {
            List<CellViewModel<TCellEntity>> list = new List<CellViewModel<TCellEntity>>();
            for (int i = 0; i < capacity; i++)
                foreach (var cs in paramColumnSettings)
                    list.Add(new CellViewModel<TCellEntity>(cs));
            _observableGrouping = new ObservableGrouping<TRowEntity,CellViewModel<TCellEntity>>(paramKey,list);

            colSettingsCount = paramColumnSettings.Count();
            _rowSettings = paramRowSettings;
        }

        //0-based index
        public void Add(TCellEntity entity, int index, Action<INotifyPropertyChanged> setClickObject)
        {
            int startIndex = index * colSettingsCount;
            for (int i = startIndex; i < startIndex + colSettingsCount; i++)
                _observableGrouping[i].setEntity(entity, setClickObject);
        }

        public string value
        {
            get
            {
                return _rowSettings.rowHeaderOperator(_observableGrouping.Key);
            }
        }


        private Action<object> _onClick;
        protected ICommand _cellClickCommand;
        public ICommand cellClick
        {
            get
            {
                //if (_cellClickCommand == null)
                //    this._cellClickCommand = new DelegateCommand(_onClick);
                //return this._cellClickCommand;
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
