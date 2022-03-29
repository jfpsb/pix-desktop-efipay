using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
using VMIClientePix.Util;
using VMIClientePix.View;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Services.Concretos;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class InformaValorPixViewModel : ObservableObject
    {
        public ICommand GerarQRCodeComando { get; set; }
        private double _valorPix;
        private IMessageBoxService messageBoxService;
        private DAOCobranca daoCobranca;
        private ISession _session;
        public InformaValorPixViewModel(ISession session, IMessageBoxService messageBoxService)
        {
            _session = session;
            daoCobranca = new DAOCobranca(session);
            GerarQRCodeComando = new RelayCommand(GerarQRCode);
            this.messageBoxService = messageBoxService;
        }

        private async void GerarQRCode(object obj)
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));
            var dados = JObject.Parse(File.ReadAllText("dados_recebedor.json"));

            var body = new
            {
                calendario = new
                {
                    expiracao = 120 //2 minutos
                },
                valor = new
                {
                    original = ValorPix.ToString("F2", CultureInfo.InvariantCulture)
                },
                chave = (string)dados["chave"]
            };

            try
            {
                var cobrancaPix = endpoints.PixCreateImmediateCharge(null, body);
                Cobranca cobranca = JsonConvert.DeserializeObject<Cobranca>(cobrancaPix);

                var result = await daoCobranca.Inserir(cobranca);

                if (result)
                {
                    ApresentaQRCodeEDadosViewModel dadosPixViewModel = new ApresentaQRCodeEDadosViewModel(_session, cobranca, new MessageBoxService(), (ICloseable)obj);
                    ApresentaQRCodeEDados view = new ApresentaQRCodeEDados()
                    {
                        DataContext = dadosPixViewModel
                    };
                    view.ShowDialog();
                }
            }
            catch (GnException e)
            {
                Debug.WriteLine(e.ErrorType);
                Debug.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                messageBoxService.Show($"Não Foi Possível Criar Cobrança Pix. Cheque Sua Conexão Com A Internet.\n\n{ex.Message}", "Criar Cobrança Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
