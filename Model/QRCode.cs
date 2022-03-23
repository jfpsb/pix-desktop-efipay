using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class QRCode : ObservableObject
    {
        private string _qrcode;
        private string _imagemQrcode;

        public string Qrcode
        {
            get
            {
                return _qrcode;
            }

            set
            {
                _qrcode = value;
                OnPropertyChanged("Qrcode");
            }
        }

        public string ImagemQrcode
        {
            get
            {
                return _imagemQrcode;
            }

            set
            {
                _imagemQrcode = value;
                OnPropertyChanged("ImagemQrcode");
            }
        }
    }
}
