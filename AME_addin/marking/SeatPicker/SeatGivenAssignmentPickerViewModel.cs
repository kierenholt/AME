using AME_base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AME_addin
{
    public class SeatGivenAssignmentPickerViewModel
    {
        private IEnumerable<Seat> _Seats ;

        private ObservableCollection<SeatViewModel> _SeatsVM;
        public ObservableCollection<SeatViewModel> SeatsVM 
        {
            get
            {
                if (_SeatsVM == null)
                    _SeatsVM = new ObservableCollection<SeatViewModel>(_Seats.Select(s => new SeatViewModel(s)));
                return _SeatsVM;
            }
        }

        public SeatGivenAssignmentPickerViewModel(IEnumerable<Seat> paramSeats,
            string paramAlternateEmailAddress)
        {
            _Seats = paramSeats;
            _alternateEmailAddress = paramAlternateEmailAddress;
        }

        public SeatViewModel selectedSeatVM { get; set; }

        public bool cancelmarking { get; set; }
        public bool addToAlternateEmail { get; set; }
        private string _alternateEmailAddress;
        public string alternateEmailAddress { get { return _alternateEmailAddress; } }


        protected ICommand _OKButtonClickCommand;
        public ICommand OKButtonClick
        {
            get
            {
                if (_OKButtonClickCommand == null)
                    this._OKButtonClickCommand = new DelegateCommand(
                        (o) =>
                        {
                            Window window = (Window)o;
                            window.Close();
                            //_onOKButtonPressed();
                        }
                        );
                return this._OKButtonClickCommand;
            }
        }

        protected ICommand _CancelButtonClickCommand;
        public ICommand CancelButtonClick
        {
            get
            {
                if (_CancelButtonClickCommand == null)
                    this._CancelButtonClickCommand = new DelegateCommand(
                        (o) =>
                        {
                            Window window = (Window)o;
                            window.Close();
                            cancelmarking = true;
                        }
                        );
                return this._CancelButtonClickCommand;
            }
        }
    }
}
