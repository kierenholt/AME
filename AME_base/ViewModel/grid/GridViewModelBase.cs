using AME_base;
using System.ComponentModel; 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Data;
using DocumentFormat.OpenXml;


namespace AME_base
{
    public class LegendItem
    {
        public string icon { get; set; }
        public string descriptor { get; set; }
    }

    public interface IGridViewModelBase : INotifyPropertyChanged //for binding to readonlysettings
    {
        string cornerText { get; }
        void deselectRow(int index);
        void selectRow(int index);
        event EventHandler<VisibilityChangedEventArgs> VisibilityChanged;
        IEnumerable<IGridRowViewModel> readOnlyRows { get; }
        IEnumerable<IColumnViewModel> readOnlyColumns { get; }
        IList<ColumnSettings> readOnlySettings { get; } 
    }

    //COPY THE ObservableCollection
    public interface IGridViewModelBase<TRowEntity> : IGridViewModelBase 
    {
        IEnumerable<TRowEntity> selectedRowEntities { get; }
        //IChart<TRowEntity> chart { get; }
    }


    public class GridViewModelBase<TColumnEntity, TRowEntity, TCellEntity> : IGridViewModelBase<TRowEntity>, 
        INotifyPropertyChanged
    {        
        //SETTINGS - MUST MATCH UP!
        public RowSettings<TRowEntity, TCellEntity> rowSettings { get; set; } //ONE PER GRID
        public List<ColumnSettings<TColumnEntity,TCellEntity>> _columnSettings { get; set; } //MANY PER GRID - paired to cellsettings
        public IList<ColumnSettings> readOnlySettings { get { return _columnSettings.ToArray(); } } 

        public GridViewModelBase(
            List<LegendItem> paramLegend,
            string paramName,
            string paramCornerText,
            Func<IEnumerable<TColumnEntity>> paramcolumnsGetter,
            Func<IEnumerable<TRowEntity>> paramrowsGetter,
            Func<IEnumerable<TCellEntity>> paramcellsGetter,
            Func<TCellEntity,int[]> paramcellXYGetter,
            List<ColumnSettings<TColumnEntity,TCellEntity>> paramColumnSettings,
            RowSettings<TRowEntity,TCellEntity> paramRowSettings
        )
        {
            legend = paramLegend;
            name = paramName;
            cornerText = paramCornerText;
            _columnSettings = paramColumnSettings;
            rowSettings = paramRowSettings;
            _columnsGetter = paramcolumnsGetter;
            _rowsGetter = paramrowsGetter;
            _cellsGetter = paramcellsGetter;
            _cellXYGetter = paramcellXYGetter;
        }

        public void updateCell(TCellEntity entity)
        {
            int x = _cellXYGetter(entity)[0];
            int y = _cellXYGetter(entity)[1];
            _rows[y].Add(entity, x, setClickObject);
        }

#region initialisation

        private Func<IEnumerable<TRowEntity>> _rowsGetter;
        private Func<IEnumerable<TColumnEntity>> _columnsGetter;
        private Func<IEnumerable<TCellEntity>> _cellsGetter;
        private Func<TCellEntity, int[]> _cellXYGetter;
        public bool initialized = false;
        private int _rowCount;
        private int _colCount;
        private void Initialize()
        {
            IEnumerable<TRowEntity> rowHeaders = _rowsGetter();
            IEnumerable<TColumnEntity> colHeaders = _columnsGetter();
            _rowCount  = rowHeaders.Count();
            _colCount = colHeaders.Count();


            //EMPTY ROWS
            _rows = new ObservableCollection<GridRowViewModel<TRowEntity, TCellEntity>>();
            foreach (var rowEntity in rowHeaders)
            {
                GridRowViewModel<TRowEntity, TCellEntity> rvm = new GridRowViewModel<TRowEntity, TCellEntity>(
                    rowEntity,
                    rowSettings,
                    _colCount,
                    _columnSettings);

                _rows.Add(rvm);
            }

            //FILL CELLS
            foreach (TCellEntity entity in _cellsGetter())
                updateCell(entity);

            //COLUMNS
            _columns = new ObservableCollection<ColumnViewModel<TColumnEntity>>();
            foreach (var columnEntity in colHeaders)
                foreach (var columnSetting in _columnSettings)
                    _columns.Add(new ColumnViewModel<TColumnEntity>(columnEntity,
                            setClickObject,
                            columnSetting, 
                            columnSetting.colHeaderValue,
                            columnSetting.columnViewModelConstructor));                
            
            //SETTINGS
            foreach (var columnSetting in _columnSettings)
            {
                //when visibility is changed, update visibleColumns
                columnSetting.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "isVisible")
                            RaisePropertyChanged("visibleColumns");
                    };
            }

