using System;
using System.Linq;
using Office = Microsoft.Office.Core;

using Microsoft.Office.Interop.Outlook;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using AME_addin.Properties;
using AME_base;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace AME_addin
{
    public partial class ThisAddIn
    {
        Dispatcher _dispatcher;
        public Dispatcher Dispatcher { get { return _dispatcher; } }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //UNLICENSED
            if (!License.isLicensed)
            {
                foreach (Office.COMAddIn c in this.Application.COMAddIns)
                {
                    if (c.ProgId == "AME_Outlook") { c.Connect = false; }
                }
                return;
            }

            //LICENSED

            //superceded by assign button
            Application.ItemSend += new ApplicationEvents_11_ItemSendEventHandler(Application_ItemSend);
            _inboxItems = OutlookProvider.inbox.Items;
            _inboxItems.ItemAdd += new ItemsEvents_ItemAddEventHandler(inbox_ItemAdd);

            Task.Factory.StartNew(() =>
            {
                _activeDb = new SchoolContext();
            }).ContinueWith((result) =>
            {
                dbInitialised = (_activeDb != null);
            });
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            _activeDb = null;
        }

        Items _inboxItems;
        private bool _dbInitialised = false;
        public bool dbInitialised
        {
            get
            {
                return _dbInitialised;
            }
            private set
            {
                _dbInitialised = value;
                Globals.Ribbons.Ribbon1.showGroups = true;
            }
        }

        private SchoolContext _activeDb;
        public SchoolContext activeDb
        {
            get { return _activeDb ?? (_activeDb = new SchoolContext()); }
        }
        


        //item arrive
        private void inbox_ItemAdd(object Item)
        {
            if (Globals.Ribbons.Ribbon1.markOnArrival && (Item is MailItem) )
            {
                markingWorker.mark(new[] { (Item  as MailItem)});
                //event is triggered inside markworker
            }
        }

        public event ResponsesMarkedEventHandler responsesMarked;
        public void fireResponsesMarked(IEnumerable<Response> updatedResponses)
        {
            if (Globals.ThisAddIn.responsesMarked != null)
                Globals.ThisAddIn.responsesMarked(this, new ResponsesMarkedEventArgs(updatedResponses));
        }
        //after Seat emails are sent, this deals with the master email. 
        //It cannot be moved while sending.
        //masterEmail.Move(OutlookProvider.AssignedWorkSubfolder());
        //masterEmail.Close(OlInspectorClose.olDiscard);
        //Timer delayedCloseTimer;
        //bool sending = false;
        //void setDelayedCloseTimer(MailItem mi, MAPIFolder moveToFolder)
        //{
        //    if (delayedCloseTimer == null)
        //        delayedCloseTimer = new Timer();
        //    delayedCloseTimer.Interval = 100; // 0.1 second
        //    delayedCloseTimer.Tick += (sender, e) =>
        //        {
        //            if (!sending && mi != null)
        //            {
        //                //rename the attachments
        //                //List<string> tempAttachments = new List<string>();
        //                //for (int a = 1; a < mi.Attachments.Count + 1; a++)
        //                //{
        //                //    tempAttachments.Add(OutlookProvider.renameAttachment(mi.Attachments[1], mi.Subject));
        //                //    mi.Attachments.Remove(1);
        //                //}
        //                //foreach (string a in tempAttachments)
        //                //    mi.Attachments.Add(a);

        //                //label the subject
        //                //mi.Subject = "***TEACHER COPY*** " + mi.Subject;

        //                //copy to new folder
        //                //if (moveToFolder != null)
        //                //    mi.Save();
        //                    //mi.Move(moveToFolder); //CRASHES

        //                //mi.Close(OlInspectorClose.olDiscard);


        //                delayedCloseTimer.Stop();
        //                mi = null;
        //                delayedCloseTimer = null;
        //            }
        //        };
        //    delayedCloseTimer.Start();
        //}

        //assign on item send
        void Application_ItemSend(object Item, ref bool Cancel)
        {
            //sending = true;
            MailItem mail = Item as MailItem;
            
            if (!Settings.Default.ItemSendEventEnabled || 
                mail == null || 
                OutlookProvider.getMarkedWorkProperty(mail))
                    return;
            
            //GET PDF
            byte[] bytes = OutlookProvider.getAllPDFAttachments(mail).FirstOrDefault();

            if (bytes == null)
                return; //just a normal email message!
                //System.Windows.MessageBox.Show("no attachments", "", MessageBoxButton.OK, MessageBoxImage.Error);

            AuthoredWork work = new PDF(bytes).getWork<AuthoredWork>();

            if (work == null)
            {
                Cancel = false;
                //System.Windows.MessageBox.Show("unable to read attached PDF ", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var e = mail.Recipients.GetEnumerator();
            var list = new List<Recipient>();
            while ( e.MoveNext() ) 
                list.Add((Recipient)e.Current);

            if (list.Count > 1)
            {
                Cancel = System.Windows.MessageBox.Show("you can only assign work to a single recipient. Would you like to send this email anyway? Any returned pdfs will NOT be markable.", "!", MessageBoxButton.YesNo) == MessageBoxResult.No;
                return;
            }
            if (!OutlookProvider.isGroup(list[0].AddressEntry))
            {
                Cancel = System.Windows.MessageBox.Show("you can only assign work to a distribution list. Would you like to send this email anyway? Any returned pdfs will NOT be markable.", "!", MessageBoxButton.YesNo) == MessageBoxResult.No;
                return;
            }
            IEnumerable<OutlookSeat> recipients = OutlookProvider.getSeats(mail.Recipients);


            string groupName = mail.To.Split(new[] { ';' })[0];
            Group group =  activeDb.getOrCreateGroup(groupName);
            IEnumerable<Seat> seats = activeDb.getOrCreateOutlookSeats(recipients, group);

            if (group == null)
            {
                Cancel = System.Windows.MessageBox.Show("Error creating class. Would you like to send this email anyway? Any returned pdfs will NOT be markable.", "", MessageBoxButton.YesNo) == MessageBoxResult.No;
                return;
            }
            if (!work.assignmentHTML.QuestionAssignmentHTMLs.Any())
            {
                Cancel = System.Windows.MessageBox.Show("This assignment does not contain any questions and will not be saved in the markbook. Would you like to send this email anyway? Any returned pdfs will NOT be markable.", "", MessageBoxButton.YesNo) == MessageBoxResult.No;
                return;
            }

            MAPIFolder assignedWorkFolder = OutlookProvider.AssignedWorkSubfolder(groupName);

            var tasks = new List<Task>();
            //EMAILS PARALLEL!
            foreach (Grouping<Attempt, Seat> attemptGroup in activeDb.getNewAttemptsFromAuthoredWork(
                group, 
                mail.Subject, 
                Properties.Settings.Default.shuffleQuestionsEnabled,
                Properties.Settings.Default.markLimitValue,
                seats,
                work._repeatedRows))
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    AssignedWork assignedWork = new AssignedWork(
                        attemptGroup.Key.GroupID,
                        attemptGroup.Key.AssignmentNum,
                        attemptGroup.Key.ShuffleSeed,
                        OutlookProvider.currentUserEmailAddress
                        );

                    AttemptHTML html = new AttemptHTML(attemptGroup.Key);
                    string fileToAttach = html.AttemptPDFTempFile(SimplerAES.asEncryptedString(assignedWork), false);

                    MailItem emailForSeat;
                    emailForSeat = (MailItem)this.Application.CreateItem(OlItemType.olMailItem);
                    emailForSeat.SaveSentMessageFolder = assignedWorkFolder ;

                    if (!string.IsNullOrEmpty(fileToAttach))
                        emailForSeat.Attachments.Add(fileToAttach);

                    emailForSeat.To = string.Empty;
                    emailForSeat.BCC = string.Join(";", attemptGroup.Select(s => s.Email));
                    emailForSeat.Subject = mail.Subject;
                    emailForSeat.HTMLBody = mail.HTMLBody + Settings.Default.AssignEmailFooter;
                    emailForSeat.Send();
                }));
            } //end foreach

            Task.WaitAll(tasks.ToArray());

            Cancel = true;
            //sending = false;
            activeDb.SaveChanges();
            System.Windows.MessageBox.Show("Emails have been sent to students. You can close this window.", "", MessageBoxButton.OK);


        }



        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
