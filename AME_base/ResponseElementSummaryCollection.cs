using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AME_base
{
    public class ResponseElementSummaries : ObservableCollection<ResponseElementSummary>
    {
        private IEnumerable<Response> _responses;
        private List<IEnumerable<SubResponse>> _transposeResponseElements;

        public ResponseElementSummaries(IEnumerable<Response> paramResponses)
        {
            _responses = paramResponses;

            //int ResponseElementCount = paramResponses[0].SubResponses.Count;

            //_transposeResponseElements = new List<CovariantObservableCollection<SubResponse>>();
            //for (int re = 0; re < ResponseElementCount; re++)
            //{
            //    _transposeResponseElements.Add(new CovariantObservableCollection<SubResponse>());
            //    for (int r = 0; r < paramResponses.Count(); r++)
            //        _transposeResponseElements[re].Add(paramResponses[r].SubResponses[re]);

            //    Add(new ResponseElementSummary(_transposeResponseElements[re]));
            //}

            //when a new response is added, update the transpose responses
            //_responses.CollectionChanged += (sender, e) =>
            //    {
            //        if (e.NewItems != null)
            //        {
            //            Response r = e.NewItems[0] as Response;

            //            for (int re = 0; re < r.SubResponses.Count; re++)
            //                _transposeResponseElements[re].Add(r.SubResponses[re]);
            //        }
            //    };
        }

    }

    //question > response > ResponseElements
    public class ResponseElementSummary : ObservableCollection<ResponseElementSummaryItem>
    {
        public ResponseElementSummary(ObservableCollection<SubResponse> ResponseElements) :
            base((from SubResponse re in ResponseElements
                         select re.Text)
                            .Distinct()
                            .Select(g => new ResponseElementSummaryItem(g, ResponseElements))
                            .OrderByDescending(res => res.totalWithSameText))
        {
            //if a completely new text is added, then create a new summary
            ResponseElements.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    SubResponse re = e.NewItems[0] as SubResponse;
                    if (!Items.Any(res => res.text == re.Text))
                        Add(new ResponseElementSummaryItem(re.Text, ResponseElements));
                }
            };

            //ResponseElementsummary contains event to update when ResponseElements are changed
        }


    }

    public class ResponseElementSummaryItem : INotifyPropertyChanged
    {
        private IEnumerable<SubResponse> _allResponseElements;
        private string _text;
        public ResponseElementSummaryItem(string paramText, ObservableCollection<SubResponse> paramResponseElements)
        {
            _allResponseElements = paramResponseElements;
            _text = paramText;

            paramResponseElements.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged("population");
                RaisePropertyChanged("totalWithSameText");
                RaisePropertyChanged("percentWithSameText");
            };
        }

        public string text { get { return _text; } }

        public int population { get { return _allResponseElements.Count(); } }

        public int totalWithSameText { get { return _allResponseElements.Count(r => r.Text == _text); } }

        public decimal percentWithSameText { get { return totalWithSameText * 100m / population; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
