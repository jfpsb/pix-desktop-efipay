using System.Windows;
using System.Windows.Controls;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfiguracaoImpressora.xaml
    /// </summary>
    public partial class ConfiguracaoImpressora : UserControl
    {
        private Window window;
        public ConfiguracaoImpressora()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Title = "Configuração De Impressora";
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            window.Closing += (_, _) =>
            {
                if (DataContext is IOnClosing)
                {
                    (DataContext as IOnClosing).OnClosingFromVM();
                }
            };
        }
    }
}
