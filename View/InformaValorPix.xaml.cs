using System.Windows;
using System.Windows.Controls;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for InformaValorPix.xaml
    /// </summary>
    public partial class InformaValorPix : UserControl, ICloseable
    {
        private Window window;

        public InformaValorPix()
        {
            InitializeComponent();
        }

        public void CloseView()
        {
            window.Close();
        }

        private void TelaInformarValorPix_Loaded(object sender, RoutedEventArgs e)
        {
            TxtValor.Focus();
            TxtValor.SelectAll();

            window = Window.GetWindow(this);
            window.Title = "Informar Valor De Cobrança Pix";
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