            initialized = true;
        }
        
        private ObservableCollection<ColumnViewModel<TColumnEntity>> _columns;
        public ObservableCollection<ColumnViewModel<TColumnEntity>> columns 
        {
            get
            {
                if (!initialized)
                    Initialize();
                return _columns;
            }
        }
        
        private ObservableCollection<GridRowViewModel<TRowEntity, TCellEntity>> _rows;
        public ObservableCollection<GridRowViewModel<TRowEntity, TCellEntity>> rows
        {
            get 
            {
                if (!initialized)
                    Initialize();
                return _rows;
            } 
        }


#endregion

        //GET SET PROPERTIES
        public string name { get; private set; }
        public string cornerText { get; private set; }
        public List<LegendItem> legend { get; set; }

        #region updateCell


        #endregion

        //public IChart<TRowEntity> chart 
        //{
        //    get
        //    {
        //        return _gridData.chart;
        //    }
        //}

#region sorting
        //SORT BY COLUMN
        private ColumnViewModel<TColumnEntity> _sortColumn;
        public ColumnViewModel<TColumnEntity> sortColumn
        {
            get
            {
                return _sortColumn;
            }
            set
            {
                _view.SortDescriptions.Clear();
                int foundIndex = _columns.IndexOf(value);
                _view.SortDescriptions.Add(foundIndex > -1 ?
                    new SortDescription(
                        String.Format("[{0}].value", _columns.IndexOf(value).ToString()),
                        ListSortDirection.Ascending) :
                    new SortDescription(
                        "value",
                        ListSortDirection.Ascending)
                    );
                _view.Refresh();
                _sortColumn = value;
            }
        }
        public IEnumerable<ColumnViewModel<TColumnEntity>> visibleColumns
        {
            get
            {
                List<ColumnViewModel<TColumnEntity>> retVal = columns.Where(c => c.settings.isVisible).ToList();
                retVal.Insert(0, new ColumnViewModel<TColumnEntity>(e => "Seat name"));
                return retVal;
            }
        }
#endregion

        //collection of columnviewmodels
        public IEnumerable<IColumnViewModel> readOnlyColumns
        {
            get
            {
                return columns.ToArray();
            }
        }
        
            
            
        //collection of GridGridRowViewModels
        private ICollectionView _view; //bound to readonlyrows
        public IEnumerable<IGridRowViewModel> readOnlyRows
        {
            get
            {
                IEnumerable<IGridRowViewModel> retVal = rows.ToArray();
                _view = CollectionViewSource.GetDefaultView(retVal);
                _view.Filter = delegate(object obj)
                {
                    bool isShown = true;
                    IGridRowViewModel rvm = obj as IGridRowViewModel;
                    if (obj != null && rowSettings.ifRowFilterEnabledVisibleWhen != null)
                        isShown = !rowSettings.filterActive || rowSettings.ifRowFilterEnabledVisibleWhen(rvm);
                    return isShown;
                };
                rowSettings.PropertyChanged += (sender, e) => //when rowsettings filteractive changes, update the view
                {
                    if (e.PropertyName == "filterActive")
                        _view.Refresh();
                };
                

                return retVal;
            }
        }

        //binds to listview selectedItems

        public void deselectRow(int index)
        {
            if (index >= 0)
                rows[index].isSelected = false;
        }
        public void selectRow(int index)
        {
            if (index >= 0)
                rows[index].isSelected = true;
        }

        public IEnumerable<GridRowViewModel<TRowEntity, TCellEntity>> selectedRows
        {
            get
            {
                return rows.Where(r => r.isSelected);
            }
        }
        
        public IEnumerable<TRowEntity> selectedRowEntities
        {
            get
            {
                return selectedRows.Select(r => r.observableGrouping.Key);
            }
        }





        protected ICommand _exportCommand;
        public ICommand export
        {
            get
            {
                if (_exportCommand == null)
                    this._exportCommand = new DelegateCommand(this._exportMethod);
                return this._exportCommand;
            }
        }
        protected virtual void _exportMethod(object o) //NULL PARAMETER
        {
            OpenXML.createMarkSpreadsheet(new Dictionary<string, IGridViewModelBase>()
                { { this.name, this as IGridViewModelBase } }
                );
        }

        //CLICKOBJECT
        private INotifyPropertyChanged _clickObject;
        public INotifyPropertyChanged clickObject
        {
            get { return _clickObject; }
        }
        public void setClickObject(INotifyPropertyChanged value)
        {
            _clickObject = value;
            RaisePropertyChanged("clickObject");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler<VisibilityChangedEventArgs> VisibilityChanged;

    }
}
