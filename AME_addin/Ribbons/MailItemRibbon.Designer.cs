namespace AME_addin
{
    partial class MailItemRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MailItemRibbon()
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
            this.MailItemTab = this.Factory.CreateRibbonTab();
            this.AMEgroup = this.Factory.CreateRibbonGroup();
            this.shuffleQuestionsToggleButton = this.Factory.CreateRibbonToggleButton();
            this.includeSolutionsToggleButton = this.Factory.CreateRibbonToggleButton();
            this.markLimitTextBox = this.Factory.CreateRibbonEditBox();
            this.masterSwitchGroup = this.Factory.CreateRibbonGroup();
            this.masterSwitchToggleButton = this.Factory.CreateRibbonToggleButton();
            this.saveSettingsGroup = this.Factory.CreateRibbonGroup();
            this.saveSettings = this.Factory.CreateRibbonButton();
            this.MailItemTab.SuspendLayout();
            this.AMEgroup.SuspendLayout();
            this.masterSwitchGroup.SuspendLayout();
            this.saveSettingsGroup.SuspendLayout();
            // 
            // MailItemTab
            // 
            this.MailItemTab.Groups.Add(this.AMEgroup);
            this.MailItemTab.Groups.Add(this.masterSwitchGroup);
            this.MailItemTab.Groups.Add(this.saveSettingsGroup);
            this.MailItemTab.Label = "AME";
            this.MailItemTab.Name = "MailItemTab";
            // 
            // AMEgroup
            // 
            this.AMEgroup.Items.Add(this.shuffleQuestionsToggleButton);
            this.AMEgroup.Items.Add(this.includeSolutionsToggleButton);
            this.AMEgroup.Items.Add(this.markLimitTextBox);
            this.AMEgroup.Label = "PDF Options";
            this.AMEgroup.Name = "AMEgroup";
            this.AMEgroup.Visible = false;
            // 
            // shuffleQuestionsToggleButton
            // 
            this.shuffleQuestionsToggleButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.shuffleQuestionsToggleButton.Image = global::AME_addin.Properties.Resources._1483162236_icon_shuffle;
            this.shuffleQuestionsToggleButton.Label = "Shuffle Questions";
            this.shuffleQuestionsToggleButton.Name = "shuffleQuestionsToggleButton";
            this.shuffleQuestionsToggleButton.ScreenTip = "If this box is checked, questions will be shuffled when assigned to Seats.";
            this.shuffleQuestionsToggleButton.ShowImage = true;
            this.shuffleQuestionsToggleButton.SuperTip = "If this box is checked, questions will be shuffled when assigned to Seats.";
            this.shuffleQuestionsToggleButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.shuffleQuestionsToggleButton_Click);
            // 
            // includeSolutionsToggleButton
            // 
            this.includeSolutionsToggleButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.includeSolutionsToggleButton.Label = "Include Solutions";
            this.includeSolutionsToggleButton.Name = "includeSolutionsToggleButton";
            this.includeSolutionsToggleButton.ShowImage = true;
            this.includeSolutionsToggleButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.includeSolutionsToggleButton_Click);
            // 
            // markLimitTextBox
            // 
            this.markLimitTextBox.Label = "mark limit";
            this.markLimitTextBox.Name = "markLimitTextBox";
            this.markLimitTextBox.Text = null;
            this.markLimitTextBox.TextChanged += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.questionLimit_TextChanged);
            // 
            // masterSwitchGroup
            // 
            this.masterSwitchGroup.Items.Add(this.masterSwitchToggleButton);
            this.masterSwitchGroup.Label = "When SEND is clicked...";
            this.masterSwitchGroup.Name = "masterSwitchGroup";
            // 
            // masterSwitchToggleButton
            // 
            this.masterSwitchToggleButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.masterSwitchToggleButton.Image = global::AME_addin.Properties.Resources._1483162356_power_button;
            this.masterSwitchToggleButton.Label = "master ON OFF";
            this.masterSwitchToggleButton.Name = "masterSwitchToggleButton";
            this.masterSwitchToggleButton.ShowImage = true;
            this.masterSwitchToggleButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.masterSwitchToggleButton_Click);
            // 
            // saveSettingsGroup
            // 
            this.saveSettingsGroup.Items.Add(this.saveSettings);
            this.saveSettingsGroup.Label = "Settings";
            this.saveSettingsGroup.Name = "saveSettingsGroup";
            // 
            // saveSettings
            // 
            this.saveSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.saveSettings.Image = global::AME_addin.Properties.Resources._1483162181_icons_save;
            this.saveSettings.Label = "Save these settings for next time";
            this.saveSettings.Name = "saveSettings";
            this.saveSettings.ShowImage = true;
            this.saveSettings.Visible = false;
            this.saveSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.saveSettings_Click);
            // 
            // MailItemRibbon
            // 
            this.Name = "MailItemRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose, Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.MailItemTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MailItemRibbon_Load);
            this.MailItemTab.ResumeLayout(false);
            this.MailItemTab.PerformLayout();
            this.AMEgroup.ResumeLayout(false);
            this.AMEgroup.PerformLayout();
            this.masterSwitchGroup.ResumeLayout(false);
            this.masterSwitchGroup.PerformLayout();
            this.saveSettingsGroup.ResumeLayout(false);
            this.saveSettingsGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab MailItemTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup AMEgroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton shuffleQuestionsToggleButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox markLimitTextBox;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton includeSolutionsToggleButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup masterSwitchGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton masterSwitchToggleButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup saveSettingsGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton saveSettings;
    }

    partial class ThisRibbonCollection
    {
        internal MailItemRibbon MailItemRibbon
        {
            get { return this.GetRibbon<MailItemRibbon>(); }
        }
    }
}
