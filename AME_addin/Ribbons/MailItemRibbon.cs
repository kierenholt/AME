using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using AME_addin.Properties;
using Microsoft.Office.Interop.Outlook;
 

namespace AME_addin
{
    public partial class MailItemRibbon
    {
        private void MailItemRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            this.shuffleQuestionsToggleButton.Checked = Settings.Default.shuffleQuestionsEnabled;
            this.includeSolutionsToggleButton.Checked = Settings.Default.includeSolutions;
            this.markLimitTextBox.Text = Settings.Default.markLimitValue.ToString();


            masterSwitchToggleButton.Checked = Settings.Default.ItemSendEventEnabled;
            this.AMEgroup.Visible = Settings.Default.ItemSendEventEnabled;
        }

        private void shuffleQuestionsToggleButton_Click(object sender, RibbonControlEventArgs e)
        {
            Settings.Default.shuffleQuestionsEnabled = this.shuffleQuestionsToggleButton.Checked;
            //Settings.Default.Save(); do not persist
        }

        private void questionLimit_TextChanged(object sender, RibbonControlEventArgs e)
        {
            byte trial;
            if (byte.TryParse(this.markLimitTextBox.Text, out trial) && trial > 0 && trial <= 100)
            {
                Settings.Default.markLimitValue = trial;
                //Settings.Default.Save(); do not persist
            }
        }

        private void includeSolutionsToggleButton_Click(object sender, RibbonControlEventArgs e)
        {
            Settings.Default.includeSolutions = this.includeSolutionsToggleButton.Checked;
            //Settings.Default.Save(); do not persist
        }


        private void masterSwitchToggleButton_Click(object sender, RibbonControlEventArgs e)
        {
            this.AMEgroup.Visible = masterSwitchToggleButton.Checked;
            Settings.Default.ItemSendEventEnabled = masterSwitchToggleButton.Checked;
            //Settings.Default.Save(); do not persist
        }

        private void saveSettings_Click(object sender, RibbonControlEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
