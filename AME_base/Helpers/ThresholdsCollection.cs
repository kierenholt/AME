using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AME_base
{
    //NOT AN ENTITY
    public class Threshold
    {
        private readonly static char MINOR_DELIMITER = (char)15;

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        private decimal _value;
        public decimal value
        {
            get { return _value; }
            set 
            {
                if (_value > 0 && value > 0) //do not delete zero threshold and do not set any other with value zero
                    _value = value;
            }
        }
        private char _gradeLetterOrNumber;
        public char gradeLetterOrNumber
        {
            get { return _gradeLetterOrNumber; }
            set { _gradeLetterOrNumber = value; }
        }
        private bool _isAbsolute;
        public bool isAbsolute
        {
            set { _isAbsolute = value; }
        }

        //default values
        public Threshold(
            string paramDescription = "description", 
            decimal paramValue = 80, 
            char paramGradeLetterOrNumber = 'A', 
            bool paramIsAbsolute = true) 
        {
            _description = paramDescription;
            _value = paramValue ;
            _gradeLetterOrNumber = paramGradeLetterOrNumber;
            _isAbsolute = paramIsAbsolute ;
        }

        public Threshold(bool paramIsAbsolute, string delimitedString)
        {
            string[] splitString = delimitedString.Split(new [] {MINOR_DELIMITER}, StringSplitOptions.None);
            _description = splitString[0];
            _value = decimal.Parse(splitString[1]);
            _gradeLetterOrNumber = splitString[2][0];
            _isAbsolute = paramIsAbsolute;
        }

        public string toString()
        {
            return _description + MINOR_DELIMITER + _value + MINOR_DELIMITER + _gradeLetterOrNumber; 
        }


        //ignores -1
        public decimal percentileOrValue(IEnumerable<decimal> source)
        {
            decimal retVal;
            if (_isAbsolute)
            {
                retVal = value;
            }
            else
            {
                if (source.Count() < 2)
                {
                    throw new InvalidOperationException("Cannot compute median for an empty set.");
                }

                var sortedList = from number in source
                                 where number != -1
                                 orderby number
                                 select number;

                decimal index = sortedList.Count() * value / 100;
                int truncatedIndex = (int)Math.Truncate(index);
                decimal fractionalExtra = index - truncatedIndex;
                decimal[] chosenPair = sortedList.Skip(truncatedIndex).Take(2).ToArray();
                retVal = chosenPair[0] + fractionalExtra * (chosenPair[1] - chosenPair[0]);
            }
            return retVal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Thresholds: List<Threshold>
    {
        private readonly static char MAJOR_DELIMITER = (char)16;

        public Thresholds(bool paramIsAbsolute = true, string delimitedString = "") : base()
        {
            if (string.IsNullOrEmpty(delimitedString))
            {
                Add(new Threshold( "LOWEST 25%", 0, 'D', true ));
                Add(new Threshold( "BELOW MEDIAN", 25, 'C', true ));
                Add(new Threshold( "ABOVE MEDIAN", 50, 'B', true ));
                Add(new Threshold( "UPPER 25%", 75, 'A', true ));
            }
            else
            {
                foreach (var s in delimitedString.Split(new [] {MAJOR_DELIMITER}, StringSplitOptions.None))
                    Add(new Threshold(paramIsAbsolute, s));
            }
        }

        public new bool Remove(Threshold t)
        {
            if (IndexOf(t) != 0) //do not delete first threshold
            {
                bool retVal = base.Remove(t);
                return retVal;
            }
            return false;
        }

        public string toString()
        {
            return this.Select(i => i.toString()).Aggregate((first, second) => first + MAJOR_DELIMITER + second);
        }

        public bool isAbsolute 
        { 
            set
            {
                foreach (Threshold t in this)
                    t.isAbsolute = value;
            }
        }

        public char PercentToGrade(decimal percent)
        {
            return (from threshold in this
                    orderby threshold.value descending
                    select threshold).First(t => percent >= t.value).gradeLetterOrNumber;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
