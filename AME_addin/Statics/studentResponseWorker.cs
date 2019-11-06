
using AME_base;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_addin
{
    
    public class SeatResponseWorker
    {
        //opens DeadlineQuestionsWindow and hooks up events 
        public SeatResponseWorker(MailItem mi, SchoolContext paramDb)
        {
            SchoolContext _db = paramDb;

            if (mi != null)
            {
                PDF _pdfAssignment = OutlookProvider.getAssignmentsFromMailItem(mi).FirstOrDefault();
                if (_pdfAssignment != null)
                {
                    Course course = _db.getCourse(_pdfAssignment, true);
                    Assignment _assignment = _db.getAssignment(_pdfAssignment, course, false);
                    List<OutlookSeat> senderAndRecipients = new List<OutlookSeat>();
                    senderAndRecipients.AddRange(OutlookSeat.getEmailNames(mi.Recipients));
                    if (mi.Sender != null)
                        senderAndRecipients.AddRange(OutlookSeat.getEmailNames(mi.Sender));

                    if (_assignment != null)
                        foreach (OutlookSeat en in senderAndRecipients)
                        {
                            Seat _Seat = _db.getSeat(en, null, false, true);
                            if (_Seat != null)
                            {
                                _deadline = _db.DeadlinesOfSeatAssignment(_Seat, _assignment).LastOrDefault();
                                if (_deadline != null)
                                    break;
                            }
                        }
                }
            }

            DeadlineQuestionsWindow dqw = new DeadlineQuestionsWindow(_db);
            if (_deadline != null)
            {
                TopViewModel top = dqw.DataContext as TopViewModel;
                top.TCSs.selectedEntity = _deadline.TCS;
                top.TCSs.selectedViewModel.deadlines.selectedEntity = _deadline;
            }

            dqw.Closed += dqw_Closed;
            dqw.Show();
        }

        void dqw_Closed(object sender, EventArgs e)
        {
            OnFinished(EventArgs.Empty);
        }

        public event EventHandler finished;
        protected virtual void OnFinished(EventArgs args)
        {
            var handler = this.finished;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
