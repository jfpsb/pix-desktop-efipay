namespace SincronizacaoServico.Model
{
    public class Horario : AModel
    {
        private int _id;
        private DateTime _solicitacao;
        private DateTime _liquidacao;

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

        public virtual DateTime Solicitacao
        {
            get
            {
                return _solicitacao;
            }

            set
            {
                _solicitacao = value;
                OnPropertyChanged("Solicitacao");
            }
        }

        public virtual DateTime Liquidacao
        {
            get
            {
                return _liquidacao;
            }

            set
            {
                _liquidacao = value;
                OnPropertyChanged("Liquidacao");
            }
        }

        public override void Copiar(object source)
        {
            Horario d = source as Horario;
            Solicitacao = d.Solicitacao;
            Liquidacao = d.Liquidacao;

            Uuid = d.Uuid;
            CriadoEm = d.CriadoEm;
            ModificadoEm = d.ModificadoEm;
            DeletadoEm = d.DeletadoEm;
            Deletado = d.Deletado;
        }

        public override object GetIdentifier()
        {
            return Id;
        }

        public override void InicializaLazy()
        {
            throw new NotImplementedException();
        }

        public override void SetIdentifierToDefault()
        {
            throw new NotImplementedException();
        }
    }
}
