using System;
using System.Collections.Generic;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Pix : AModel
    {
        private string _txid;
        private Pagador _pagador;
        private Cobranca _cobranca;
        private string _endToEndId;
        private double _valor;
        private DateTime _horario;
        private string _infoPagador;
        private IList<Devolucao> _devolucoes = new List<Devolucao>();

        public virtual string Txid
        {
            get
            {
                return _txid;
            }

            set
            {
                _txid = value;
                OnPropertyChanged("Txid");
            }
        }

        public virtual Pagador Pagador
        {
            get
            {
                return _pagador;
            }

            set
            {
                _pagador = value;
                OnPropertyChanged("Pagador");
            }
        }

        public virtual string EndToEndId
        {
            get
            {
                return _endToEndId;
            }

            set
            {
                _endToEndId = value;
                OnPropertyChanged("EndToEndId");
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

        public virtual DateTime Horario
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

        public virtual string InfoPagador
        {
            get
            {
                return _infoPagador;
            }

            set
            {
                _infoPagador = value;
                OnPropertyChanged("InfoPagador");
            }
        }

        public virtual IList<Devolucao> Devolucoes
        {
            get
            {
                return _devolucoes;
            }

            set
            {
                _devolucoes = value;
                OnPropertyChanged("Devolucoes");
            }
        }

        public virtual Cobranca Cobranca
        {
            get
            {
                return _cobranca;
            }

            set
            {
                _cobranca = value;
                OnPropertyChanged("Cobranca");
            }
        }
    }

    public class Pagador : AModel
    {
        private long _id;
        private string _cpf;
        private string _cnpj;
        private string _nome;

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
    }
}
