namespace VMIClientePix.ViewModel.Interfaces
{
    /// <summary>
    /// Usado nas ViewModels para disponibilizar um método que retorne qualquer tipo de dado após a janela atrelada a ViewModel ser fechada.
    /// Somente funciona com janelas abertas usando o método ShowDialog.
    /// </summary>
    public interface IReturnData
    {
        object GetData();
    }
}
