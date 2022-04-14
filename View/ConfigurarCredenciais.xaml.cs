using Microsoft.Win32;
using System.Windows;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfigurarCredenciais.xaml
    /// </summary>
    public partial class ConfigurarCredenciais : Window, IOpenFileDialog
    {
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
            if (DataContext is IRequestClose)
            {
                (DataContext as IRequestClose).RequestClose += (_, __) => this.Close();
            }
        }
    }
}
