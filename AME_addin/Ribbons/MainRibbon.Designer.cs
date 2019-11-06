namespace AME_addin
{
    partial class MainRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MainRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainRibbon));
            this.AME = this.Factory.CreateRibbonTab();
            this.manageClassesGroup = this.Factory.CreateRibbonGroup();
            this.markbookButton = this.Factory.CreateRibbonButton();
            this.manageAssignmentGroup = this.Factory.CreateRibbonGroup();
            this.markWorkButton = this.Factory.CreateRibbonSplitButton();
            this.toggleMarkSendButton = this.Factory.CreateRibbonToggleButton();
            this.deleteMarkedWorktoggleButton1 = this.Factory.CreateRibbonToggleButton();
            this.viewDeadlineButton = this.Factory.CreateRibbonButton();
            this.settingsGroup = this.Factory.CreateRibbonGroup();
            this.settingsButton = this.Factory.CreateRibbonButton();
            this.databaseGroup = this.Factory.CreateRibbonGroup();
            this.purgeButton = this.Factory.CreateRibbonButton();
            this.exportAllButton = this.Factory.CreateRibbonButton();
            this.dropDown1 = this.Factory.CreateRibbonDropDown();
            this.configButton = this.Factory.CreateRibbonButton();
            this.initialisationMessageGroup = this.Factory.CreateRibbonGroup();
            this.label1 = this.Factory.CreateRibbonLabel();
            this.AME.SuspendLayout();
            this.manageClassesGroup.SuspendLayout();
            this.manageAssignmentGroup.SuspendLayout();
            this.settingsGroup.SuspendLayout();
            this.databaseGroup.SuspendLayout();
            this.initialisationMessageGroup.SuspendLayout();
            // 
            // AME
            // 
            this.AME.Groups.Add(this.manageClassesGroup);
            this.AME.Groups.Add(this.manageAssignmentGroup);
            this.AME.Groups.Add(this.settingsGroup);
            this.AME.Groups.Add(this.databaseGroup);
            this.AME.Groups.Add(this.initialisationMessageGroup);
            this.AME.Label = "AME";
            this.AME.Name = "AME";
            // 
            // manageClassesGroup
            // 
            this.manageClassesGroup.Items.Add(this.markbookButton);
            this.manageClassesGroup.Label = "manage classes";
            this.manageClassesGroup.Name = "manageClassesGroup";
            // 
            // markbookButton
            // 
            this.markbookButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.markbookButton.Image = global::AME_addin.Properties.Resources._1483162332_LIBRARY_2;
            this.markbookButton.Label = "markbook";
            this.markbookButton.Name = "markbookButton";
            this.markbookButton.ShowImage = true;
            this.markbookButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button2_Click);
            // 
            // manageAssignmentGroup
            // 
            this.manageAssignmentGroup.Items.Add(this.markWorkButton);
            this.manageAssignmentGroup.Items.Add(this.viewDeadlineButton);
            this.manageAssignmentGroup.Label = "manage assignment";
            this.manageAssignmentGroup.Name = "manageAssignmentGroup";
            // 
            // markWorkButton
            // 
            this.markWorkButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.markWorkButton.Image = ((System.Drawing.Image)(resources.GetObject("markWorkButton.Image")));
            this.markWorkButton.Items.Add(this.toggleMarkSendButton);
            this.markWorkButton.Items.Add(this.deleteMarkedWorktoggleButton1);
            this.markWorkButton.Label = "mark";
            this.markWorkButton.Name = "markWorkButton";
            this.markWorkButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.splitButton1_Click);
            // 
            // toggleMarkSendButton
            // 
            this.toggleMarkSendButton.Label = "Auto mark+send on arrival";
            this.toggleMarkSendButton.Name = "toggleMarkSendButton";
            this.toggleMarkSendButton.ShowImage = true;
            this.toggleMarkSendButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.toggleButton1_Click);
            // 
            // deleteMarkedWorktoggleButton1
            // 
            this.deleteMarkedWorktoggleButton1.Label = "Delete marked work";
            this.deleteMarkedWorktoggleButton1.Name = "deleteMarkedWorktoggleButton1";
            this.deleteMarkedWorktoggleButton1.ShowImage = true;
            this.deleteMarkedWorktoggleButton1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.deleteMarkedWorktoggleButton1_Click);
            // 
            // viewDeadlineButton
            // 
            this.viewDeadlineButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.viewDeadlineButton.Image = global::AME_addin.Properties.Resources._1483163586_ViewType_Details;
            this.viewDeadlineButton.Label = "view scores";
            this.viewDeadlineButton.Name = "viewDeadlineButton";
            this.viewDeadlineButton.ShowImage = true;
            this.viewDeadlineButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button3_Click);
            // 
            // settingsGroup
            // 
            this.settingsGroup.Items.Add(this.settingsButton);
            this.settingsGroup.Label = "settings";
            this.settingsGroup.Name = "settingsGroup";
            // 
            // settingsButton
            // 
            this.settingsButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.Label = "settings";
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.ShowImage = true;
            this.settingsButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.settingsButton_Click);
            // 
            // databaseGroup
            // 
            this.databaseGroup.Items.Add(this.purgeButton);
            this.databaseGroup.Items.Add(this.exportAllButton);
            this.databaseGroup.Label = "database";
            this.databaseGroup.Name = "databaseGroup";
            // 
            // purgeButton
            // 
            this.purgeButton.Label = "purge";
            this.purgeButton.Name = "purgeButton";
            this.purgeButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // exportAllButton
            // 
            this.exportAllButton.Label = "export All";
            this.exportAllButton.Name = "exportAllButton";
            this.exportAllButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.exportAllButton_Click);
            // 
            // dropDown1
            // 
            this.dropDown1.Label = "below ";
            this.dropDown1.Name = "dropDown1";
            // 
            // configButton
            // 
            this.configButton.Label = "AME";
            this.configButton.Name = "configButton";
            this.configButton.ShowImage = true;
            this.configButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.configButton_Click);
            // 
            // initialisationMessageGroup
            // 
            this.initialisationMessageGroup.Items.Add(this.label1);
            this.initialisationMessageGroup.Name = "initialisationMessageGroup";
            // 
            // label1
            // 
            this.label1.Label = "Please wait while AME initialises";
            this.label1.Name = "label1";
            // 
            // MainRibbon
            // 
            this.Name = "MainRibbon";
            // 
            // MainRibbon.OfficeMenu
            // 
            this.OfficeMenu.Items.Add(this.configButton);
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.AME);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.AME.ResumeLayout(false);
            this.AME.PerformLayout();
            this.manageClassesGroup.ResumeLayout(false);
            this.manageClassesGroup.PerformLayout();
            this.manageAssignmentGroup.ResumeLayout(false);
            this.manageAssignmentGroup.PerformLayout();
            this.settingsGroup.ResumeLayout(false);
            this.settingsGroup.PerformLayout();
            this.databaseGroup.ResumeLayout(false);
            this.databaseGroup.PerformLayout();
            this.initialisationMessageGroup.ResumeLayout(false);
            this.initialisationMessageGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab AME;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup manageClassesGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup manageAssignmentGroup;
        //internal Microsoft.Office.Tools.Ribbon.RibbonButton button5;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup databaseGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton purgeButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton markWorkButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleMarkSendButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton markbookButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton viewDeadlineButton;
        //internal Microsoft.Office.Tools.Ribbon.RibbonBox box1;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown dropDown1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton configButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup settingsGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton settingsButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton exportAllButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton deleteMarkedWorktoggleButton1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup initialisationMessageGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel label1;
    }

    partial class ThisRibbonCollection
    {
        internal MainRibbon Ribbon1
        {
            get { return this.GetRibbon<MainRibbon>(); }
        }
    }
}
