using AME_addin.Properties;
using AME_base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AME_addin
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            _cancelCommand = new DelegateCommand(_cancelMethod);
            _saveChangesCommand = new DelegateCommand(_saveChangesMethod);
            _closeWindowCommand = new DelegateCommand(_CloseWindowMethod, delegate(object obj) { return true; });
        }

        public Boolean InteruptMarkingBelowPercentEnabled
        {
            get { return Settings.Default.InteruptMarkingBelowPercentEnabled; }
            set { Settings.Default.InteruptMarkingBelowPercentEnabled = value; }
        }

        public int InteruptMarkingBelowPercentValue
        {
            get { return Settings.Default.InteruptMarkingBelowPercentValue; }
            set { Settings.Default.InteruptMarkingBelowPercentValue = value; }
        }

        public int DaysFromHandedOutToExpectedIn
        {
            get { return Settings.Default.DaysFromHandedOutToExpectedIn; }
            set { Settings.Default.DaysFromHandedOutToExpectedIn = value; }
        }

        public bool deleteMarkedEmails
        {
            get { return Settings.Default.deleteMarkedEmails; }
            set { Settings.Default.deleteMarkedEmails = value; }
        }

        public bool ItemSendEventEnabled
        {
            get { return Settings.Default.ItemSendEventEnabled; }
            set { Settings.Default.ItemSendEventEnabled = value; }
        }
        

        //setting work

        public bool shuffleQuestionsEnabled
        {
            get { return Settings.Default.shuffleQuestionsEnabled; }
            set { Settings.Default.shuffleQuestionsEnabled = value; }
        }

        public bool includeSolutions
        {
            get { return Settings.Default.includeSolutions; }
            set { Settings.Default.includeSolutions = value; }
        }

        public int markLimitValue
        {
            get { return Settings.Default.markLimitValue; }
            set { Settings.Default.markLimitValue = value; }
        }




        

        //private ObservableCollection<ObservableString> _skillCompletionNames;
        //public ObservableCollection<ObservableString> skillCompletionNames
        //{
        //    get 
        //    {
        //        if (_skillCompletionNames == null)
        //        {
        //            string[] str = new string[Settings.Default.SkillCompletionNames.Count];
        //            Settings.Default.SkillCompletionNames.CopyTo(str, 0);
        //            _skillCompletionNames = new ObservableCollection<ObservableString>(str.Select(s => new ObservableString(s)));
        //            foreach (ObservableString os in _skillCompletionNames)
        //                os.PropertyChanged += (sender, e) =>
        //                    {
        //                        StringCollection stringColl = new StringCollection() {};
        //                        stringColl.AddRange(_skillCompletionNames.Select(s => s.str).ToArray());
        //                        Settings.Default.SkillCompletionNames = stringColl;
        //                    };
        //        }
        //        return _skillCompletionNames;
        //    }
        //}

        //commands
        private ICommand _cancelCommand;
        public ICommand cancel
        {
            get { return this._cancelCommand; }
        }
        private void _cancelMethod(object name)
        {
            Settings.Default.Reload();
        }

        private ICommand _closeWindowCommand;
        public ICommand closeWindow
        {
            get { return this._closeWindowCommand; }
        }
        public void _CloseWindowMethod(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }

        private ICommand _saveChangesCommand;
        public ICommand saveChanges
        {
            get { return this._saveChangesCommand; }
        }
        private void _saveChangesMethod(object name)
        {
            Settings.Default.Save();
        }
    }
}
