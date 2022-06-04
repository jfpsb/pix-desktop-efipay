namespace SincronizacaoServico.Model
{
    public class Pagador : AModel
    {
        private int _id;
        private string _cpf;
        private string _cnpj;
        private string _nome;

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

        public virtual string Cpf
        {
            get
            {
                return _cpf;
            }

            set
            {
                _cpf = value;
                OnPropertyChanged("Cpf");
            }
        }

        public virtual string Cnpj
        {
            get
            {
                return _cnpj;
            }

            set
            {
                _cnpj = value;
                OnPropertyChanged("Cnpj");
            }
        }

        public virtual string Nome
        {
            get
            {
                return _nome;
            }

            set
            {
                _nome = value;
                OnPropertyChanged("Nome");
            }
        }

        public override void Copiar(object source)
        {
            Pagador p = source as Pagador;
            Cpf = p.Cpf;
            Cnpj = p.Cnpj;
            Nome = p.Nome;

            Uuid = p.Uuid;
            CriadoEm = p.CriadoEm;
            ModificadoEm = p.ModificadoEm;
            DeletadoEm = p.DeletadoEm;
            Deletado = p.Deletado;
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
