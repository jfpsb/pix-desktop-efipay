
namespace VMIClientePix.ViewModel.Services
{
    public interface IOpenViewService
    {
        void Show(object viewModel);
        bool? ShowDialog(object viewModel);
    }
}
