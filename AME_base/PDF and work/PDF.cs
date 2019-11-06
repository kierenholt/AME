
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{

    public class PDF
    {
        //OPEN FILLED PDF FROM BYTEARRAY I.E FROM EMAIL
        //https://msdn.microsoft.com/en-us/library/office/hh290849.aspx

        public PdfReader reader { get; private set; }

        public PDF(byte[] bytes)
        {
            reader = new PdfReader(bytes);
        }

        public T getWork<T>()
        {
            if (reader != null)
            {
                string metaString = string.Empty;
                reader.Info.TryGetValue("Author", out metaString);
                if (!string.IsNullOrEmpty(metaString))
                {
                    return SimplerAES.fromEncryptedString<T>(metaString);
                }
            }
            return default(T);
        }

        public bool isFlattened
        {
            get
            {
                return reader.Info.ContainsKey("Author") && !reader.AcroFields.Fields.Any();
            }
        }

        public IEnumerable<Field> fields
        {
            get
            {
                //GET PDF FIELDS AS SUBRESPONSES
                List<Field> dumpedFields = new List<Field>();
                foreach (string fieldName in reader.AcroFields.Fields.Keys)
                {
                    Regex rgx = new Regex("^Q([0-9]+):([0-9]+)");
                    MatchCollection matches = rgx.Matches(fieldName);

                    string value = reader.AcroFields.GetField(fieldName);
                    if (matches.Count == 1 && !string.IsNullOrWhiteSpace(value))
                        dumpedFields.Add(new Field()
                        {
                            subQuestionIndex = Int32.Parse(matches[0].Groups[2].Value),
                            QuestionNum = byte.Parse(matches[0].Groups[1].Value),
                            Value = value
                        });
                }

                return dumpedFields; //cannot be empty or whitespace
            }
        }
        
    }
    
    public struct Field
    {
        public int subQuestionIndex;
        public byte QuestionNum;
        public string Value;
    }
}
