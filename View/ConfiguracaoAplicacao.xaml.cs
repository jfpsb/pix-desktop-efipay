using System.Windows;
using System.Windows.Controls;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfiguracaoAplicacao.xaml
    /// </summary>
    public partial class ConfiguracaoAplicacao : UserControl
    {
        private Window window;
        public ConfiguracaoAplicacao()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Title = "Configuração de Aplicação";
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
        }
    }
}
