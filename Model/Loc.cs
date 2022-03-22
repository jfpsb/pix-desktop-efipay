using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Loc : ObservableObject
    {
        private int _id;
        private string _location;
        private string _tipoCob;

        public int Id
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

        public string Location
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

        public string TipoCob
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
