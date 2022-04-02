using Microsoft.Win32;
using System.Windows;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ConfigurarCredenciais.xaml
    /// </summary>
    public partial class ConfigurarCredenciais : Window, ICloseable, IOpenFileDialog
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
    }
}
