using System.Windows;
using VMIClientePix.View.Interfaces;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IOnClosing)
            {
                Closing += (_, _) =>
                {
                    (DataContext as IOnClosing).OnClosingFromVM();
                };
            }
        }
    }
}
