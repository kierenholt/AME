using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using AME_base;
using System.ComponentModel;

namespace AME_base
{
    public class MarkViewModel: INotifyPropertyChanged
    {
        private Mark _entity;
        
        public MarkViewModel(Mark m) 
        {
            _entity = m;
        }

        public static readonly string[] levelImages = { "View/icons/cross.png", "View/icons/tick.png", "View/icons/star.png" };
        public static List<LegendItem> legend
        {
            get
            {
                List<LegendItem> retVal = new List<LegendItem>();
                for (int i = 0; i < completionNames.Count(); i++)
                    retVal.Add(new LegendItem() { descriptor = completionNames[i], icon = levelImages[i] });
                return retVal;
            }
        }

        //public bool isEnabled
        //{
        //    get
        //    {
        //        return _entity.isEnabled;
        //    }
        //}

        private static string[] _completionNames;
        public static string[] completionNames
        {
            get
            {
                if (_completionNames == null)
                {
                    return new[] { "undecided","not at all", "partial", "complete"};
                    //_completionNames = new string[Settings.Default.SkillCompletionNames.Count];
                    //Settings.Default.SkillCompletionNames.CopyTo(_completionNames, 0);
                }
                return _completionNames;
            }
        } 

        
        public byte completionIndex //the level achieved 0,1,2 //bound to listbox
        {
            get { return _entity.CompletionZeroBased; }
            set 
            { 
                _entity.CompletionZeroBased = value;
                RaisePropertyChanged("completionIndex");
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
