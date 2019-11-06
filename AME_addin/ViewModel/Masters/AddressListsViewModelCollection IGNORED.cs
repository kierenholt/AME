using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AME_addin;

//how to use MVVM
//http://markVM-dot-net.blogspot.co.uk/2008/12/list-filtering-in-wpf-with-m-v-vm.html

namespace AME_addin
{
    public class AddressListsObservableCollection : Collection<addressListViewModel>
    {
        //private ICollectionView _view;
        //private bool _showOnlyGroupsFilter; //PASSED ON TO CHILDREN
        
        public AddressListsObservableCollection()
        {
            foreach (AddressList a in OutlookProvider.addressLists)
                Items.Add(new addressListViewModel(a));
            //_view = CollectionViewSource.GetDefaultView(addressLists);
        }

        //public bool showOnlyGroupsFilter
        //{
        //    get
        //    {
        //        return _showOnlyGroupsFilter;
        //    }
        //    set
        //    {
        //        _showOnlyGroupsFilter = value;
        //        foreach (addressListViewModel am in addressLists)
        //        {
        //            am.showOnlyGroupsFilter = _showOnlyGroupsFilter;
        //        }
        //        _view.Refresh();
        //    }
        //}
    }
}
