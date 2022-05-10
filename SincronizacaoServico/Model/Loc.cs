namespace SincronizacaoServico.Model
{
    public class Loc : AModel
    {
        private int _id;
        private string _location;
        private string _tipoCob;

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

        public override void InicializaLazy()
        {
            throw new System.NotImplementedException();
        }

        public override void Copiar(object source)
        {
            Loc l = source as Loc;
            Location = l.Location;
            TipoCob = l.TipoCob;

            CriadoEm = l.CriadoEm;
            ModificadoEm = l.ModificadoEm;
            DeletadoEm = l.DeletadoEm;
            Deletado = l.Deletado;
        }
    }
}
