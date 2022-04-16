using VMIClientePix.ViewModel.Interfaces;

namespace VMIClientePix.ViewModel.Services
{
    public interface IOpenViewService
    {
        void Show(IViewModel viewModel);
        bool? ShowDialog(IViewModel viewModel);
    }
}
