using System;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Calendario : ObservableObject
    {
        private DateTime _criacao;
        private DateTime _apresentacao;
        private int _expiracao;

        public DateTime Criacao
        {
            get => _criacao;
            set
            {
                _criacao = value;
                OnPropertyChanged("Criacao");
                OnPropertyChanged("CriacaoLocalTime");
            }
        }
        public DateTime CriacaoLocalTime
        {
            get => Criacao.ToLocalTime();
        }
        public DateTime Apresentacao
        {
            get => _apresentacao;
            set
            {
                _apresentacao = value;
                OnPropertyChanged("Apresentacao");
                OnPropertyChanged("ApresentacaoLocalTime");
            }
        }
        public DateTime ApresentacaoLocalTime
        {
            get => _apresentacao.ToLocalTime();
        }
        public int Expiracao
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
