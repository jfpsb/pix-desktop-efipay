using System.Windows;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as IOnClosing).OnClosing();
        }
    }
}
