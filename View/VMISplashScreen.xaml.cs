using System.Windows;
using System.Windows.Controls;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for TelaInicial.xaml
    /// </summary>
    public partial class VMISplashScreen : UserControl
    {
        private Window window;
        public VMISplashScreen()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.WindowStyle = WindowStyle.None;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;

            if (DataContext is IRequestClose)
            {
                (DataContext as IRequestClose).RequestClose += (_, __) =>
                {
                    window.Close();
                };
            }
        }
    }
}
