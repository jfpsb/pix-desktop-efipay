using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Loc : AModel
    {
        private int _id;
        private string _location;
        private string _tipoCob;
        private Cobranca _cobranca;

        public override object GetIdentifier()
        {
            return Id;
        }

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

        public virtual Cobranca Cobranca
        {
            get
            {
                return _cobranca;
            }

            set
            {
                _cobranca = value;
                OnPropertyChanged("Cobranca");
            }
        }

        public override void InicializaLazy()
        {
            throw new System.NotImplementedException();
        }
    }
}
