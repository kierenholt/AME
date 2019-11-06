using System.Windows;

namespace AME_addin
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class MarkBookWindow : Window
    {
        public MarkBookWindow(TopViewModel topVM)
        {
            DataContext = topVM;
            InitializeComponent();
        }

    }
}
