using Gerencianet.NETCore.SDK;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
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
        private ISession _session;
        private Cobranca _cobranca;
        private IMessageBoxService _messageBox;
        private DAOCobranca daoCobranca;

        public ApresentaQRCodeEDadosViewModel(ISession session, Cobranca cobranca, IMessageBoxService messageBoxService, ICloseable closeableOwner = null)
        {
            _session = session;
            _closeableOwner = closeableOwner;
            _cobranca = cobranca;
            _messageBox = messageBoxService;

            daoCobranca = new DAOCobranca(session);

            GeraESalvaQrCode();
        }

        private async void GeraESalvaQrCode()
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));
            var dados = JObject.Parse(File.ReadAllText("dados_recebedor.json"));

            var paramQRCode = new
            {
                id = _cobranca.Loc.Id
            };

            if (_cobranca.QrCode == null)
            {
                try
                {

                    var qrCodeGn = endpoints.PixGenerateQRCode(paramQRCode);

                    QRCode qrCode = new QRCode();

                    // Generate QRCode Image to JPEG Format
                    JObject qrCodeJson = JObject.Parse(qrCodeGn);

                    qrCode.Qrcode = (string)qrCodeJson["qrcode"];
                    qrCode.ImagemQrcode = ((string)qrCodeJson["imagemQrcode"]).Replace("data:image/png;base64,", "");

                    _cobranca.QrCode = qrCode;

                    var result = await daoCobranca.Atualizar(_cobranca);

                    if (result)
                    {
                        PopulaDados(dados);
                    }
                    else
                    {
                        throw new Exception("Erro Ao Salvar Dados De QRCode!");
                    }

                }
                catch (GnException e)
                {
                    _messageBox.Show($"Houve Um Erro Ao Mostrar Informações de Cobrança Pix.\n\n{e.ErrorType}\n\n{e.Message}");
                    Debug.WriteLine(e.ErrorType);
                    Debug.WriteLine(e.Message);
                }
                catch (Exception ex)
                {
                    _messageBox.Show($"Não Foi Possível Mostrar Informações De Cobrança Pix. Cheque Sua Conexão Com A Internet.\n\n{ex.Message}", "Dados De Cobrança Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            else
            {
                PopulaDados(dados);
            }
        }

        private void PopulaDados(dynamic dados)
        {
            ImagemQrCode = _cobranca.QrCode.QrCodeBitmap;
            Fantasia = (string)dados["fantasia"];
            Razao = (string)dados["razaosocial"];
            Cnpj = (string)dados["cnpj"];
            Instituicao = (string)dados["instituicao"];
            Valor = _cobranca.Valor.Original;
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
