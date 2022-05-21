using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Model
{
    public class ListaCobrancas : ObservableObject
    {
        private Parametros _parametros;
        IList<Cobranca> _cobrancas = new List<Cobranca>();

        [JsonProperty(PropertyName = "cobs")]
        public IList<Cobranca> Cobrancas
        {
            get
            {
                return _cobrancas;
            }

            set
            {
                _cobrancas = value;
                OnPropertyChanged("Cobrancas");
            }
        }

        public Parametros Parametros
        {
            get
            {
                return _parametros;
            }

            set
            {
                _parametros = value;
                OnPropertyChanged("Parametros");
            }
        }
    }
}