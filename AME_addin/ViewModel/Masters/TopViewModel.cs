using System;

using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using AME_base;

namespace AME_addin
{

    public class TopViewModel //: INotifyPropertyChanged
    {
        public SchoolContext _db;
        private Action<TopViewModel> removeEvent;

        public TopViewModel(SchoolContext paramDb, Action<TopViewModel> paramRemoveEvent) //initialise new connection
        {
            _db = paramDb;
            removeEvent = paramRemoveEvent;
        }

        public void updateGrids(object s, ResponsesMarkedEventArgs e)
        {
            if (_teachingGroups != null)
                foreach (var g in _teachingGroups)
                    g.updateGrids(e.responses.Where(r => r.GroupID == g._entity.GroupID));

        }

        private ObservableCollection<GroupViewModel> _teachingGroups;
        public ObservableCollection<GroupViewModel> teachingGroups
        {
            get
            {
                return _teachingGroups ?? (_teachingGroups = new ObservableCollection<GroupViewModel>(_db.Groups.ToList().Select(g => new GroupViewModel(g))));
            }
        }

        private ObservableCollection<SeatViewModel> _Seats;
        public ObservableCollection<SeatViewModel> Seats
        {
            get
            {
                return _Seats ?? (_Seats = new ObservableCollection<SeatViewModel>(_db.Seats.Select(s => new SeatViewModel(s))));
            }
        }



        #region commands

        private ICommand _closeWindowCommand;
        public ICommand closeWindow
        {
            get 
            {
                if (_closeWindowCommand == null)
                    
                    _closeWindowCommand = new DelegateCommand(
                        (o) => 
                            {        
                                Window win = o as Window;
                                win.Close();
                                removeEvent(this);
                            });
                return _closeWindowCommand; 
            }
        }
        public void _CloseWindowMethod(object obj)
        {
        }

        private ICommand _saveChangesCommand;
        public ICommand saveChanges
        {
            get 
            {
                if (_saveChangesCommand == null)
                    _saveChangesCommand = new DelegateCommand(_saveChangesMethod);
                return _saveChangesCommand; 
            }
        }
        private void _saveChangesMethod(object name)
        {
            _db.SaveChanges();
        }
        #endregion
        
    }
}
