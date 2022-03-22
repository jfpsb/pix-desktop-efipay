using Newtonsoft.Json;
using System;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class Parametros : ObservableObject
    {
        private DateTime _inicio;
        private DateTime _fim;

        [JsonProperty(PropertyName = "inicio")]
        public DateTime Inicio
        {
            get
            {
                return _inicio.ToLocalTime();
            }

            set
            {
                _inicio = value;
                OnPropertyChanged("Inicio");
            }
        }

        [JsonProperty(PropertyName = "fim")]
        public DateTime Fim
        {
            get
            {
                return _fim.ToLocalTime();
            }

            set
            {
                _fim = value;
                OnPropertyChanged("Fim");
            }
        }
    }
}
