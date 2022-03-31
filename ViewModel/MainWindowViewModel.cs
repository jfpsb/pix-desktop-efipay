using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Input;
using VandaModaIntimaWpf.BancoDeDados.ConnectionFactory;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
using VMIClientePix.Util;
using VMIClientePix.View;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Concretos;

namespace VMIClientePix.ViewModel
{
    public class MainWindowViewModel : ObservableObject, IOnClosing
    {
        private MessageBoxService messageBoxService;
        private ObservableCollection<Cobranca> _cobrancas = new ObservableCollection<Cobranca>();
        private DAOCobranca daoCobranca;
        private ISession session;
        public ICommand CriarCobrancaPixComando { get; set; }
        public ICommand ListViewLeftMouseClickComando { get; set; }
        public ICommand AtualizarListaComando { get; set; }
        public ICommand AbrirConfigImpressoraComando { get; set; }

        public MainWindowViewModel()
        {
            SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
            session = SessionProvider.GetSession();

            daoCobranca = new DAOCobranca(session);

            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);
            ListViewLeftMouseClickComando = new RelayCommand(ListViewLeftMouseClick);
            AtualizarListaComando = new RelayCommand(AtualizarLista);
            AbrirConfigImpressoraComando = new RelayCommand(AbrirConfigImpressora);
            messageBoxService = new MessageBoxService();
            ListarCobrancas();
        }

        private void AbrirConfigImpressora(object obj)
        {
            ConfiguracaoImpressora view = new ConfiguracaoImpressora();
            view.ShowDialog();
        }

        private async void ListarCobrancas()
        {
            Cobrancas = new ObservableCollection<Cobranca>(await daoCobranca.ListarPorDia(DateTime.Now));
        }

        private async void AtualizarCobrancasPelaGN()
        {
            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));

            var param = new
            {
                inicio = JsonConvert.SerializeObject(DateTime.Now.Date.ToUniversalTime()).Replace("\"", ""),
                fim = JsonConvert.SerializeObject(DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToUniversalTime()).Replace("\"", "")
            };

            try
            {
                var response = endpoints.PixListCharges(param);
                ListaCobrancas listaCobranca = JsonConvert.DeserializeObject<ListaCobrancas>(response);
                IList<Cobranca> cobrancasAtt = new List<Cobranca>();

                foreach (var cobranca in listaCobranca.Cobrancas)
                {
                    var cobrancaLocal = Cobrancas.Where(w => w.Txid == cobranca.Txid).FirstOrDefault(); //Cobrança salva no banco de dados local
                    if (cobrancaLocal == null) continue;
                    cobranca.Calendario = cobrancaLocal.Calendario;
                    cobranca.Valor = cobrancaLocal.Valor;
                    cobranca.Loc = cobrancaLocal.Loc;
                    cobranca.QrCode = cobrancaLocal.QrCode;
                    foreach (var p in cobranca.Pix)
                    {
                        p.Cobranca = cobranca;
                    }
                    if (cobranca.Pix.Count > 0)
                        cobranca.PagoEm = cobranca.Pix[0].Horario;

                    cobrancasAtt.Add(cobranca);
                }

                session.Clear();
                var result = await daoCobranca.InserirOuAtualizar(cobrancasAtt);

                if (result)
                    ListarCobrancas();
            }
            catch (GnException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
        }

        private void AtualizarLista(object obj)
        {
            AtualizarCobrancasPelaGN();
        }

        private void ListViewLeftMouseClick(object obj)
        {
            ApresentaQRCodeEDadosViewModel viewModel = new ApresentaQRCodeEDadosViewModel(session, (Cobranca)obj, new MessageBoxService());
            ApresentaQRCodeEDados view = new ApresentaQRCodeEDados() { DataContext = viewModel };
            view.ShowDialog();
            ListarCobrancas();
        }

        private void CriarCobrancaPix(object obj)
        {
            InformaValorPixViewModel viewModel = new InformaValorPixViewModel(session, new MessageBoxService());
            InformaValorPix view = new InformaValorPix() { DataContext = viewModel };
            view.ShowDialog();
            ListarCobrancas();
        }

        public void OnClosing()
        {
            SessionProvider.FechaSession(session);
            SessionProvider.FechaSessionFactory();
        }

        public ObservableCollection<Cobranca> Cobrancas
        {
            get
            {
                return _cobrancas;
            }

            set
            {
                _cobrancas = value;
                OnPropertyChanged("Cobrancas");
            }
        }
    }
}
