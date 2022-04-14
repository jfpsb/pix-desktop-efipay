using System.Windows;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IRequestClose)
            {
                (DataContext as IRequestClose).RequestClose += (_, __) =>
                {
                    if (Owner != null)
                        Owner.Close();

                    Close();
                };
            }

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
