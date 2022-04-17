using System.Windows;
using System.Windows.Controls;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ApresentaDadosTransfPix.xaml
    /// </summary>
    public partial class ApresentaDadosTransfPix : UserControl
    {
        private Window window;
        public ApresentaDadosTransfPix()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Title = "Dados De Transferência Pix";
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
