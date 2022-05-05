namespace SincronizacaoServico.Model
{
    public class Valor : AModel
    {
        private int _id;
        private double _original;

        public override object GetIdentifier()
        {
            return Id;
        }
        public override void SetIdentifierToDefault()
        {
            Id = 0;
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

        public override void Copiar(object source)
        {
            Valor v = source as Valor;
            Original = v.Original;
        }
    }
}
