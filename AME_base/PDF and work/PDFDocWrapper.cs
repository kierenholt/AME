using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;


namespace AME_base
{
    public class PDFDocWrapper
    {
        public byte[] bytes;
        //public MemoryStream _stream;
        private PdfWriter _writer;
        public Document document;
        public Dictionary<string, PdfFormField> radioGroups { get; private set; }
        public List<Action<PdfWriter, Rectangle>> pageActions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaString"></param>
        /// <param name="_stream"></param>
        /// <param name="htmls"></param>
        /// <param name="_includeSolutions"></param>
        /// <param name="studentNameAddedToPrint"></param>
        public static void PDFToStream(string metaString, 
            Stream _stream, 
            IEnumerable<HTML> htmls, 
            bool _includeSolutions,
            string studentNameAddedToPrint)
        {
            int errorCount = 0;
            errorCount = htmls.Count(h => h.hardErrors.Count > 0);
            if ((errorCount > 0) &&
                MessageBox.Show("some rows had errors in them and will not show up in the PDF - do you wish to continue?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;


            using (Document document = new Document())
            {
                using (PdfWriter _writer = PdfWriter.GetInstance(document, _stream))
                {
                    List<Action<PdfWriter, Rectangle>> pageActions = new List<Action<PdfWriter, Rectangle>>();
                    _writer.PageEvent = new AllFormsPageEvent(pageActions);

                    Dictionary<string, PdfFormField>  radioGroups = new Dictionary<string, PdfFormField>();

                    document.Open();
                    document.AddAuthor(metaString); //MUST HAPPEN AFTER DOC OPEN


                    //optional studentNameAddedToPrint
                    if (!string.IsNullOrWhiteSpace(studentNameAddedToPrint))
                    {
                        TitleNode title = new TitleNode(studentNameAddedToPrint);
                        document.Add(title.element);
                    }


                    PdfPTable questionsTable = new PdfPTable(2);



                    questionsTable.SplitLate = true; //splits row that is too large
                    questionsTable.SplitRows = false; //allows rows to split

                    string previousTitle = string.Empty;

                    foreach (HTML html in htmls.Where(h => h.hardErrors.Count == 0))
                    {
                        if (html.title != previousTitle)
                        {
                            PdfPCell titleCell = html.titleCellNode().cell();
                            titleCell.Border = PdfPCell.NO_BORDER;
                            titleCell.Padding = 10f;
                            questionsTable.AddCell(titleCell);
                            previousTitle = html.title;
                        }
                        foreach (CellNode cell in html.cellNodes)
                        {
                            //add actions for input fields
                            foreach (FieldNode node in cell.descendants.OfType<FieldNode>())
                            {
                                node.actionId = pageActions.Count.ToString();
                                if (node is RadioNode)
                                {
                                    PdfFormField radioGroup;
                                    if (!radioGroups.TryGetValue(node.fieldName, out radioGroup))
                                    {
                                        radioGroup = PdfFormField.CreateRadioButton(_writer, false);
                                        radioGroup.FieldName = node.fieldName;
                                        radioGroups.Add(node.fieldName, radioGroup);
                                    }
                                    pageActions.Add((node as RadioNode).radioAction(radioGroup));
                                }
                                else
                                    pageActions.Add(node.action);
                            }
                            //add cell to document
                            PdfPCell pcell = cell.cell();
                            //pcell.Border = PdfPCell.NO_BORDER;
                            pcell.Padding = 10f;
                            questionsTable.AddCell(pcell);
                        }
                    }
                    questionsTable.CompleteRow();
                    
                    document.Add(questionsTable);

                    if (_includeSolutions)
                    {
                        document.Add(Chunk.NEXTPAGE);
                        Paragraph solutionsPara = new Paragraph("Solutions \n");
                        solutionsPara.Add(string.Join("\n", htmls.OfType<QuestionAssignmentHTML>().Select(h => h.solutionPlainText("\n"))));
                        document.Add(solutionsPara);
                    }
                    document.Close();
                } //end of using writer
            } //end of using document
        }

        public static string populateFieldsAfterWriterClose(IEnumerable<Field> paramFields, byte[] bytes, string fileName)
        {
            string path;
            using (PdfReader reader = new PdfReader(bytes))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //reader.Close();
                    //FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite, );
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        AcroFields fields = stamper.AcroFields;
                        foreach (Field f in paramFields)
                            fields.SetField(f.QuestionNum + ":" + f.subQuestionIndex, f.Value);
                        //stamper.FormFlattening = true;
                        stamper.Close();
                    }
                    path = saveToFile(fileName, ms);
                }
                reader.Close();
            }

            return path;

        }

        public static string saveToFile(string fileNameNoDate, MemoryStream _stream)
        {
            //string _path = Path.Combine(Path.GetTempPath(),
            //    DateTime.Now.ToString("yy-MM-dd ") +
            //    Helpers.getSafeFilename(fileNameNoDate) +
            //    ".pdf");

            //while (File.Exists(_path)) //ALWAYS GENERATE RANDOM FILENAME TO AVOID THREAD COLLISIONS
            string _path = Path.Combine(
                Path.GetTempPath(),
                DateTime.Now.ToString("yy-MM-dd ") +
                Helpers.getSafeFilename(fileNameNoDate) +
                ThreadSafeRandom.create().Next(99999).ToString() +
                ".pdf"
                );

            byte[] bytes = _stream.ToArray();
            if ((bytes.Length > 5 * 1024 * 1024) && MessageBox.Show("This PDF is larger than 5MB. Some iphones may not be able to return it via email. Do you wish to continue?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return null;

            System.IO.File.WriteAllBytes(_path, bytes );
            return _path;
        }

      
    }
}
