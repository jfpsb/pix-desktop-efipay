using Newtonsoft.Json;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Cobranca : ObservableObject
    {
        private Calendario _calendario;
        private Valor _valor;
        private Loc _loc;
        private string _txid;
        private int _revisao;
        private string _status;
        private string _chave;
        private string _location;

        [JsonProperty(PropertyName = "calendario")]
        public Calendario Calendario
        {
            get
            {
                return _calendario;
            }

            set
            {
                _calendario = value;
                OnPropertyChanged("Calendario");
            }
        }

        [JsonProperty(PropertyName = "valor")]
        public Valor Valor
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

        [JsonProperty(PropertyName = "txid")]
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

        [JsonProperty(PropertyName = "revisao")]
        public int Revisao
        {
            get
            {
                return _revisao;
            }

            set
            {
                _revisao = value;
                OnPropertyChanged("Revisao");
            }
        }

        [JsonProperty(PropertyName = "status")]
        public string Status
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

        [JsonProperty(PropertyName = "chave")]
        public string Chave
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

        public string Location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }

        public Loc Loc
        {
            get
            {
                return _loc;
            }

            set
            {
                _loc = value;
                OnPropertyChanged("Loc");
            }
        }
    }
}
