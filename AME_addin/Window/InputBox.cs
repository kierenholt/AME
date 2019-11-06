using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AME_addin.Providers
{
    public partial class InputBox : Form
    {
        //sample usage
        
                            //InputBox inputbox = new InputBox("Enter a name for the class", false);
                            //if (inputbox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            //{
                            //    newSeatGroup.Name = inputbox.text;
                            //}
                            //if (inputbox.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                            //{
                            //    db.SeatGroups.Remove(newSeatGroup);
                            //    newSeatGroup = null;
                            //    return;
                            //}


        private bool _acceptBlanks;
        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string caption, bool paramAcceptBlanks = false)
        {
            InitializeComponent();
            label1.Text = caption;
            _acceptBlanks = paramAcceptBlanks;
        }

        public string text
        {
            get { return this.textBox1.Text; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            if (!_acceptBlanks && string.IsNullOrEmpty(textBox1.Text))
                return;
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
