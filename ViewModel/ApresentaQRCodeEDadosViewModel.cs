using Gerencianet.NETCore.SDK;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMIClientePix.Model;
using VMIClientePix.Util;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class ApresentaQRCodeEDadosViewModel : ObservableObject, IOnClosing
    {
        private string _fantasia;
        private string _razao;
        private string _cnpj;
        private string _instituicao;
        private double _valor;
        private ImageSource _imagemQrCode;
        private ICloseable _closeableOwner;
        public ApresentaQRCodeEDadosViewModel(Cobranca cobranca, IMessageBoxService messageBoxService, ICloseable closeableOwner = null)
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));
            var dados = JObject.Parse(File.ReadAllText("dados_recebedor.json"));
            _closeableOwner = closeableOwner;

            var paramQRCode = new
            {
                id = cobranca.Loc.Id
            };

            try
            {
                var qrCode = endpoints.PixGenerateQRCode(paramQRCode);

                // Generate QRCode Image to JPEG Format
                JObject qrCodeJson = JObject.Parse(qrCode);
                string img = (string)qrCodeJson["imagemQrcode"];
                img = img.Replace("data:image/png;base64,", "");

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(img));
                bitmapImage.EndInit();

                //ImagemQrCode = Image.FromStream(new MemoryStream(Convert.FromBase64String(img)));
                ImagemQrCode = bitmapImage;
                Fantasia = (string)dados["fantasia"];
                Razao = (string)dados["razaosocial"];
                Cnpj = (string)dados["cnpj"];
                Instituicao = (string)dados["instituicao"];
                Valor = cobranca.Valor.Original;
            }
            catch (GnException e)
            {
                messageBoxService.Show($"Houve Um Erro Ao Mostrar Informações de Cobrança Pix.\n\n{e.ErrorType}\n\n{e.Message}");
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                messageBoxService.Show($"Não Foi Possível Mostrar Informações De Cobrança Pix. Cheque Sua Conexão Com A Internet.\n\n{ex.Message}", "Dados De Cobrança Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public string Fantasia
        {
            get
            {
                return _fantasia;
            }

            set
            {
                _fantasia = value;
                OnPropertyChanged("Fantasia");
            }
        }

        public string Razao
        {
            get
            {
                return _razao;
            }

            set
            {
                _razao = value;
                OnPropertyChanged("Razao");
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

        public string Instituicao
        {
            get
            {
                return _instituicao;
            }

            set
            {
                _instituicao = value;
                OnPropertyChanged("Instituicao");
            }
        }

        public ImageSource ImagemQrCode
        {
            get
            {
                return _imagemQrCode;
            }

            set
            {
                _imagemQrCode = value;
                OnPropertyChanged("ImagemQrCode");
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

        public void OnClosing()
        {
            if (_closeableOwner != null)
                _closeableOwner.Close();
        }
    }
}
