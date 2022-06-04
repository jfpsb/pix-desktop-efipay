namespace VMIClientePix.ViewModel.Interfaces
{
    /// <summary>
    /// Usado nas ViewModels para disponibilizar um método que pode ser chamado no code behind da View no evento Closing.
    /// Para atribuir o método ao evento Closing da janela use o evento Loaded.
    /// </summary>
    /// <example>Este exemplo mostra como atribuir o método OnClosing da ViewModel no evento Closing da View.
    /// <code>
    /// if (DataContext is IOnClosing)
    /// {
    ///     Closing += (_, _) =>
    ///     {
    ///         (DataContext as IOnClosing).OnClosingFromVM();
    ///     };
    /// }
    /// </code>
    /// </example>
    public interface IOnClosing
    {
        void OnClosingFromVM();
    }
}
