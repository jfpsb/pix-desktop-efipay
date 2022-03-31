using System.Windows;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfiguracaoImpressora.xaml
    /// </summary>
    public partial class ConfiguracaoImpressora : Window
    {
        public ConfiguracaoImpressora()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var onClosingDataContext = DataContext as IOnClosing;
            onClosingDataContext.OnClosing();
        }
    }
}
