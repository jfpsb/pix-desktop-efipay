using System.Windows;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ApresentaDadosTransfPix.xaml
    /// </summary>
    public partial class ApresentaDadosTransfPix : Window
    {
        public ApresentaDadosTransfPix()
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
