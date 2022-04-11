using System.Windows;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for InformaValorPix.xaml
    /// </summary>
    public partial class InformaValorPix : Window, ICloseable
    {
        public InformaValorPix()
        {
            InitializeComponent();
        }

        private void TelaInformarValorPix_Loaded(object sender, RoutedEventArgs e)
        {
            TxtValor.SelectAll();
        }

        private void TelaInformarValorPix_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var onClosingDataContext = DataContext as IOnClosing;
            onClosingDataContext.OnClosing();
        }
    }
}
