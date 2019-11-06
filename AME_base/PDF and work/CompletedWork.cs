using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;

namespace AME_base
{
    
    public class CompletedWork // :avoid inherit   AssignedWork
    {
        private SchoolContext _db;
        private Func<IEnumerable<Seat>, Seat> studentPickerMethod;
        private AssignedWork assignedWork;

        public CompletedWork(
            AssignedWork paramAssignedWork,
            IEnumerable<Field> paramFields, 
            SchoolContext paramDb,
            string paramTeacherUserCheck,
            Func<IEnumerable<Seat>, Seat> paramStudentPickerMethod,
            string senderEmail,
            DateTime paramWhenReturned)
        {
            if (!paramFields.Any())
                errorMessage = "PDF contained no responses";
            //if (paramAssignedWork.teacherEmailAddress != paramTeacherUserCheck)
            //    errorMessage = "Assigned from different teacher account, " + paramAssignedWork.teacherEmailAddress;

            //initialise
            studentPickerMethod = paramStudentPickerMethod;
            assignedWork = paramAssignedWork;
            _db = paramDb;

            //assignment
            if (string.IsNullOrEmpty(errorMessage))
            {
                assignment = _db.Assignments.Find(assignedWork.GroupId, assignedWork.AssignmentNum);
                if (assignment == null)
                    errorMessage = "assignment not found in database";
            }

            //group
            if (string.IsNullOrEmpty(errorMessage))
            {
                group = _db.Groups.Find(assignedWork.GroupId); //get the seats too
                if (group == null)
                    errorMessage = "group not found in database";
            }

            //attempts
            if (string.IsNullOrEmpty(errorMessage))
            {
                _allAttempts = _db.Attempts.Where(a =>
                    a.GroupID == assignedWork.GroupId &&
                    a.AssignmentNum == assignedWork.AssignmentNum &&
                    a.ShuffleSeed == assignedWork.ShuffleSeed
                    ).Include(at => at.Seat).ToList();

                //update responses from pdf and assign scores
                if (!_allAttempts.Any())
                    errorMessage = "attempt not found in database";
            }

            //sender
            if (string.IsNullOrEmpty(errorMessage))
            {
                attempt = _allAttempts.SingleOrDefault(r => r.Seat.Email == senderEmail || r.Seat.AlternateEmail == senderEmail);
                if (attempt != null)
                {
                    sender = attempt.Seat;
                }
                else
                {
                    sender = group.Seats.SingleOrDefault(s => s.Email == senderEmail || s.AlternateEmail == senderEmail) ??
                        studentPickerMethod(group.Seats);
                    if (sender == null)
                        errorMessage = "student not found in database";
                }
            }
            
            //attempt
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (attempt == null)
                {
                    attempt = _allAttempts.SingleOrDefault(s => s.GroupID == sender.GroupID && s.SeatNum == sender.SeatNum);
                    if (attempt == null)
                    {
                        attempt = _db.Attempts.Create();
                        attempt.ShuffleSeed = assignedWork.ShuffleSeed;
                        attempt.SeatNum = sender.SeatNum;
                        attempt.AssignmentNum = assignedWork.AssignmentNum;
                        attempt.GroupID = assignedWork.GroupId;
                        attempt.whenAssigned = _allAttempts.First().whenAssigned;
                        attempt.AttemptNum = ++assignment.AttemptCount;

                        _db.Attempts.Add(attempt);
                    }
                }
                attempt.UpdateResponseValuesFromPDF(paramFields, _db);
                attempt.whenReturned = paramWhenReturned;

                attemptHTML = new AttemptHTML(attempt);
                attemptHTML.CalculateAndUpdateAttemptScore();
                attempt.updateScore();
            }
            //optional user marking - changing some scores
        }
        
        public string errorMessage { get; private set; }

        public Seat sender { get; private set; }
        public Assignment assignment { get; private set;}
        private List<Attempt> _allAttempts;
        public Attempt attempt { get; private set; }
        public IEnumerable<Response> changedResponses { get { return attempt.Responses; } }

        public AttemptHTML attemptHTML { get; private set; }
        public Group group { get; private set; }


        private EarMarkResult _earMarkTest = EarMarkResult.unknown;
        public EarMarkResult earMarkTest
        {
            get
            {
                if (_earMarkTest == EarMarkResult.unknown)
                {
                    if (_allAttempts.Any(s => s.GroupID == sender.GroupID && s.SeatNum == sender.SeatNum))
                        _earMarkTest = EarMarkResult.valid;
                    else
                        if (group.Seats.Any(s => s.GroupID == sender.GroupID && s.SeatNum == sender.SeatNum))
                            _earMarkTest = EarMarkResult.fraud;
                }
                return _earMarkTest;
            }
        }


                
        public enum EarMarkResult
        {
            unknown, //unrecognised email
            fraud, //email belongs to someone else in class
            valid //sender is valid
        }


        public string PDFTempFile()
        {
            return attemptHTML.AttemptPDFTempFile(SimplerAES.asEncryptedString(assignedWork), true);
        }

        //fraud

                        //if (result == MessageBoxResult.No)
                        //{
                        //    //AssignedWork reAssigned = work.reAssign(new[] { naughtyBoy }).First();
                        //    //                            MailItem replyEmail2 = mail.Reply(); 
                        //    //                            replyEmail2.Body = @"the pdf you attached was not the original one I sent to you, but from someone else in the class. 
                        //    //                                I do not know whether this is your authentic work or a copy of someone else's. 
                        //    //                                Please complete the original pdf I sent you (see attached).";
                        //    //                            replyEmail2.Attachments.Add(reAssigned.PDFTempFile());
                        //}

    }
}
