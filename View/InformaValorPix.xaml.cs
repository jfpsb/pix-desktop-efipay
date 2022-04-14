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

        public void CloseView()
        {
            Close();
        }

        private void TelaInformarValorPix_Loaded(object sender, RoutedEventArgs e)
        {
            TxtValor.SelectAll();

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
