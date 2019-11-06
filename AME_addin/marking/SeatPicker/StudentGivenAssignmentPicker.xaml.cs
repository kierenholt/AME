

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SeatGivenAssignmentPicker.xaml
    /// </summary>
    public partial class SeatGivenAssignmentPickerWindow : Window
    {
        public SeatGivenAssignmentPickerWindow(SeatGivenAssignmentPickerViewModel svm)
        {
            DataContext = svm;
            InitializeComponent();
        }
    }
}
