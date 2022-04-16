using System.Windows;

namespace VMIClientePix.ViewModel.Services.Concretos
{
    public class OpenView : IOpenViewService
    {
        public void Show(object viewModel)
        {
            var view = new Window();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.Content = viewModel;
            view.Show();
        }

        public bool? ShowDialog(object viewModel)
        {
            var view = new Window();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.SizeToContent = SizeToContent.WidthAndHeight;
            view.Content = viewModel;
            return view.ShowDialog();
        }
    }
}
