namespace VMIClientePix.View.Interfaces
{
    /// <summary>
    /// Usado nas Views para disponibilizar um método que pode ser chamado em ViewModels para abrir um OpenFileDialog.
    /// </summary>
    public interface IOpenFileDialog
    {
        string OpenFileDialog();
    }
}
