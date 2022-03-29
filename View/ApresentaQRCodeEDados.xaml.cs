using System.Windows;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ApresentaQRCodeEDados.xaml
    /// </summary>
    public partial class ApresentaQRCodeEDados : Window
    {
        public ApresentaQRCodeEDados()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var onClosingDataContext = DataContext as IOnClosing;
            onClosingDataContext.OnClosing();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var onLoadDataContext = DataContext as IOnLoad;
            onLoadDataContext.OnWindowLoad();
        }
    }
}
