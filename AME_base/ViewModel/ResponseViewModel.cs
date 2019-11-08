using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
 
using AME_base;
using System.ComponentModel;

namespace AME_base
{
    public class ResponseViewModel: INotifyPropertyChanged
    {
        public Response _entity { get { return responseHTML.response; } }
        public ResponseHTML responseHTML { get; private set; }

        public ResponseViewModel(ResponseHTML r) 
        {
            responseHTML = r;
        }



        public string previewHTML
        {
            get { return responseHTML.previewHTML; }
        }


        private ObservableCollection<SubResponseViewModel> _subResponses;
        public ObservableCollection<SubResponseViewModel> subResponses
        {
            get 
            {
                if (_subResponses == null)
                {
                    _subResponses = new ObservableCollection<SubResponseViewModel>(_entity.subResponses.Select(sr => new SubResponseViewModel(sr)));
                    foreach (SubResponseViewModel sr in _subResponses)
                        sr.PropertyChanged += (s, e) =>
                        {
                            _entity.updateScore(); 
                            RaisePropertyChanged("score");
                        };
                    //publisher = subresponse, subscriber = response
                    //publisher has ref to target

                    //OTHER WAY ROUND
                    //publisher = addin, subscriber = grid

                }
                return _subResponses;
            }
        }

        public static List<LegendItem> legend
        {
            get
            {
                List<LegendItem> retVal = new List<LegendItem>();
                //foreach (KeyValuePair<QuestionElement.Decision, string> kvp in PDFQuestionViewModel.decisionImages)
                //    retVal.Add(new LegendItem() { descriptor = QuestionElement.DecisionAsString[(int)kvp.Key], icon = kvp.Value });
                return retVal;
            }
        }

        
        public int score
        {
            get
            {
                return _entity.Score;
            }
            set
            {
                _entity.Score = (byte)value;
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
