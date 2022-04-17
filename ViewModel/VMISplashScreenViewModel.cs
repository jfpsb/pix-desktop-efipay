using System;
using VMIClientePix.Util;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class VMISplashScreenViewModel : ObservableObject, IRequestClose
    {
        public event EventHandler<EventArgs> RequestClose;

        public void CloseView()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
