using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Data.Entity;
using Microsoft.Office.Interop.Outlook;
using System.ComponentModel;
using System.Diagnostics;

namespace AME_base
{
    public class ObservableCollection<VM, E> : ObservableCollection<VM> 
    {
        protected ICollectionView _view;
        public ICollectionView view
        {
            get
            {
                if (_view == null)
                    _view = CollectionViewSource.GetDefaultView(this);
                return _view;
            }
        }
        
        public ObservableCollection<E> collectionEntity { get { return _collectionEntity; } }
        protected ObservableCollection<E> _collectionEntity;


        public ObservableCollection(DbSet<E> dbSet) : base(dbSet.Local.Select(e => new VM(){ entity = e }))
        {
            _collectionEntity = dbSet.Local;
        }

        public ObservableCollection(ObservableCollection<E> oc) : base(oc.Select(e => new VM{ entity = e }))
        {
             _collectionEntity = oc;
        }

        public ObservableCollection(IEnumerable<E> oc) : base(oc.Select(e => new VM { entity = e }))
        {
            _collectionEntity = new ObservableCollection<E>(oc);
        }

        public ObservableCollection(IEnumerable<VM> vms)
            : base(vms)
        {
            _collectionEntity = new ObservableCollection<E>(vms.Select(vm => vm.entity));
        }

        public E selectedEntity
        {
            get
            {
                return selectedViewModel.entity;
            }
            set
            {
                VM targetViewModel = this.FirstOrDefault(vm => vm.entity.myEquals(value as E));
                if (targetViewModel != null) view.MoveCurrentTo(targetViewModel);
            }
        }

        public VM selectedViewModel
        {
            get
            {
                return view.CurrentItem as VM;
            }
            set
            {
                view.MoveCurrentTo(value);
            }
        }

        //protected ICommand _addCommand;
        //public ICommand add
        //{
        //    get
        //    {
        //        if (_addCommand == null)
        //            this._addCommand = new DelegateCommand(this._addMethod); 
        //        return this._addCommand; 
        //    }
        //}
        //protected void _addMethod(object o)
        //{
        //    if (o is VM) //add single viewmodel
        //    {
        //        VM vm = (VM)o; 
        //        if (vm != null && !Contains(vm.entity))
        //            Add(vm);
        //    }
        //    if (o is IEnumerable<VM>) //add all viewmodels in a collection
        //    {
        //        foreach (VM vm in (IEnumerable<VM>)o)
        //            if (!Contains(vm.entity))
        //                Add(vm);
        //    }
        //    if (o is IEnumerable<E>) //add all entities in a collection
        //    {
        //        foreach (E e in (IEnumerable<E>)o)
        //            if (e != null && !Contains(e))
        //                Add(new VM() { entity = e });
        //    }
        //    if (o is E) //add single entity
        //    {
        //        E e = (E)o;
        //        if (e != null && !Contains(e))
        //            Add(new VM() { entity = e });
        //    }
        //}
        
        //protected ICommand _createCommand;
        //public ICommand create
        //{
        //    get 
        //    { 
        //        if (_createCommand == null)
        //            this._createCommand = new DelegateCommand(this._createMethod);
        //        return this._createCommand; 
        //    }
        //}
        //protected virtual void _createMethod(object o) //NULL PARAMETER
        //{
        //    E e = new E();
        //    int i = 0;
        //    while (myContains(e) && e.becomeUnique(i)) //if non unique, keep trying to make the entity unique
        //        i++;
        //    myAdd(new VM()
        //    {
        //        entity = e
        //    });
        //}

        //public new void Add(VM vm)
        //{
        //    base.Add(vm);
        //    _collectionEntity.Add(vm.entity);
        //}

        //public new void Remove(VM vm)
        //{
        //    base.Remove(vm);
        //    _collectionEntity.Remove(vm.entity);
        //}

        //public new bool Contains(E e)
        //{
        //    return _collectionEntity.Any(coll => coll.myEquals(e)); ;
        //}

        //protected ICommand _deleteCommand;
        //public ICommand delete
        //{
        //    get 
        //    { 
        //        if (_deleteCommand == null)
        //            this._deleteCommand = new DelegateCommand(this._deleteMethod);
        //        return this._deleteCommand; 
        //    }
        //}
        //protected virtual void _deleteMethod(object m) //ViewModelBase
        //{
        //    VM vm = (VM)m;
        //    if (vm is VM && vm != null && Contains(vm.entity))
        //        this.Remove(vm);
        //}

    }

}