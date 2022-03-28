using NHibernate;
using System;
using System.Collections.ObjectModel;
using System.IO;
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
        private HttpListener listener;
        public ICommand CriarCobrancaPixComando { get; set; }
        public ICommand ListViewLeftMouseClickComando { get; set; }
        public ICommand AtualizarListaComando { get; set; }

        public MainWindowViewModel()
        {
            SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
            session = SessionProvider.GetSession();

            daoCobranca = new DAOCobranca(session);

            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);
            ListViewLeftMouseClickComando = new RelayCommand(ListViewLeftMouseClick);
            AtualizarListaComando = new RelayCommand(AtualizarLista);
            messageBoxService = new MessageBoxService();
            ListarCobrancas();

            //listener = new HttpListener();
            //listener.Prefixes.Add("http://*:6569/");
            //listener.Start();
            //var result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            HttpListener httpListener = (HttpListener)ar.AsyncState;
            var context = httpListener.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                var rqstEncoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(request.InputStream, rqstEncoding);
                messageBoxService.Show(reader.ReadToEnd());
                request.InputStream.Close();
                reader.Close();
            }

            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private async void ListarCobrancas()
        {
            Cobrancas = new ObservableCollection<Cobranca>(await daoCobranca.ListarPorDia(DateTime.Now));
        }

        private void AtualizarLista(object obj)
        {
            ListarCobrancas();
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
