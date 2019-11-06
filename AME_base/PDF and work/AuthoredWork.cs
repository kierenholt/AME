
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace AME_base
{
    [Serializable]
    public class AuthoredWork
    {
        public List<SerializableRow> _repeatedRows;
        public bool includeSolutions { get; set; }
        
        public AuthoredWork(
            IEnumerable<ISerializableRow> paramRepeatedRows,
            bool paramIncludeSolutions)
        {
            includeSolutions = paramIncludeSolutions;
            _repeatedRows = paramRepeatedRows.Select(r => r.toSerializableRow()).ToList(); //these get serialised
        }


        [NonSerialized]
        private AssignmentHTML _assignmentHTML;
        public AssignmentHTML assignmentHTML
        {
            get
            {
                if (_assignmentHTML == null)
                {
                    Assignment dummy = new Assignment().init(_repeatedRows.Select(r => r.toRowOrQuestion()),
                        new Group(),
                        0,
                        string.Empty,
                        false,
                        0);
                    _assignmentHTML = new AssignmentHTML(dummy);
                }
                return _assignmentHTML;
            }
        }

        public string PDFTempFile(string studentNameAddedToPrint)
        {
            return assignmentHTML.AssignmentPDFTempFile(SimplerAES.asEncryptedString(this), includeSolutions, studentNameAddedToPrint);
        }


        public string HTMLTempFile()
        {
            return assignmentHTML.AssignmentHTMLTempFile();
        }
    }
}
