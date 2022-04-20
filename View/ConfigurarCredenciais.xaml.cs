using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfigurarCredenciais.xaml
    /// </summary>
    public partial class ConfigurarCredenciais : UserControl, IOpenFileDialog
    {
        private Window window;
        public ConfigurarCredenciais()
        {
            InitializeComponent();
        }

        public string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private void TelaConfigurarCredenciais_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Title = "Configurar Credenciais";

            if (DataContext is IRequestClose)
            {
                (DataContext as IRequestClose).RequestClose += (_, _) => window.Close();
            }
        }
    }
}
