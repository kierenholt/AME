
using AME_base;
using System.Windows;

namespace AME_addin
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class DeadlineQuestionsWindow : Window
    {

        public DeadlineQuestionsWindow(TopViewModel topVM)
        {
            DataContext = topVM;
            InitializeComponent();
        }

    }
}
