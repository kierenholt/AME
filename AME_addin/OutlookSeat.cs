using AME_base;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_addin
{
    public class OutlookSeat : ISeat
    {
        

        public OutlookSeat(AddressEntry addressEntry)
        {

            if (addressEntry.Type != "SMTP" && (
                addressEntry.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry ||
                addressEntry.AddressEntryUserType == OlAddressEntryUserType.olExchangeRemoteUserAddressEntry))
            {
                var exchangeUser = addressEntry.GetExchangeUser();
                if (exchangeUser != null)
                {
                    FirstName = exchangeUser.FirstName;
                    LastName = exchangeUser.LastName;
                    Email = exchangeUser.PrimarySmtpAddress.ToLower();
                }
                else
                {
                    FirstName = addressEntry.GetContact().FirstName;
                    LastName = addressEntry.GetContact().LastName;
                    Email = addressEntry.GetExchangeUser().PrimarySmtpAddress.ToLower();
                }
            }
            else
            {
                //if (addressEntry.AddressEntryUserType == OlAddressEntryUserType.olSmtpAddressEntry)
                //{
                //    string firstName = string.Empty;
                //    string lastName = string.Empty;
                //    if (addressEntry.GetContact() != null)
                //    {
                //        FirstName = addressEntry.GetContact().FirstName;
                //        LastName = addressEntry.GetContact().LastName;
                //    }
                //    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                //    {
                //        string[] splitName = addressEntry.Address.ToLower().Split(new[] { '@' });
                //        FirstName = splitName.Count() > 0 ? splitName[0] : string.Empty;
                //        LastName = splitName.Count() > 1 ? splitName[1] : string.Empty;
                //    }
                //    Email = addressEntry.Address.ToLower();
                //}
                //SMTP type
                //else
                {
                    FirstName = addressEntry.Address;
                    Email = addressEntry.Address;
                }
            }
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AlternateEmail { get; set; }
    }
}
