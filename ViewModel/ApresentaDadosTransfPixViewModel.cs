using ACBrLib.PosPrinter;
using NHibernate;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using VMIClientePix.BancoDeDados.ConnectionFactory;
using VMIClientePix.Model;
using VMIClientePix.Model.DAO;
using VMIClientePix.Util;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class ApresentaDadosTransfPixViewModel : ObservableObject, IOnClosing
    {
        private ISession session;
        private DAOPix daoPix;
        private Pix _pix;
        private ACBrPosPrinter posPrinter;
        private IMessageBoxService _messageBox;

        public ICommand ImprimirComprovanteComando { get; set; }

        public ApresentaDadosTransfPixViewModel(string pixId, IMessageBoxService messageBox)
        {
            _messageBox = messageBox;
            IniciaSessionEDAO();
            GetPix(pixId);

            posPrinter = new ACBrPosPrinter();
            ConfiguraPosPrinter();

            ImprimirComprovanteComando = new RelayCommand(ImprimirComprovante);
        }

        private void ImprimirComprovante(object obj)
        {
            string s = "</zera>" + "\n";
            s += "</ce>" + "\n";
            s += "</logo>" + "\n";
            s += "<e>COMPROVANTE DE TRANSFERÊNCIA PIX</e>" + "\n";
            s += "</ae>" + "\n";
            s += "<n>Chave Utilizada:</n>" + "\n";
            s += $"</fn>{Pix.Chave}" + "\n";

            if (!string.IsNullOrEmpty(Pix.InfoPagador))
            {
                s += "<n>Informações Enviadas Pelo Pagador:</n>" + "\n";
                s += $"</fn>{Pix.InfoPagador.ToUpper()}" + "\n";
            }

            s += "</ae>" + "\n";
            s += $"<a><n><in>VALOR: {Pix.Valor.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"))}</in></n></a>" + "\n";
            s += "</linha_dupla>" + "\n";
            s += "</ce>" + "\n";
            s += $"<a>TRANSFERÊNCIA EFETUADA EM {Pix.HorarioLocalTime.ToString(CultureInfo.CurrentCulture)}" + "\n";
            s += "</corte_total>";

            try
            {
                posPrinter.Imprimir(s);
            }
            catch (Exception ex)
            {
                _messageBox.Show("Erro ao imprimir comprovante. Cheque se a impressora está conectada corretamente e que está ligada.\n\n" + ex.Message, "Impressão De Comprovante Pix", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfiguraPosPrinter()
        {
            try
            {
                posPrinter.ConfigLer();
            }
            catch (Exception ex)
            {
                _messageBox.Show("Erro ao iniciar impressora. Cheque se a impressora está conectada corretamente e que está ligada.\n\n" + ex.Message, "Impressão De Comprovante Pix", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IniciaSessionEDAO()
        {
            session = SessionProvider.GetSession();
            daoPix = new DAOPix(session);
        }

        private async void GetPix(string pixId)
        {
            Pix = await daoPix.ListarPorId(pixId);
        }

        public void OnClosing()
        {
            SessionProvider.FechaSession(session);

            if (posPrinter != null)
                posPrinter.Dispose();
        }

        public Pix Pix
        {
            get
            {
                return _pix;
            }

            set
            {
                _pix = value;
                OnPropertyChanged("Pix");
            }
        }
    }
}
