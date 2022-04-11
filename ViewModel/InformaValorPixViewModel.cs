using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VMIClientePix.BancoDeDados.ConnectionFactory;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
using VMIClientePix.Util;
using VMIClientePix.View;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Concretos;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class InformaValorPixViewModel : ObservableObject, IOnClosing
    {
        public ICommand GerarQRCodeComando { get; set; }
        private double _valorPix;
        private IMessageBoxService messageBoxService;
        private DAOCobranca daoCobranca;
        private ISession session;

        public InformaValorPixViewModel(IMessageBoxService messageBoxService)
        {
            IniciaSessionEDAO();
            GerarQRCodeComando = new RelayCommand(GerarQRCode);
            this.messageBoxService = messageBoxService;
        }

        private void IniciaSessionEDAO()
        {
            SessionProvider.FechaSession(session);
            session = SessionProvider.GetSession();
            daoCobranca = new DAOCobranca(session);
        }

        private async void GerarQRCode(object obj)
        {
            if (ValorPix == 0)
            {
                messageBoxService.Show("Valor De Cobrança Pix Não Pode Ser Zero!", "Informe O Valor Do Pix", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                var gnEndPoints = Credentials.GNEndpoints();

                if (gnEndPoints == null)
                {
                    messageBoxService.Show($"Erro ao recuperar credenciais da GerenciaNet.\nAcesse {Log.LogCredenciais} para mais detalhes.", "Erro em Credenciais GerenciaNet", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dynamic endpoints = new Endpoints(gnEndPoints);
                var dados = JObject.Parse(File.ReadAllText("dados_recebedor.json"));

                var body = new
                {
                    calendario = new
                    {
                        expiracao = 150 //2 e meio minutos
                    },
                    valor = new
                    {
                        original = ValorPix.ToString("F2", CultureInfo.InvariantCulture)
                    },
                    chave = (string)dados["chave"]
                };

                var cobrancaPix = endpoints.PixCreateImmediateCharge(null, body);
                Cobranca cobranca = JsonConvert.DeserializeObject<Cobranca>(cobrancaPix);

                try
                {
                    await daoCobranca.Inserir(cobranca);
                    await daoCobranca.RefreshEntidade(cobranca);
                    ComunicaoPelaRede.NotificaListarCobrancas(cobranca.Txid);
                    ApresentaQRCodeEDadosViewModel dadosPixViewModel = new ApresentaQRCodeEDadosViewModel(cobranca.Txid, new MessageBoxService(), (ICloseable)obj);
                    ApresentaQRCodeEDados view = new ApresentaQRCodeEDados()
                    {
                        DataContext = dadosPixViewModel
                    };
                    view.ShowDialog();
                }
                catch (Exception ex)
                {
                    messageBoxService.Show(ex.Message, "Informe O Valor Do Pix", MessageBoxButton.OK, MessageBoxImage.Error);
                    IniciaSessionEDAO();
                }
            }
            catch (GnException e)
            {
                Log.EscreveLogGn(e);
                messageBoxService.Show($"Erro ao criar cobrança Pix na GerenciaNet.\nAcesse {Log.LogGn} para mais detalhes.", "Erro ao criar cobrança", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                messageBoxService.Show($"Não Foi Possível Criar Cobrança Pix. Cheque Sua Conexão Com A Internet.\n\n{ex.GetType().Name}\n{ex.Message}\n{ex.ToString()}", "Criar Cobrança Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public void OnClosing()
        {
            SessionProvider.FechaSession(session);
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
