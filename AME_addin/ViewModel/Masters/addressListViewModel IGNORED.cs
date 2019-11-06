using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using System.Collections.ObjectModel;
using System.Windows.Data;


namespace AME_addin
{
    public class addressListViewModel : ViewModelBase<AddressList>
    {
        private ICollectionView _view;

        public string Name
        {
            get
            {
                return _entity.Name;
            }
        }

        public ObservableCollection<AddressEntry> entries
        {
            get;
            private set;
        }

        public addressListViewModel(AddressList a) : base(a)
        {
            _showOnlyGroupsFilter = false;

            var query = from AddressEntry e in _entity.AddressEntries
                        select e;
            entries = new ObservableCollection<AddressEntry>(query);

            _view = CollectionViewSource.GetDefaultView(entries);
            _view.Filter = delegate(object obj)
            {
                AddressEntry e = (AddressEntry)obj;
                bool isShown = String.IsNullOrEmpty(_nameFilter) ? true : e.Name.Contains(_nameFilter);
                isShown &= ( _showOnlyGroupsFilter == OutlookProvider.isGroup(e) );
                return isShown;
            };

            _view.CurrentChanged += delegate
            {
                RaisePropertyChanged("selectedPersonCollection");
                RaisePropertyChanged("selectedTeacher");
                RaisePropertyChanged("selectedSeat");
                RaisePropertyChanged("selectedSeatGroup");
            };
        }

        private string _nameFilter;
        public string nameFilter
        {
            get
            {
                return _nameFilter;
            }
            set
            {
                if (value != _nameFilter)
                {
                    _nameFilter = value;
                    _view.Refresh();
                }
            }
        }

        private bool _showOnlyGroupsFilter;
        public bool showOnlyGroupsFilter
        {
            get
            {
                return _showOnlyGroupsFilter;
            }
            set
            {
                if (value != _showOnlyGroupsFilter)
                {
                    _showOnlyGroupsFilter = value;
                    _view.Refresh();
                }
            }
        }

        private AddressEntry _selected { get { return (_view.CurrentItem as AddressEntry); } }
    }
}
