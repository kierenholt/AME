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
    public class CompletedWorkViewModel
    {
        private CompletedWork _work;
        private Action _onOKButtonPressed;
        public CompletedWorkViewModel(CompletedWork paramWork, Action paramOnOKButtonPressed)
        {
            _onOKButtonPressed = paramOnOKButtonPressed;
            _work = paramWork;
        }

        public string outOf
        {
            get
            {
                return _work.assignment.OutOf.ToString();
            }
        }

        private AttemptViewModel _attempt;
        public AttemptViewModel attempt
        {
            get { return _attempt ?? (_attempt = new AttemptViewModel(_work.attemptHTML )); }
        }


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
                                _onOKButtonPressed();
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
                            }
                        );
                return this._CancelButtonClickCommand;
            }
        }
    }
}
