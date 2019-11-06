
using AME_base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AME_addin
{
    /// <summary>
    /// Interaction logic for MarkAssignment.xaml
    /// </summary>
    public partial class MarkAssignmentWindow : Window
    {
        public MarkAssignmentWindow(CompletedWorkViewModel paramVM)
        {
            DataContext = paramVM;
            InitializeComponent();
        }


    }
}
