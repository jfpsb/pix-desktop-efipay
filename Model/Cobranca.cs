using Newtonsoft.Json;
using System;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Cobranca : ObservableObject
    {
        private string _txid;
        private Calendario _calendario;
        private Valor _valor;
        private Loc _loc;
        private QRCode _qrCode;
        private int _revisao;
        private string _status;
        private string _chave;
        private string _location;
        private string _endToEndId;
        private DateTime _pagoEm;

        [JsonProperty(PropertyName = "calendario")]
        public virtual Calendario Calendario
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
        public virtual Valor Valor
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

        [JsonProperty(PropertyName = "revisao")]
        public virtual int Revisao
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

        [JsonProperty(PropertyName = "chave")]
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

        [JsonProperty(PropertyName = "location")]
        public virtual string Location
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

        [JsonProperty(PropertyName = "loc")]
        public virtual Loc Loc
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

        [JsonProperty(PropertyName = "endToEndId")]
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

        [JsonProperty(PropertyName = "horario")]
        public virtual DateTime PagoEm
        {
            get
            {
                return _pagoEm;
            }

            set
            {
                _pagoEm = value;
                OnPropertyChanged("PagoEm");
            }
        }

        public virtual QRCode QrCode
        {
            get
            {
                return _qrCode;
            }

            set
            {
                _qrCode = value;
                OnPropertyChanged("QrCode");
            }
        }
    }
}
