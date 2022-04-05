using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Loc : AModel
    {
        private int _id;
        private string _location;
        private string _tipoCob;

        public virtual int Id
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

        public virtual string Location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }

        public virtual string TipoCob
        {
            get
            {
                return _tipoCob;
            }

            set
            {
                _tipoCob = value;
                OnPropertyChanged("TipoCob");
            }
        }
    }
}
