using Gerencianet.NETCore.SDK;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using VMIClientePix.Util;

namespace VMIClientePix.ViewModel
{
    public class InformaValorPixViewModel : ObservableObject
    {
        public ICommand GerarQRCodeComando { get; set; }
        private double _valorPix;
        public InformaValorPixViewModel()
        {
            GerarQRCodeComando = new RelayCommand(GerarQRCode);
        }

        private void GerarQRCode(object obj)
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));

            var body = new
            {
                calendario = new
                {
                    expiracao = 600
                },
                valor = new
                {
                    original = ValorPix.ToString("F2", CultureInfo.InvariantCulture)
                },
                chave = "7cd4f18d-86d4-4bf7-8905-be0e1c46e9da"
            };

            try
            {
                var cobrancaPix = endpoints.PixCreateImmediateCharge(null, body);
                JObject cobrancaPixJson = JObject.Parse(cobrancaPix);

                var paramQRCode = new
                {
                    id = cobrancaPixJson["loc"]["id"]
                };

                try
                {
                    var qrCode = endpoints.PixGenerateQRCode(paramQRCode);

                    // Generate QRCode Image to JPEG Format
                    JObject qrCodeJson = JObject.Parse(qrCode);
                    string img = (string)qrCodeJson["imagemQrcode"];
                    img = img.Replace("data:image/png;base64,", "");

                    var qrCodeImage = Image.FromStream(new MemoryStream(Convert.FromBase64String(img)));
                    qrCodeImage.Save("QRCodeImage.jpg");

                }
                catch (GnException e)
                {
                    Console.WriteLine(e.ErrorType);
                    Console.WriteLine(e.Message);
                }
            }
            catch (GnException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
        }

        public double ValorPix
        {
            get => _valorPix;
            set
            {
                _valorPix = value;
                OnPropertyChanged("ValorPix");
            }
        }
    }
}
