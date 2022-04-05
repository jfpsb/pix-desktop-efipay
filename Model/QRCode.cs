using System;
using System.IO;
using System.Windows.Media.Imaging;
using VMIClientePix.Util;

namespace VMIClientePix.Model
{
    public class QRCode : AModel
    {
        private long _id;
        private string _qrcode;
        private string _imagemQrcode;

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

        public virtual string Qrcode
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

        public virtual string ImagemQrcode
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

        public virtual BitmapImage QrCodeBitmap
        {
            get
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(ImagemQrcode));
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
