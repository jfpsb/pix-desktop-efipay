using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Valor : AModel
    {
        private long _id;
        private double _original;

        public virtual long Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public virtual double Original
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
