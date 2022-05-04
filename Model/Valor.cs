namespace VMIClientePix.Model
{
    public class Valor : AModel
    {
        private int _id;
        private double _original;

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

        public override void InicializaLazy()
        {
            throw new System.NotImplementedException();
        }
    }
}
