namespace SincronizacaoServico.Model
{
    public class QRCode : AModel
    {
        private int _id;
        private string _qrcode;
        private string _imagemQrcode;

        public override object GetIdentifier()
        {
            return Id;
        }

        public override void SetIdentifierToDefault()
        {
            Id = 0;
        }

        public virtual int Id
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

        public override void InicializaLazy()
        {
            throw new NotImplementedException();
        }

        public override void Copiar(object source)
        {
            QRCode q = source as QRCode;
            Qrcode = q.Qrcode;
            ImagemQrcode = q.ImagemQrcode;
        }
    }
}
