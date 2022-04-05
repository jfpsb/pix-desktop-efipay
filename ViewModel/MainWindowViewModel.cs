using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using VMIClientePix.BancoDeDados.BackupRemoto;
using VMIClientePix.BancoDeDados.ConnectionFactory;
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
        private Timer timerSync;
        public ICommand CriarCobrancaPixComando { get; set; }
        public ICommand ListViewLeftMouseClickComando { get; set; }
        public ICommand AtualizarListaComando { get; set; }
        public ICommand AbrirConfigImpressoraComando { get; set; }
        public ICommand ConfigCredenciaisComando { get; set; }

        public MainWindowViewModel()
        {
            SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
            session = SessionProvider.GetSession();

            daoCobranca = new DAOCobranca(session);

            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);
            ListViewLeftMouseClickComando = new RelayCommand(ListViewLeftMouseClick);
            AtualizarListaComando = new RelayCommand(AtualizarLista);
            AbrirConfigImpressoraComando = new RelayCommand(AbrirConfigImpressora);
            ConfigCredenciaisComando = new RelayCommand(ConfigCredenciais);
            messageBoxService = new MessageBoxService();
            ListarCobrancas();

            timerSync = new Timer(); //Inicia timer de imediato e dentro do timer configuro para rodar de 1 em 1 minuto
            timerSync.Elapsed += TimerSync_Elapsed;
            timerSync.AutoReset = false;
            timerSync.Enabled = true;
        }

        private async void TimerSync_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"INICIO SYNC em {e.SignalTime}");
            timerSync.Stop();
            try
            {
                if (SessionProviderBackup.BackupSessionFactory == null)
                    SessionProviderBackup.BackupSessionFactory = SessionProviderBackup.BuildSessionFactory();

                File.AppendAllText("SyncLog.txt", $"\nData/Hora: {DateTime.Now}\nSessionFactory De Backup Iniciada Com Sucesso.");
            }
            catch (Exception ex)
            {
                File.AppendAllText("SyncLog.txt", $"\nOperação: CRIAÇÃO DE SESSION FACTORY BACKUP\nData/Hora: {DateTime.Now}\n{ex.Message}");
            }

            if (SessionProviderBackup.BackupSessionFactory != null)
            {
                await Sync.Sincronizar<Calendario>();
                await Sync.Sincronizar<Valor>();
                await Sync.Sincronizar<Loc>();
                await Sync.Sincronizar<QRCode>();
                await Sync.Sincronizar<Cobranca>();
                await Sync.Sincronizar<Pagador>();
                await Sync.Sincronizar<Pix>();
                await Sync.Sincronizar<Horario>();
                await Sync.Sincronizar<Devolucao>();
            }

            if (timerSync.Interval == 100) //Intervalo padrão
            {
                timerSync.Stop();
                timerSync.Interval = 60000; //Configura para 1 minuto
                timerSync.AutoReset = true;
                timerSync.Start();
            }

            Console.WriteLine($"FIM SYNC em {DateTime.Now}");
            timerSync.Start();
        }

        private void ConfigCredenciais(object obj)
        {
            ConfiguraCredenciaisViewModel viewModel = new ConfiguraCredenciaisViewModel();
            ConfigurarCredenciais view = new ConfigurarCredenciais() { DataContext = viewModel };
            view.ShowDialog();
        }

        private void AbrirConfigImpressora(object obj)
        {
            ConfiguracoesImpressoraViewModel viewModel = new ConfiguracoesImpressoraViewModel(messageBoxService);
            ConfiguracaoImpressora view = new ConfiguracaoImpressora() { DataContext = viewModel };
            view.ShowDialog();
        }

        private async void ListarCobrancas()
        {
            Cobrancas = new ObservableCollection<Cobranca>(await daoCobranca.ListarPorDia(DateTime.Now));
        }

        private async void AtualizarCobrancasPelaGN()
        {
            dynamic endpoints = new Endpoints(Credentials.GNEndpoints());

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
            if (obj != null)
            {
                ApresentaQRCodeEDadosViewModel viewModel = new ApresentaQRCodeEDadosViewModel(session, (Cobranca)obj, new MessageBoxService());
                ApresentaQRCodeEDados view = new ApresentaQRCodeEDados() { DataContext = viewModel };
                view.ShowDialog();
                ListarCobrancas();
            }
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
            SessionProviderBackup.FechaSessionFactory();

            timerSync.Stop();
            timerSync.Dispose();
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
