﻿using ACBrLib.PosPrinter;
using Efipay;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using VMIClientePix.BancoDeDados.ConnectionFactory;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
using VMIClientePix.Util;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Concretos;

namespace VMIClientePix.ViewModel
{
    public class MainWindowViewModel : ObservableObject, IOnClosing
    {
        private MessageBoxService messageBoxService;
        private ObservableCollection<Cobranca> _cobrancas = new ObservableCollection<Cobranca>();
        private ObservableCollection<Pix> _listaPix = new ObservableCollection<Pix>();
        private DAOCobranca daoCobranca;
        private DAOPix daoPix;
        private ISession session;
        private double _totalCobrancas;
        private double _totalTransferencias;
        private ACBrPosPrinter posPrinter;
        private OpenView openView;
        private Timer timerConsulta;
        public ICommand CriarCobrancaPixComando { get; set; }
        public ICommand ListViewCobrancaLeftMouseClickComando { get; set; }
        public ICommand ListViewPixLeftMouseClickComando { get; set; }
        public ICommand ImprimirRelatorioPixComando { get; set; }
        public ICommand AtualizarListaComando { get; set; }
        public ICommand AbrirConfigImpressoraComando { get; set; }
        public ICommand ConfigCredenciaisComando { get; set; }
        public ICommand AbrirConfigAppComando { get; set; }
        public ICommand ConsultarRecebimentoPixComando { get; set; }

        public MainWindowViewModel()
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
#endif
            openView = new OpenView();

            var splashScreenVM = new VMISplashScreenViewModel();
            openView.Show(splashScreenVM);

            Directory.CreateDirectory("Logs");

            messageBoxService = new MessageBoxService();
            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);
            ListViewCobrancaLeftMouseClickComando = new RelayCommand(ListViewLeftMouseClick);
            ListViewPixLeftMouseClickComando = new RelayCommand(ListViewPixLeftMouseClick);
            ImprimirRelatorioPixComando = new RelayCommand(ImprimirRelatorioPix);
            AtualizarListaComando = new RelayCommand(AtualizarLista);
            AbrirConfigImpressoraComando = new RelayCommand(AbrirConfigImpressora);
            ConfigCredenciaisComando = new RelayCommand(ConfigCredenciais);
            AbrirConfigAppComando = new RelayCommand(AbrirConfigApp);
            ConsultarRecebimentoPixComando = new RelayCommand(ConsultarRecebimentoPix);

            PropertyChanged += MainWindowViewModel_PropertyChanged;

            JObject configApp = null;

            try
            {
                configApp = JObject.Parse(ArquivosApp.GetConfig());
            }
            catch (Exception ex)
            {
                messageBoxService.Show($"Erro ao abrir arquivo de configurações da aplicação. Algumas funções não funcionam sem este arquivo. Certifique-se que a aplicação foi configurada.\n\n{ex.Message}",
                    "Abrir Configurações De Aplicação",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            try
            {
                SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
                IniciaSessionEDAO();
                AtualizarCobrancasPelaGN(false);
                AtualizarListaPixPelaEfi(false);

                timerConsulta = new Timer();
                timerConsulta.Elapsed += TimerConsulta_Elapsed;
                timerConsulta.Start();
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "criar session factory");
                messageBoxService.Show($"Erro Ao Conectar Banco de Dados Local\nAcesse {Log.LogLocal} para mais detalhes.");
            }

            splashScreenVM.CloseView();
        }

        private async void TimerConsulta_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerConsulta.Stop();
            await daoCobranca.RefreshEntidade(Cobrancas);
            ListarCobrancas();
            ListarPix();

            if (timerConsulta.Interval == 100)
            {
                timerConsulta.Interval = 5000;
            }

