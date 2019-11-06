using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Office.Interop.Outlook;
using System.IO;

namespace AME_addin
{
    public static class License
    {
        static string[] DOMAINS = { "tiffin.kingston.sch.uk" }; //USER EMAIL ADDRESS MUST END WITH
        static string[] USERS = { "kierenholt@gmail.com" }; //USER EMAIL ADDRESS MUST EQUAL

        public static bool isLicensed
        {
            get
            {
                return DOMAINS.Any(d => OutlookProvider.currentUserEmailAddress.ToLower().EndsWith(d.ToLower())) 
                    
                    || USERS.Any(d => string.Equals(d,OutlookProvider.currentUserEmailAddress,StringComparison.OrdinalIgnoreCase) );

                
             }
        }
    }
}
