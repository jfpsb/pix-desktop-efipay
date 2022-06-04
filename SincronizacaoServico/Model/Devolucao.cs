namespace SincronizacaoServico.Model
{
    public class Devolucao : AModel
    {
        private string _id;
        private Pix _pix;
        private Horario _horario;
        private string _rtrId;
        private double _valor;
        private string _status;

        public override object GetIdentifier()
        {
            return Id;
        }

        public override void SetIdentifierToDefault()
        {
            Id = null;
        }

        public virtual string Id
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

        public virtual Pix Pix
        {
            get
            {
                return _pix;
            }

            set
            {
                _pix = value;
                OnPropertyChanged("Pix");
            }
        }

        public virtual string RtrId
        {
            get
            {
                return _rtrId;
            }

            set
            {
                _rtrId = value;
                OnPropertyChanged("RtrId");
            }
        }

        public virtual double Valor
        {
            get
            {
                return _valor;
            }

            set
            {
                _valor = value;
                OnPropertyChanged("Valor");
            }
        }

        public virtual string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public virtual Horario Horario
        {
            get
            {
                return _horario;
            }

            set
            {
                _horario = value;
                OnPropertyChanged("Horario");
            }
        }

        public override void InicializaLazy()
        {
            throw new NotImplementedException();
        }

        public override void Copiar(object source)
        {
            Devolucao d = source as Devolucao;
            Id = d.Id;
            Pix = d.Pix;
            Horario = d.Horario;
            RtrId = d.RtrId;
            Valor = d.Valor;
            Status = d.Status;

            Uuid = d.Uuid;
            CriadoEm = d.CriadoEm;
            ModificadoEm = d.ModificadoEm;
            DeletadoEm = d.DeletadoEm;
            Deletado = d.Deletado;
        }
    }
}
