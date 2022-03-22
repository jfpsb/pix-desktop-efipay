using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Valor : ObservableObject
    {
        private double _original;

        public double Original
        {
            get
            {
                return _original;
            }

            set
            {
                _original = value;
                OnPropertyChanged("Original");
            }
        }
    }
}