            timerConsulta.Start();
        }

        private async void ComunicaoPelaRede_AposReceberComandoListar(object sender, EventArgs e)
        {
            try
            {
                var cobs = await daoCobranca.ListarPorDia(DateTime.Now);

                if (cobs != null)
                {
                    if (cobs.Count > 0)
                        await daoCobranca.RefreshEntidade(cobs.Where(w => w.Status.Equals("ATIVA")).ToList());
                    Cobrancas = new ObservableCollection<Cobranca>(cobs);
                }
            }
            catch (Exception ex)
            {
                messageBoxService.Show(ex.Message, "Erro ao listar cobranças após comando", MessageBoxButton.OK, MessageBoxImage.Error);
                IniciaSessionEDAO();
            }

            ListarPix();
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Cobrancas":
                    if (Cobrancas != null && Cobrancas.Count > 0)
                        TotalCobrancas = Cobrancas.Where(w => w.Status.Equals("CONCLUIDA")).Sum(s => s.Valor.Original);
                    else
                        TotalCobrancas = 0;
                    break;
                case "ListaPix":
                    if (ListaPix != null && ListaPix.Count > 0)
                        TotalTransferencias = ListaPix.Sum(s => s.Valor);
                    else
                        TotalTransferencias = 0;
                    break;
            }
        }

        private void ImprimirRelatorioPix(object obj)
        {
            //Atualiza listas caso estejam desatualizadas
            //Ao mesmo tempo atualiza campos de total
            AtualizarCobrancasPelaGN(false);
            AtualizarListaPixPelaEfi(false);
            ListarCobrancas();
            ListarPix();

            posPrinter = new ACBrPosPrinter();
            ConfiguraPosPrinter.Configurar(posPrinter);

            int numCobs = Cobrancas.Where(w => w.Status.Equals("CONCLUIDA")).ToList().Count;
            int colunas = 48;
            int numEspacos = 0;

            string s = "</zera>" + "\n";
            s += "</ce>" + "\n";
            s += "</logo>" + "\n";
            s += "<e>RELATÓRIO PIX</e>" + "\n";
            s += "</ae>" + "\n";
            s += "<n>DATA</n>" + "\n";
            s += $"</fa>{DateTime.Now.ToString("d", CultureInfo.CurrentCulture)}" + "\n";
            s += "</pular_linhas>\n";
            s += $"<e><n>COBRANÇAS</n></e>" + "\n";

            if (numCobs > 0)
            {
                string horario = "Horário";
                string valor = "Valor";
                numEspacos = colunas - horario.Length - valor.Length;
                s += string.Concat("<n>", horario, "</n>", new string(' ', numEspacos), "<n>", valor, "</n>", "\n");

                foreach (var c in Cobrancas.Where(w => w.Status.Equals("CONCLUIDA")))
                {
                    string data = c.Pix[0].HorarioLocalTime.ToString("T", CultureInfo.CurrentCulture);
                    string valorCob = c.Valor.Original.ToString("C", CultureInfo.CurrentCulture);
                    numEspacos = colunas - data.Length - valorCob.Length;
                    s += string.Concat("</fn>", data, new string(' ', numEspacos), valorCob, "\n");
                }

                s += $"</ad><n>Total: {TotalCobrancas.ToString("C", CultureInfo.CurrentCulture)}</n>";
            }
            else
            {
                s += "</fn>Não Houve Cobranças No Dia\n";
            }

            s += "</pular_linhas>\n";
            s += "</ae>";
            s += "<e><n>TRANSFERÊNCIAS</n></e>\n";

            if (ListaPix.Count > 0)
            {
                string horario = "Horário";
                string valor = "Valor";
                numEspacos = colunas - horario.Length - valor.Length;
                s += string.Concat("<n>", horario, "</n>", new string(' ', numEspacos), "<n>", valor, "</n>", "\n");

                foreach (var p in ListaPix)
                {
                    string data = p.HorarioLocalTime.ToString("T", CultureInfo.CurrentCulture);
                    string valorPix = p.Valor.ToString("C", CultureInfo.CurrentCulture);
                    numEspacos = colunas - data.Length - valorPix.Length;
                    s += string.Concat("</fn>", data, new string(' ', numEspacos), valorPix, "\n");
                }

                s += $"</ad><n>Total: {TotalTransferencias.ToString("C", CultureInfo.CurrentCulture)}</n>";
            }
            else
            {
                s += "</fn>Não Houve Transferências No Dia\n";
            }

            s += "</pular_linhas>\n";

            s += $"</ad><e><n>TOTAL GERAL: {(TotalCobrancas + TotalTransferencias).ToString("C", CultureInfo.CurrentCulture)}</n></e>\n";
            s += "</pular_linhas>\n";
            s += "</pular_linhas>\n";
            s += "</corte_total>\n";

            try
            {
                posPrinter.Imprimir(s);
                if (posPrinter != null)
                    posPrinter.Dispose();
            }
            catch (Exception ex)
            {
                messageBoxService.Show("Erro ao imprimir relatório Pix. Cheque se a impressora está conectada corretamente e que está ligada.\n\n" + ex.Message, "Impressão De QR Code Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ListViewPixLeftMouseClick(object obj)
        {
            if (obj != null)
            {
                ApresentaDadosTransfPixViewModel viewModel = new ApresentaDadosTransfPixViewModel(((Pix)obj).EndToEndId, new MessageBoxService());
                openView.ShowDialog(viewModel);
            }
        }

        private void ConsultarRecebimentoPix(object obj)
        {
            AtualizarListaPixPelaEfi(true);
            ListarPix();
        }

        /// <summary>
        /// Cria nova session e (re)cria DAOs
        /// </summary>
        private void IniciaSessionEDAO()
        {
            SessionProvider.FechaSession(session);
            session = SessionProvider.GetSession();
            daoCobranca = new DAOCobranca(session);
            daoPix = new DAOPix(session);
        }

        private void AbrirConfigApp(object obj)
        {
            ConfiguracaoAplicacaoViewModel viewModel = new ConfiguracaoAplicacaoViewModel(messageBoxService);
            openView.ShowDialog(viewModel);
        }


        private void ConfigCredenciais(object obj)
        {
            ConfiguraCredenciaisViewModel viewModel = new ConfiguraCredenciaisViewModel();
            openView.ShowDialog(viewModel);
        }

        private void AbrirConfigImpressora(object obj)
        {
            ConfiguracoesImpressoraViewModel viewModel = new ConfiguracoesImpressoraViewModel(messageBoxService);
            openView.ShowDialog(viewModel);
        }

        private async void ListarCobrancas()
        {
            try
            {
                var cobs = await daoCobranca.ListarPorDia(DateTime.Now);

                if (cobs != null)
                {
                    Cobrancas = new ObservableCollection<Cobranca>(cobs);
                }
            }
            catch (Exception ex)
            {
                messageBoxService.Show(ex.Message, "Erro ao listar cobranças", MessageBoxButton.OK, MessageBoxImage.Error);
                IniciaSessionEDAO();
            }
        }

        private async void ListarPix()
        {
            try
            {
                var dados = JObject.Parse(ArquivosApp.GetDadosRecebedor());
                var pix = await daoPix.ListarPorDiaPorChave(DateTime.Now, (string)dados["chave_estatica"]);

                if (pix != null)
                {
                    ListaPix = new ObservableCollection<Pix>(pix);
                }
            }
            catch (Exception ex)
            {
                messageBoxService.Show(ex.Message, "Erro ao listar pix", MessageBoxButton.OK, MessageBoxImage.Error);
                IniciaSessionEDAO();
            }
        }

        /// <summary>
        /// Consulta A API da Efi SA para retornar os Pix do dia atual
        /// </summary>
        /// <param name="throwEx">Determina se eventuais exceções devem mostrar aviso ao usuário ou se serão tratadas silenciosamente</param>
        private async void AtualizarListaPixPelaEfi(bool throwEx)
        {
            var gnEndPoints = Credentials.EfiEndpoints();
            var dados = JObject.Parse(ArquivosApp.GetDadosRecebedor());

            if (gnEndPoints == null)
            {
                messageBoxService.Show($"Erro ao recuperar credenciais da GerenciaNet.\nAcesse {Log.LogCredenciais} para mais detalhes.", "Erro em Credenciais GerenciaNet", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dynamic endpoints = new EfiPay(gnEndPoints);

            var param = new
            {
                inicio = JsonConvert.SerializeObject(DateTime.Now.Date.ToUniversalTime()).Replace("\"", ""),
                fim = JsonConvert.SerializeObject(DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToUniversalTime()).Replace("\"", "")
            };

            try
            {
                var response = endpoints.PixReceivedList(param);
                ListaPixs listaPixs = JsonConvert.DeserializeObject<ListaPixs>(response);
                IList<Pix> pixAtt = new List<Pix>();

                foreach (var pixConsulta in listaPixs.Pixs.Where(w => w.Chave == null || w.Chave.ToLower().Equals((string)dados["chave_estatica"])))
                {
                    var pixLocal = await daoPix.ListarPorId(pixConsulta.EndToEndId);
                    if (pixLocal != null) continue; //Não insere pix que já existem no banco local
                    pixAtt.Add(pixConsulta); //Novo pix para inserir no banco
                }

                if (pixAtt.Count > 0)
                {
                    try
                    {
                        await daoPix.Inserir(pixAtt);
                    }
                    catch (Exception ex)
                    {
                        messageBoxService.Show(ex.Message, "Erro ao inserir/listar pix", MessageBoxButton.OK, MessageBoxImage.Error);
                        IniciaSessionEDAO();
                    }
                }
            }
            catch (EfiException e)
            {
                Log.EscreveLogEfi(e);
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar pix da instituição GerenciaNet.\n\nAcesse {Log.LogGn} para mais detalhes.", "Erro ao consultar pix", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (JsonReaderException jex)
            {
                Log.EscreveExceptionGenerica(jex);
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar pix da instituição GerenciaNet. Cheque se está conectado a internet.\n\n{jex.Message}", "Erro ao Consultar Pix", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Log.EscreveExceptionGenerica(ex);
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar pix da instituição GerenciaNet. Cheque se está conectado a internet.\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Erro ao Consultar Pix", MessageBoxButton.OK, MessageBoxImage.Error);
                    IniciaSessionEDAO();
                }
            }
        }

        /// <summary>
        /// Consulta A API da GerenciaNet para retornar as cobranças do dia atual com seus respectivos status
        /// </summary>
        /// <param name="throwEx">Determina se eventuais exceções devem mostrar aviso ao usuário ou se serão tratadas silenciosamente</param>
        private async void AtualizarCobrancasPelaGN(bool throwEx)
        {
            var gnEndPoints = Credentials.EfiEndpoints();

            if (gnEndPoints == null)
            {
                messageBoxService.Show($"Erro ao recuperar credenciais da GerenciaNet.\nAcesse {Log.LogCredenciais} para mais detalhes.", "Erro em Credenciais GerenciaNet", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dynamic endpoints = new EfiPay(gnEndPoints);

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

                foreach (var cobrancaConsulta in listaCobranca.Cobrancas)
                {
                    var cobrancaLocal = await daoCobranca.ListarPorId(cobrancaConsulta.Txid);

                    //Cobrança já existe no banco local
                    if (cobrancaLocal != null)
                    {
                        cobrancaLocal.Pix.Clear();
                        foreach (var p in cobrancaConsulta.Pix)
                        {
                            var pixLocal = await daoPix.ListarPorId(p.EndToEndId);

                            if (pixLocal == null)
                            {
                                p.Cobranca = cobrancaLocal;
                                cobrancaLocal.Pix.Add(p);
                            }
                            else
                            {
                                pixLocal.Cobranca = cobrancaLocal;
                                cobrancaLocal.Pix.Add(pixLocal);
                            }
                        }

                        cobrancaLocal.Revisao = cobrancaConsulta.Revisao;
                        cobrancaLocal.Status = cobrancaConsulta.Status;
                        cobrancaLocal.Chave = cobrancaConsulta.Chave;
                        cobrancaLocal.Location = cobrancaConsulta.Location;

                        if (cobrancaConsulta.Pix.Count > 0)
                            cobrancaLocal.PagoEm = cobrancaConsulta.Pix[0].Horario;

                        cobrancasAtt.Add(cobrancaLocal);
                    }
                    else
                    {
                        foreach (var p in cobrancaConsulta.Pix)
                        {
                            p.Cobranca = cobrancaConsulta;
                        }

                        if (cobrancaConsulta.Pix.Count > 0)
                            cobrancaConsulta.PagoEm = cobrancaConsulta.Pix[0].Horario;

                        cobrancasAtt.Add(cobrancaConsulta);
                    }
                }

                try
                {
                    await daoCobranca.InserirOuAtualizar(cobrancasAtt);
                }
                catch (Exception ex)
                {
                    messageBoxService.Show(ex.Message, "Erro ao inserir ou atualizar cobranças", MessageBoxButton.OK, MessageBoxImage.Error);
                    IniciaSessionEDAO();
                }
            }
            catch (EfiException e)
            {
                Log.EscreveLogEfi(e);
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar cobranças da instituição GerenciaNet.\n\nAcesse {Log.LogGn} para mais detalhes.", "Erro ao consultar cobranças", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (JsonReaderException jex)
            {
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar cobranças da instituição GerenciaNet. Cheque se está conectado a internet.\n\n{jex.Message}", "Erro ao Consultar Cobranças Pix", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                if (!throwEx)
                {
                    messageBoxService.Show($"Erro ao listar cobranças da instituição GerenciaNet. Cheque se está conectado a internet.\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Erro ao Consultar Cobranças Pix", MessageBoxButton.OK, MessageBoxImage.Error);
                    IniciaSessionEDAO();
                }
            }
        }

        private void AtualizarLista(object obj)
        {
            AtualizarCobrancasPelaGN(true);
            ListarCobrancas();
        }

        private async void ListViewLeftMouseClick(object obj)
        {
            if (obj != null)
            {
                ApresentaQRCodeEDadosViewModel viewModel = new ApresentaQRCodeEDadosViewModel(((Cobranca)obj).Txid, new MessageBoxService());
                openView.ShowDialog(viewModel);
                //atualiza cobrança na session
                await daoCobranca.RefreshEntidade((Cobranca)obj);
                ListarCobrancas();
            }
        }

        private async void CriarCobrancaPix(object obj)
        {
            InformaValorPixViewModel viewModel = new InformaValorPixViewModel(new MessageBoxService());
            openView.ShowDialog(viewModel);
            var txid = (viewModel as IReturnData).GetData();
            if (txid != null)
            {
                await daoCobranca.RefreshEntidade(await daoCobranca.ListarPorId(txid));
                ListarCobrancas();
            }
        }

        public void OnClosingFromVM()
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

        public ObservableCollection<Pix> ListaPix
        {
            get
            {
                return _listaPix;
            }

            set
            {
                _listaPix = value;
                OnPropertyChanged("ListaPix");
            }
        }

        public double TotalCobrancas
        {
            get
            {
                return _totalCobrancas;
            }

            set
            {
                _totalCobrancas = value;
                OnPropertyChanged("TotalCobrancas");
            }
        }

        public double TotalTransferencias
        {
            get
            {
                return _totalTransferencias;
            }

            set
            {
                _totalTransferencias = value;
                OnPropertyChanged("TotalTransferencias");
            }
        }

        /// <summary>
        /// Configura o webhook com a chave informada.
        /// Somente irá funcionar caso a chave informada esteja cadastrada na conta usada na aplicação no momento do cadastro.
        /// </summary>
        /// <param name="chave">Chave PIX cadastrada em conta</param>
        public static void ConfigurarWebhookPix(string chave)
        {
            dynamic endpoints = new EfiPay(Credentials.EfiEndpoints());

            var headers = "{\"x-skip-mtls-checking\": \"true\"}";

            var param = new
            {
                chave = chave
            };

            var body = new
            {
                webhookUrl = "https://api.vandamodaintima.com.br/prod/webhook/"
            };

            try
            {
                var response = endpoints.PixConfigWebhook(param, body, headers);
                Console.WriteLine(response);
            }
            catch (EfiException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
        }

        public static void ListarWebhooksCadastrados()
        {
            dynamic endpoints = new EfiPay(Credentials.EfiEndpoints());

            var param = new
            {
                inicio = "2022-01-01T16:01:35Z",
                fim = "2022-06-01T16:01:35Z"
            };

            try
            {
                var response = endpoints.PixListWebhook(param);
                Console.WriteLine(response);
            }
            catch (EfiException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
        }

        public static void DeletarWebhook(string chave)
        {
            dynamic endpoints = new EfiPay(Credentials.EfiEndpoints());

            var param = new
            {
                chave = chave
            };

            try
            {
                var response = endpoints.PixDeleteWebhook(param);
                Console.WriteLine(response);
            }
            catch (EfiException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
            }
        }
    }
}
