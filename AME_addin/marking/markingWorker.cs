using AME_addin.Providers;
using AME_base;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AME_addin
{
    public static class markingWorker
    {
        public static void mark(IEnumerable<MailItem> mailItems)
        {
            foreach (MailItem mail in mailItems)
            {
#if DEBUG
                InputBox box = new InputBox("enter sender email address");
                box.ShowDialog();
                string senderEmail = box.text;
#else
                string senderEmail = OutlookProvider.getSeats(mail.Sender).FirstOrDefault().Email;
#endif
                Func<IEnumerable<Seat>, Seat> studentPickerMethod = (IEnumerable<Seat> paramSeats) =>
                {
                    //student picker method

                    SeatGivenAssignmentPickerViewModel svm = new SeatGivenAssignmentPickerViewModel(paramSeats, senderEmail);
                    SeatGivenAssignmentPickerWindow svmw = new SeatGivenAssignmentPickerWindow(svm);
                    svmw.ShowDialog();

                    if (!svm.cancelmarking)
                    {
                        if (svm.addToAlternateEmail)
                            svm.selectedSeatVM._entity.AlternateEmail = senderEmail;
                        return svm.selectedSeatVM._entity;
                    }
                    return null;
                };


                List<byte[]> bytes = OutlookProvider.getAllPDFAttachments(mail);

                IEnumerable<PDF> PDFs = bytes.Select(b => new PDF(b)).Where(c => c.reader != null);

                List<CompletedWork> works = new List<CompletedWork>();
                foreach (PDF pdf in PDFs)
                {
                    if (pdf.isFlattened)
                    {
                        if (MessageBox.Show("This PDF has been flattened and cannot be marked. Would you like to send an automated email explaining this to the student?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            MailItem replyEmail = mail.Reply();
                            replyEmail.HTMLBody = "<p>The PDF you just sent could not be marked because you chose the 'send flattened' option when saving it. Please choose 'send original' next time and then I should be able to  mark it. Thanks.</p><br>";
                            replyEmail.Send();
                        }
                        continue;
                    }

                    AssignedWork aw = pdf.getWork<AssignedWork>();
                    if (aw != null)
                    {
                        works.Add(new CompletedWork(aw,
                            pdf.fields,
                            Globals.ThisAddIn.activeDb,
                            OutlookProvider.currentUserEmailAddress,
                            studentPickerMethod,
                            senderEmail,
                            mail.ReceivedTime));
                    }

                    AuthoredWork auth = pdf.getWork<AuthoredWork>();
                    if (auth != null)
                        MessageBox.Show("This pdf was sent without creating a record in the database. Maybe when AME was turned off.");
                    
                }

                //if not a single pdf attachment could actually be marked then...
                if (!works.Any())
                    System.Windows.MessageBox.Show("Could not find a filled PDF within the email attachments", "?", MessageBoxButton.OK);

                foreach (CompletedWork work in works)
                {
                    if (!string.IsNullOrEmpty(work.errorMessage))
                    {
                        System.Windows.MessageBox.Show(work.errorMessage);
                        continue;
                    }
                    if (work.earMarkTest == CompletedWork.EarMarkResult.fraud &&
                        System.Windows.MessageBox.Show(string.Format(@"{0} {1} has not sent back the original pdf, but one forwarded from another student. Would you like to continue marking",
                            work.sender.FirstName,
                            work.sender.LastName), "!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        continue;
                    //end marking, send email back saying not his work


                    if (!Globals.Ribbons.Ribbon1.automatedMarkSend)
                    {
                        CompletedWorkViewModel vm = new CompletedWorkViewModel(work,
                            () =>
                            {
                                Globals.ThisAddIn.activeDb.SaveChanges();

                                //UPDATE GRIDS
                                Globals.ThisAddIn.fireResponsesMarked(work.changedResponses);

                                //FEEDBACK EMAIL
                                MailItem replyEmail = (MailItem)Globals.ThisAddIn.Application.CreateItem(OlItemType.olMailItem);//mail.Reply;
                                replyEmail.Subject = "feedback for " + work.assignment.Name;
                                replyEmail.HTMLBody = work.attemptHTML.feedbackEmailBody;
                                replyEmail.To = work.sender.Email;

                                string PDFFile = work.PDFTempFile();
                                if (!string.IsNullOrEmpty(PDFFile))
                                    replyEmail.Attachments.Add(PDFFile);
                                replyEmail.DeleteAfterSubmit = true;

                                OutlookProvider.setMarkedWorkProperty(replyEmail);

                                replyEmail.Display();

                                if (Properties.Settings.Default.deleteMarkedEmails)
                                    mail.Delete();
                            }

                            );
                        MarkAssignmentWindow maw = new MarkAssignmentWindow(vm);
                        maw.Show();
                    }
                    else //auto mark
                    {
                        Globals.ThisAddIn.activeDb.SaveChanges();
                        //UPDATE GRIDS
                        Globals.ThisAddIn.fireResponsesMarked(work.changedResponses);

                        MailItem replyEmail = (MailItem)Globals.ThisAddIn.Application.CreateItem(OlItemType.olMailItem);//mail.Reply;
                        replyEmail.Subject = "feedback on " + work.assignment.Name;
                        replyEmail.HTMLBody = work.attemptHTML.feedbackEmailBody;
                        replyEmail.To = work.sender.Email;

                        string PDFFile = work.PDFTempFile();
                        if (!string.IsNullOrEmpty(PDFFile))
                            replyEmail.Attachments.Add(PDFFile);
                        replyEmail.DeleteAfterSubmit = true;

                        OutlookProvider.setMarkedWorkProperty(replyEmail);

#if DEBUG
                        replyEmail.Display();
#else
                        replyEmail.Send();
#endif

                        if (Properties.Settings.Default.deleteMarkedEmails)
                            mail.Delete();
                    }

                }

            }
        }
    }
}
