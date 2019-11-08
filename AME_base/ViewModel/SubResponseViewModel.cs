using AME_base;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace AME_base
{
    public class ScoreViewModel //a cell within the score selection grid
    {
        public int value { get; set; }
        public Brush color { get; set; }

        public ScoreViewModel(int paramValue, Brush paramColor)
        {
            value = paramValue;
            color = paramColor;
        }
    }

    public class SubResponseViewModel : INotifyPropertyChanged 
    {
        public SubResponseViewModel(SubResponse t) { _entity = t; }
        SubResponse _entity;

        public string text
        {
            get { return _entity.Text; }
        }

        public string score
        {
            get { return _entity.Score.ToString(); }
            set
            {
                byte trialResult;
                if (byte.TryParse(value, out trialResult))
                {
                    _entity.Score = trialResult;
                    RaisePropertyChanged("score");
                }
            }
        }


        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
        }

        public string questionLetter
        {
            get
            {
                int charCode = 'a' + _ID;
                return ((char)charCode).ToString();
            }
        }

        private ObservableCollection<ScoreViewModel> _possibleScores;
        public ObservableCollection<ScoreViewModel> possibleScores //binds with listbox
        {
            get
            {
                if (_possibleScores == null)
                {
                    _possibleScores = new ObservableCollection<ScoreViewModel>();
                    int outof = 1;
                    for (int i = 0; i <= outof; i++)
                        _possibleScores.Add(
                            new ScoreViewModel(
                                i,
                                new SolidColorBrush(new Color()
                                {
                                    R = (byte)(255 * (outof - i) / outof),
                                    G = (byte)(255 * i / outof),
                                    B = 0,
                                    A = 255
                                }))
                            );
                }
                return _possibleScores;
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
