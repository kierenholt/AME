
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    //question > response > responseelements
    
    public class SubResponse : IToString<SubResponse>, INotifyPropertyChanged   
    {
        private readonly static char DELIMITER = (char)17;

        public Decision decision = Decision.undecided;
        public enum Decision
        {
            undecided = 0,
            blank = 1,
            decided = 2
        }
        public static readonly string[] DecisionAsString = 
        { 
            "undecided", "blank", "decided"
        };


        public SubResponse() {} //only used in conjunction with .fromstring

        public SubResponse(int paramSubQuestionNum) : this(string.Empty, paramSubQuestionNum) { } //needed for StringableCollection constructor

        //from pdffield or other response
        public SubResponse(
            string paramText, 
            int paramSubQuestionNum)
        {
            _subQuestionIndex = paramSubQuestionNum;

            _Text = new Regex(@"x10\^").Replace(paramText, @"E");
            _Text = new Regex(@"\*10\^").Replace(_Text, @"E"); //value

            _Text = paramText;
        }

        //DEBUG ONLY - used in SEED method of school context
        public SubResponse(string paramText, byte paramScore, byte paramSubQuestionNum)
        {
            _Text = paramText;
            _Score = paramScore;
            _subQuestionIndex = paramSubQuestionNum;
        }

        private int _subQuestionIndex;  //index within its question/hash grouping 0 based
        public int index
        {
            get { return _subQuestionIndex; }
        }


        protected byte _Score;
        public byte Score
        {
            get { return _Score; }
            set 
            { 
                _Score = value;
                RaisePropertyChanged("Score");
                decision = Decision.decided; 
            }
        }

        protected string _Text;
        public string Text 
        { 
            get { return _Text; }
            set
            {
                _Text = value;
                RaisePropertyChanged("Text");
            }
        }

        string IToString<SubResponse>.toString() //does not need index
        {
            return _Text + DELIMITER + _Score;
        }

        SubResponse IToString<SubResponse>.fromString(string str, int paramSubQuestionIndex)
        {
            string[] splitString = str.Split(new[] { DELIMITER }, StringSplitOptions.None);
            _Text = splitString[0];
            _Score = byte.Parse(splitString[1]);
            _subQuestionIndex = paramSubQuestionIndex;
            
            return string.IsNullOrEmpty(_Text) ? null : this;  //does not need index
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
