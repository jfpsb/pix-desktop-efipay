using Newtonsoft.Json;

namespace SincronizacaoServico.Model
{
    public class Pix : AModel
    {
        private string _endToEndId;
        private string _txid;
        private Pagador _pagador;
        private Cobranca _cobranca;
        private double _valor;
        private string _chave;
        private DateTime _horario;
        private string _infoPagador;
        private IList<Devolucao> _devolucoes = new List<Devolucao>();

        public override object GetIdentifier()
        {
            return EndToEndId;
        }

        public override void SetIdentifierToDefault()
        {
            EndToEndId = null;
        }

        [JsonProperty(PropertyName = "txid")]
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
                OnPropertyChanged("HorarioLocalTime");
            }
        }

        public virtual DateTime HorarioLocalTime
        {
            get
            {
                return Horario.ToLocalTime();
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

        public virtual string Chave
        {
            get
            {
                return _chave;
            }

            set
            {
                _chave = value;
                OnPropertyChanged("Chave");
            }
        }

        public override void InicializaLazy()
        {
            throw new NotImplementedException();
        }

        public override void Copiar(object source)
        {
            Pix pix = source as Pix;
            EndToEndId = pix.EndToEndId;
            Txid = pix.Txid;
            Pagador = pix.Pagador;
            Cobranca = pix.Cobranca;
            Valor = pix.Valor;
            Chave = pix.Chave;
            Horario = pix.Horario;
            InfoPagador = pix.InfoPagador;
            //Devolucoes = new List<Devolucao>(pix.Devolucoes);

            Uuid = pix.Uuid;
            CriadoEm = pix.CriadoEm;
            ModificadoEm = pix.ModificadoEm;
            DeletadoEm = pix.DeletadoEm;
            Deletado = pix.Deletado;
        }
    }
}
