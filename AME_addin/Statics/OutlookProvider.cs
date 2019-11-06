using AME_addin.Providers;
using AME_base;
using Microsoft.Office.Interop.Outlook;
 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AME_addin
{
    public static class OutlookProvider
    {

        #region MAPIfolders
        public static MAPIFolder AssignedWorkSubfolder(string SeatGroupName)
        {
            MAPIFolder assignments = getSubFolder(OutlookProvider.sentItems, "assigned");
            return getSubFolder(assignments, SeatGroupName);
        }

        public static MAPIFolder MarkedWorkSubfolder(string SeatGroupName)
        {
            MAPIFolder assignments = getSubFolder(OutlookProvider.sentItems, "marked");
            return getSubFolder(assignments, SeatGroupName);
        }

        private static MAPIFolder getSubFolder(MAPIFolder parentFolder, string subFolderName)
        {
            MAPIFolder subfolder = null;
            foreach (MAPIFolder f in parentFolder.Folders)
                if (f.Name == subFolderName)
                    subfolder = f;
            if (subfolder == null)
            {
                MessageBox.Show(string.Format("A new folder has been created in your {1} called {0}.", subFolderName, parentFolder.Name), "", MessageBoxButton.OK);
                subfolder = parentFolder.Folders.Add(subFolderName);
            }
            return subfolder;
        }

        #endregion MAPIfolders


        #region recipients

        public static List<OutlookSeat> getSeats(Recipient recip) { return getSeats(recip.AddressEntry); }

        public static List<OutlookSeat> getSeats(AddressEntry addressEntry)
        {
            //https://msdn.microsoft.com/en-us/library/office/ff184624.aspx
            if (addressEntry.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry ||
                addressEntry.AddressEntryUserType == OlAddressEntryUserType.olExchangeRemoteUserAddressEntry ||
                addressEntry.AddressEntryUserType == OlAddressEntryUserType.olSmtpAddressEntry)
            {
                return new List<OutlookSeat>() { new OutlookSeat(addressEntry) };
            }

            if (addressEntry.AddressEntryUserType == OlAddressEntryUserType.olExchangeDistributionListAddressEntry)
            {
                List<OutlookSeat> list = new List<OutlookSeat>();
                foreach (AddressEntry member in addressEntry.GetExchangeDistributionList().GetExchangeDistributionListMembers())
                {
                    list.AddRange(getSeats(member));
                }
                return list;
            }

            if (addressEntry.AddressEntryUserType == OlAddressEntryUserType.olOutlookDistributionListAddressEntry)
            {
                var colContacts = Globals.ThisAddIn.Application.Session.GetDefaultFolder(OlDefaultFolders.olFolderContacts).Items;

                List<OutlookSeat> list = new List<OutlookSeat>();
                foreach (var DL in colContacts)
                {
                    if (DL is DistListItem && (DL as DistListItem).DLName == addressEntry.Name)
                    {
                        DistListItem DLItem = (DL as DistListItem);
                        for (int i = 1; i <= DLItem.MemberCount; i++) //1-based index
                        {
                            Recipient r = DLItem.GetMember(i); //! do not get the addressentrytype it throws null exception
                            list.Add(new OutlookSeat(r.AddressEntry));
                        }
                    }
                }
                return list;
            }

            //none of the above
            return new List<OutlookSeat>();
            //email = recip.PropertyAccessor.GetProperty(@"http://schemas.microsoft.com/mapi/proptag/0x39FE001E") as string;
        }

        public static List<OutlookSeat> getSeats(Recipients recips)
        {
            List<Recipient> retVal = new List<Recipient>();
            foreach (Recipient r in recips)
                retVal.Add(r);
            return getSeats(retVal);
        }

        public static List<OutlookSeat> getSeats(List<Recipient> recips)
        {
            List<OutlookSeat> emails = new List<OutlookSeat>();
            foreach (Recipient r in recips)
                emails.AddRange(getSeats(r.AddressEntry));
            return emails;
        }

        #endregion static methods


        #region mailitems

        public static IEnumerable<MailItem> selectedMailItems
        {
            get
            {
                return Globals.ThisAddIn.Application.ActiveExplorer().Selection.OfType<MailItem>();
            }
        }

        public static List<byte[]> getAllPDFAttachments(MailItem email)
        {
            List<byte[]> retVal = new List<byte[]>();
            foreach (Attachment a in email.Attachments)
            {
                string ext = (string)a.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x3703001E"); //".pdf"
                if (ext == ".pdf")
                {
                    byte[] attachmentData = (byte[])a.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x37010102");
                    if (attachmentData != null)
                        retVal.Add(attachmentData);
                }
            }
            return retVal;
        }

        public static bool getMarkedWorkProperty(MailItem email)
        {
            UserProperty prop = email.UserProperties.Find("isMarked", true);
            return prop == null ? false : (bool)email.UserProperties["isMarked"].Value;
        }

        public static void setMarkedWorkProperty(MailItem _email)
        {
            _email.UserProperties.Add("isMarked", OlUserPropertyType.olYesNo, false, false);
            _email.UserProperties["isMarked"].Value = true;
        }
        #endregion 

        //private static IEnumerable<AddressList> _addressLists;

        //private static NameSpace _nameSpace; CHANGES DEPENDING ON WHICH MAILBOX YOU ARE IN!
        //public static NameSpace nameSpace
        //{
        //    get
        //    {
        //        return isExchange ? Globals.ThisAddIn.Application.GetNamespace("MAPI") : Globals.ThisAddIn.Application.Session;
        //    }
        //}

        
        //private static bool? _isExchange; CHANGES DEPENDING ON WHICH MAILBOX!
        //public static bool isExchange
        //{
        //    get
        //    {
        //        return Globals.ThisAddIn.Application.Session.ExchangeConnectionMode != OlExchangeConnectionMode.olNoExchange;
        //    } //800 for exchange
        //}

        //public static AddressEntries getMembers(AddressEntry _selected)
        //{
        //    return (_selected.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry) ?
        //                    _selected.GetExchangeDistributionList().GetExchangeDistributionListMembers() : 
        //                    _selected.Members;
        //}


        //public static IEnumerable<AddressList> addressLists
        //{
        //    get
        //    {
        //        if (_addressLists == null)
        //            _addressLists = from AddressList a in nameSpace.AddressLists
        //                        where a.AddressEntries.Count > 0
        //                        select a;
        //        return _addressLists;
        //    }
        //}

        //account info http://msdn.microsoft.com/en-us/library/office/ff869974(v=office.15).aspx


        

        //public static MAPIFolder currentFolder
        //{
        //    get
        //    {
        //        return  Globals.ThisAddIn.Application.ActiveExplorer().CurrentFolder;
        //    }
        //}

        //public static IEnumerable<AppointmentItem> selectedApptItems
        //{
        //    get
        //    {
        //        return Globals.ThisAddIn.Application.ActiveExplorer().Selection.OfType<AppointmentItem>();
        //    }
        //}


        public static string renameAttachment(Attachment att, string newBaseNameNoExtension)
        {
            string newName = System.IO.Path.Combine(
                        System.IO.Path.GetTempPath(), 
                        Helpers.getSafeFilename(newBaseNameNoExtension) + System.IO.Path.GetExtension(att.FileName)
                        );
            int i = 1;
            while (System.IO.File.Exists(newName))
            {
                newName = System.IO.Path.Combine(
                        System.IO.Path.GetTempPath(),
                        Helpers.getSafeFilename(newBaseNameNoExtension) + "(" + i.ToString() + ")" + System.IO.Path.GetExtension(att.FileName)
                        ); 
                i++;
            }
            att.SaveAsFile(newName);
            return newName;
        }

        //account info http://msdn.microsoft.com/en-us/library/office/ff869974(v=office.15).aspx
        //private static AddressEntry _currentUser;
        //public static AddressEntry currentUser
        //{
        //    get
        //    {
        //        if (_currentUser == null) _currentUser = nameSpace.CurrentUser.AddressEntry;
        //        return _currentUser; //
        //    }
        //}


        public static string currentUserEmailAddress
        {
            get
            {
                return new OutlookSeat(Globals.ThisAddIn.Application.Session.CurrentUser.AddressEntry).Email;
            }
        }

        public static bool isGroup(AddressEntry ae)
        {
            return (ae.DisplayType == OlDisplayType.olPrivateDistList || ae.DisplayType == OlDisplayType.olDistList); //SMTP or EXCHANGE DIST LIST
        }
        
        public static MAPIFolder inbox { get { return (MAPIFolder)Globals.ThisAddIn.Application.ActiveExplorer().Session.GetDefaultFolder(OlDefaultFolders.olFolderInbox); } }
        public static MAPIFolder sentItems { get { return (MAPIFolder)Globals.ThisAddIn.Application.ActiveExplorer().Session.GetDefaultFolder(OlDefaultFolders.olFolderSentMail); } }
        public static MAPIFolder deletedItems { get { return (MAPIFolder)Globals.ThisAddIn.Application.ActiveExplorer().Session.GetDefaultFolder(OlDefaultFolders.olFolderDeletedItems ); } }
        //public static Items contactItems { get { return nameSpace.GetDefaultFolder(OlDefaultFolders.olFolderContacts).Items; } }
    }
}
