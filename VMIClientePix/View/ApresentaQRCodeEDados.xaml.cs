using System.Windows;
using System.Windows.Controls;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for ApresentaQRCodeEDados.xaml
    /// </summary>
    public partial class ApresentaQRCodeEDados : UserControl
    {
        private Window window;
        public ApresentaQRCodeEDados()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Title = "Dados e QRCode De Cobrança Pix";
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
