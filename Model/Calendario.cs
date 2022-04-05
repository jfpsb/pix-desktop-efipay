using System;

namespace VMIClientePix.Model
{
    public class Calendario : AModel
    {
        private long _id;
        private DateTime _criacao;
        private DateTime _apresentacao;
        private int _expiracao;

        public virtual long Id
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
    }
}
