using System;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Devolucao : ObservableObject
    {
        private string _id;
        private Pix _pix;
        private Horario _horario;
        private string _rtrId;
        private double _valor;
        private string _status;

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
    }

    public class Horario : ObservableObject
    {
        private long _id;
        private DateTime _solicitacao;
        private DateTime _liquidacao;

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
    }
}
