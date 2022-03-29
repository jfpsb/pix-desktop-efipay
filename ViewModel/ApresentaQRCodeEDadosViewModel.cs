using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;
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
        private Visibility _visibilidadeLabelQRCode;
        private Visibility _visibilidadeImagemQRCode;
        private double _segundosDesdeCriacao;
        private string _labelQRCode1;
        private string _labelQRCode2;
        private string _segundosAteExpiracaoEmString;
        private Timer timerExpiracaoQrCode;
        private Timer timerConsultaCobranca;
        private DateTime expiraEm;

        public ICommand ImprimirQRCodeComando { get; set; }

        public ApresentaQRCodeEDadosViewModel(ISession session, Cobranca cobranca, IMessageBoxService messageBoxService, ICloseable closeableOwner = null)
        {
            _session = session;
            _closeableOwner = closeableOwner;
            Cobranca = cobranca;
            _messageBox = messageBoxService;

            ImprimirQRCodeComando = new RelayCommand(ImprimirQRCode, IsQRCodeValido);

            expiraEm = Cobranca.Calendario.CriacaoLocalTime.AddSeconds(Cobranca.Calendario.Expiracao);

            switch (Cobranca.Status)
            {
                case "CONCLUIDA":
                    LabelQRCode1 = "Esta Cobrança Já Foi Paga.";
                    LabelQRCode2 = "Crie Uma Nova Cobrança.";

                    VisibilidadeLabelQRCode = Visibility.Visible;
                    VisibilidadeImagemQRCode = Visibility.Collapsed;

                    PopulaDados();
                    break;
                case "ATIVA":
                    if (DateTime.Now >= expiraEm)
                    {
                        VisibilidadeLabelQRCode = Visibility.Visible;
                        VisibilidadeImagemQRCode = Visibility.Collapsed;

                        LabelQRCode1 = "QR Code Expirado.";
                        LabelQRCode2 = "Crie Uma Nova Cobrança.";

                        PopulaDados();
                    }
                    else
                    {
                        VisibilidadeLabelQRCode = Visibility.Collapsed;
                        VisibilidadeImagemQRCode = Visibility.Visible;

                        daoCobranca = new DAOCobranca(_session);

                        timerExpiracaoQrCode = new Timer(1000);
                        timerExpiracaoQrCode.Elapsed += TimerExpiracaoQrCode_Elapsed;
                        timerExpiracaoQrCode.AutoReset = true;
                        timerExpiracaoQrCode.Enabled = true;

                        timerConsultaCobranca = new Timer(10000);
                        timerConsultaCobranca.Elapsed += TimerConsultaCobranca_Elapsed;
                        timerConsultaCobranca.AutoReset = true;
                        timerConsultaCobranca.Enabled = true;

                        GeraESalvaQrCode();
                    }
                    break;
            };

            Cobranca.PropertyChanged += Cobranca_PropertyChanged;
        }

        private async void Cobranca_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status"))
            {
                if (Cobranca.Status.Equals("CONCLUIDA"))
                {
                    LabelQRCode1 = "Esta Cobrança Já Foi Paga.";
                    LabelQRCode2 = "Crie Uma Nova Cobrança.";

                    VisibilidadeLabelQRCode = Visibility.Visible;
                    VisibilidadeImagemQRCode = Visibility.Collapsed;

                    SegundosAteExpiracaoEmString = "PAGAMENTO EFETUADO";

                    timerConsultaCobranca.Stop();
                    timerConsultaCobranca.Dispose();
                    timerExpiracaoQrCode.Stop();
                    timerExpiracaoQrCode.Dispose();

                    var result = await daoCobranca.Atualizar(Cobranca);

                    if (result)
                    {
                        Debug.WriteLine("COBRANÇA SALVA COM SUCESSO NO BANCO DE DADOS!");
                    }
                    else
                    {
                        Debug.WriteLine("ERRO AO SALVAR COBRANÇA NO BANCO DE DADOS!");
                    }
                }
            }
        }

        private void TimerConsultaCobranca_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime < expiraEm)
            {
                dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));

                var param = new
                {
                    txid = Cobranca.Txid
                };

                try
                {
                    var response = endpoints.PixDetailCharge(param);
                    Cobranca cobranca = JsonConvert.DeserializeObject<Cobranca>(response);

                    Cobranca.Status = cobranca.Status;
                    Cobranca.Revisao = cobranca.Revisao;
                    Cobranca.Location = cobranca.Location;
                    //TODO: Campo PIX

                    Console.WriteLine(response);
                }
                catch (GnException gne)
                {
                    Debug.WriteLine(gne.ErrorType);
                    Debug.WriteLine(gne.Message);
                }
            }
            else
            {
                timerConsultaCobranca.Stop();
                timerConsultaCobranca.Dispose();
            }
        }

        private void TimerExpiracaoQrCode_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime > expiraEm)
            {
                VisibilidadeLabelQRCode = Visibility.Visible;
                VisibilidadeImagemQRCode = Visibility.Collapsed;
                LabelQRCode1 = "QR Code Expirado.";
                LabelQRCode2 = "Crie Uma Nova Cobrança.";

                SegundosDesdeCriacao = Cobranca.Calendario.Expiracao;

                SegundosAteExpiracaoEmString = "EXPIRADO";

                timerExpiracaoQrCode.Stop();
                timerExpiracaoQrCode.Dispose();
            }
            else
            {
                SegundosDesdeCriacao = (e.SignalTime - Cobranca.Calendario.CriacaoLocalTime).TotalSeconds;
                double segundosAteExpiracao = Cobranca.Calendario.Expiracao - SegundosDesdeCriacao;
                TimeSpan timeSpan = new TimeSpan(0, 0, (int)segundosAteExpiracao);
                SegundosAteExpiracaoEmString = timeSpan.ToString(@"mm\:ss");
            }
        }

        private bool IsQRCodeValido(object arg)
        {
            return VisibilidadeImagemQRCode == Visibility.Visible;
        }

        private void ImprimirQRCode(object obj)
        {
            //throw new NotImplementedException();
        }

        private async void GeraESalvaQrCode()
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));

            var paramQRCode = new
            {
                id = Cobranca.Loc.Id
            };

            if (Cobranca.QrCode == null)
            {
                try
                {
                    var qrCodeGn = endpoints.PixGenerateQRCode(paramQRCode);

                    QRCode qrCode = new QRCode();

                    // Generate QRCode Image to JPEG Format
                    JObject qrCodeJson = JObject.Parse(qrCodeGn);

                    qrCode.Qrcode = (string)qrCodeJson["qrcode"];
                    qrCode.ImagemQrcode = ((string)qrCodeJson["imagemQrcode"]).Replace("data:image/png;base64,", "");

                    Cobranca.QrCode = qrCode;

                    var result = await daoCobranca.Atualizar(Cobranca);

                    if (result)
                    {
                        PopulaDados();
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
                PopulaDados();
            }
        }

        private void PopulaDados()
        {
            var dados = JObject.Parse(File.ReadAllText("dados_recebedor.json"));
            ImagemQrCode = Cobranca.QrCode.QrCodeBitmap;
            Fantasia = (string)dados["fantasia"];
            Razao = (string)dados["razaosocial"];
            Cnpj = (string)dados["cnpj"];
            Instituicao = (string)dados["instituicao"];
            Valor = Cobranca.Valor.Original;
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

        public Visibility VisibilidadeLabelQRCode
        {
            get
            {
                return _visibilidadeLabelQRCode;
            }

            set
            {
                _visibilidadeLabelQRCode = value;
                OnPropertyChanged("VisibilidadeLabelQRCode");
            }
        }

        public Visibility VisibilidadeImagemQRCode
        {
            get
            {
                return _visibilidadeImagemQRCode;
            }

            set
            {
                _visibilidadeImagemQRCode = value;
                OnPropertyChanged("VisibilidadeImagemQRCode");
            }
        }

        public Cobranca Cobranca
        {
            get
            {
                return _cobranca;
            }

            set
            {
                _cobranca = value;
                OnPropertyChanged("Cobranca");
            }
        }

        public double SegundosDesdeCriacao
        {
            get
            {
                return _segundosDesdeCriacao;
            }

            set
            {
                _segundosDesdeCriacao = value;
                OnPropertyChanged("SegundosDesdeCriacao");
            }
        }

        public string LabelQRCode1
        {
            get
            {
                return _labelQRCode1;
            }

            set
            {
                _labelQRCode1 = value;
                OnPropertyChanged("LabelQRCode1");
            }
        }

        public string LabelQRCode2
        {
            get
            {
                return _labelQRCode2;
            }

            set
            {
                _labelQRCode2 = value;
                OnPropertyChanged("LabelQRCode2");
            }
        }

        public string SegundosAteExpiracaoEmString
        {
            get
            {
                return _segundosAteExpiracaoEmString;
            }

            set
            {
                _segundosAteExpiracaoEmString = value;
                OnPropertyChanged("SegundosAteExpiracaoEmString");
            }
        }

        public void OnClosing()
        {
            if (_closeableOwner != null)
                _closeableOwner.Close();
        }
    }
}
