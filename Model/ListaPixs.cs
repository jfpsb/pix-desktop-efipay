using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class ListaPixs : ObservableObject
    {
        private Parametros _parametros;
        IList<Pix> _pixs = new List<Pix>();

        [JsonProperty(PropertyName = "pix")]
        public IList<Pix> Pixs
        {
            get
            {
                return _pixs;
            }

            set
            {
                _pixs = value;
                OnPropertyChanged("Pixs");
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