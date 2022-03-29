using System;
using System.Collections.Generic;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Pix : ObservableObject
    {
        private string _txid;
        private Pagador _pagador;
        private string _endToEndId;
        private double _valor;
        private DateTime _horario;
        private string _infoPagador;
        private IList<Devolucao> _devolucoes = new List<Devolucao>();

        public string Txid
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

        public Pagador Pagador
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

        public string EndToEndId
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

        public double Valor
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

        public DateTime Horario
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

        public string InfoPagador
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

        public IList<Devolucao> Devolucoes
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
    }

    public class Pagador : ObservableObject
    {
        private long _id;
        private string _cpf;
        private string _cnpj;
        private string _nome;

        public long Id
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

        public string Cpf
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

        public string Cnpj
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

        public string Nome
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
