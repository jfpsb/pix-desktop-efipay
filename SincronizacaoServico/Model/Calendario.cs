using System;

namespace SincronizacaoServico.Model
{
    public class Calendario : AModel
    {
        private int _id;
        private DateTime _criacao;
        private DateTime _apresentacao;
        private int _expiracao;

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
        public virtual DateTime Criacao
        {
            get => _criacao;
            set
            {
                _criacao = value;
                OnPropertyChanged("Criacao");
                OnPropertyChanged("CriacaoLocalTime");
            }
        }
        public virtual DateTime CriacaoLocalTime
        {
            get => _criacao.ToLocalTime();
        }
        public virtual DateTime Apresentacao
        {
            get => _apresentacao;
            set
            {
                _apresentacao = value;
                OnPropertyChanged("Apresentacao");
                OnPropertyChanged("ApresentacaoLocalTime");
            }
        }
        public virtual DateTime ApresentacaoLocalTime
        {
            get => _apresentacao.ToLocalTime();
        }
        public virtual int Expiracao
        {
            get => _expiracao;
            set
            {
                _expiracao = value;
                OnPropertyChanged("Expiracao");
            }
        }

        public override void Copiar(object source)
        {
            Calendario c = source as Calendario;
            Criacao = c.Criacao;
            Apresentacao = c.Apresentacao;
            Expiracao = c.Expiracao;

            CriadoEm = c.CriadoEm;
            ModificadoEm = c.ModificadoEm;
            DeletadoEm = c.DeletadoEm;
            Deletado = c.Deletado;
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
            Id = 0;
        }
    }
}
