using System.Windows;
using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.ViewModel.Services.Concretos
{
    public class OpenView : IOpenViewService
    {
        public void Show(IViewModel viewModel)
        {
            var view = new Window();
            view.Title = viewModel.TituloJanela();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.Content = viewModel;
            view.Show();
        }

        public bool? ShowDialog(IViewModel viewModel)
        {
            var view = new Window();
            view.Title = viewModel.TituloJanela();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.SizeToContent = SizeToContent.WidthAndHeight;
            view.Content = viewModel;
            return view.ShowDialog();
        }
    }
}
