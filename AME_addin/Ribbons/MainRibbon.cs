using Microsoft.Office.Tools.Ribbon;

using System.Windows;
using AME_addin.Providers;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System;
using AME_base;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
 

namespace AME_addin
{
    public partial class MainRibbon
    {
        public bool showGroups
        {
            set
            {
                this.initialisationMessageGroup.Visible = !value;
                this.databaseGroup.Visible = value;
                this.manageAssignmentGroup.Visible = value;
                this.manageClassesGroup.Visible = value;
                this.settingsGroup.Visible = value;
            }
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            this.deleteMarkedWorktoggleButton1.Checked = Properties.Settings.Default.deleteMarkedEmails;
            showGroups = false;
        }
        
        //mark selected email
        private void splitButton1_Click(object sender, RibbonControlEventArgs e)
        {
            markingWorker.mark(OutlookProvider.selectedMailItems);
        }


        //purge database
        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            if (System.Windows.MessageBox.Show("are you sure you want to delete ALL MARKBOOK DATA?", "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.IO.File.Delete(SchoolContext.SQLCompactDBPath);
                System.Windows.MessageBox.Show(System.IO.File.Exists(SchoolContext.SQLCompactDBPath) ? "delete failed" : "delete succeeded");
                
                //Globals.ThisAddIn.activeDb.Database.Delete();
                Globals.ThisAddIn.activeDb.Database.Initialize(true);
                //Globals.ThisAddIn.activeDb.Database.Create();// not needed initialise makes it
#if DEBUG
                if (System.Windows.MessageBox.Show("seed new database?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Globals.ThisAddIn.activeDb.Seed();
#endif

                //Globals.ThisAddIn.activeDb = new SchoolContext(seed);
                //using (SchoolContext db = Globals.ThisAddIn.activeDb) 
                //{
                    
                //    db.Database.Initialize(true); //IMPORTANT! 
                //    db.Database.Delete();
                //}
            }
        }

        //toggle automated mark+send
        private void toggleButton1_Click(object sender, RibbonControlEventArgs e) {}
        public bool automatedMarkSend
        {
            get { return this.toggleMarkSendButton.Checked; }
        }
        public bool markOnArrival
        {
            get { return this.toggleMarkSendButton.Checked; }
        }
        private void deleteMarkedWorktoggleButton1_Click(object sender, RibbonControlEventArgs e)
        {
            Properties.Settings.Default.deleteMarkedEmails  = this.deleteMarkedWorktoggleButton1.Checked;
            Properties.Settings.Default.Save();
        }
        
        //view TCS/deadlines/Seats/attempts MARKBOOK
        private void button2_Click(object sender, RibbonControlEventArgs e)
        {


            if (Globals.ThisAddIn.activeDb.Groups.Any())
            {
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                    {
                        TopViewModel tvm = new TopViewModel(Globals.ThisAddIn.activeDb, (tvm2) => { Globals.ThisAddIn.responsesMarked -= tvm2.updateGrids; });
                        Globals.ThisAddIn.responsesMarked += tvm.updateGrids;
                        MarkBookWindow mbw = new MarkBookWindow(tvm);
                        Globals.ThisAddIn.Dispatcher.BeginInvoke(() => markbookButton.Enabled = true);
                        mbw.Show();
                        System.Windows.Threading.Dispatcher.Run();
                    }));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                markbookButton.Enabled = false;
                newWindowThread.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("you have not assigned any work yet", "", MessageBoxButton.OK);
            }
        }

        //view selected assignment - deadline/questions/Seats/responses DEADLINE
        private void button3_Click(object sender, RibbonControlEventArgs e)
        {
            if (Globals.ThisAddIn.activeDb.Assignments.Any())
            {
                TopViewModel tvm = new TopViewModel(Globals.ThisAddIn.activeDb, (tvm2) => { Globals.ThisAddIn.responsesMarked -= tvm2.updateGrids; });
                Globals.ThisAddIn.responsesMarked += tvm.updateGrids;
                DeadlineQuestionsWindow dqw = new DeadlineQuestionsWindow(tvm);
                dqw.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("you have not assigned any work yet", "", MessageBoxButton.OK);
            }
        }

        //settings page - file menu
        private void configButton_Click(object sender, RibbonControlEventArgs e)
        {
            UserSettingsWindow usw = new UserSettingsWindow();
            usw.Show();
        }

        //settings page - ribbon
        private void settingsButton_Click(object sender, RibbonControlEventArgs e)
        {
            UserSettingsWindow usw = new UserSettingsWindow();
            usw.Show();
        }
        
        //compact button
        private void button1_Click_1(object sender, RibbonControlEventArgs e)
        {
            SchoolContext.compact();
        }

        //export all button
        private void exportAllButton_Click(object sender, RibbonControlEventArgs e)
        {
            using (SchoolContext db = Globals.ThisAddIn.activeDb)
            {
                Dictionary<string, IGridViewModelBase> dict = new Dictionary<string, IGridViewModelBase>();
                foreach (Group g in db.Groups)
                {
                    GroupViewModel tvm = new GroupViewModel(g);
                    dict.Add(
                        g.Name,
                        tvm.grids[0]
                        );
                }
                OpenXML.createMarkSpreadsheet(dict);
            }
        }


    }
}
