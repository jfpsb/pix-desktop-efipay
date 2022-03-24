using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VMIClientePix.Model;
using VMIClientePix.Util;
using VMIClientePix.View;
using VMIClientePix.ViewModel.Services.Concretos;

namespace VMIClientePix.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private MessageBoxService messageBoxService;
        private ObservableCollection<Cobranca> _cobrancas = new ObservableCollection<Cobranca>();
        private ListaCobrancas _listaCobranca;
        public ICommand CriarCobrancaPixComando { get; set; }
        public ICommand ListViewLeftMouseClickComando { get; set; }
        public ICommand AtualizarListaComando { get; set; }

        public MainWindowViewModel()
        {
            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);
            ListViewLeftMouseClickComando = new RelayCommand(ListViewLeftMouseClick);
            AtualizarListaComando = new RelayCommand(AtualizarLista);
            messageBoxService = new MessageBoxService();
            ListarCobrancas();
        }

        private void AtualizarLista(object obj)
        {
            ListarCobrancas();
        }

        private void ListViewLeftMouseClick(object obj)
        {
            ApresentaQRCodeEDadosViewModel viewModel = new ApresentaQRCodeEDadosViewModel((Cobranca)obj, new MessageBoxService());
            ApresentaQRCodeEDados view = new ApresentaQRCodeEDados() { DataContext = viewModel };
            view.ShowDialog();
            ListarCobrancas();
        }

        private void CriarCobrancaPix(object obj)
        {
            InformaValorPixViewModel viewModel = new InformaValorPixViewModel(new MessageBoxService());
            InformaValorPix view = new InformaValorPix() { DataContext = viewModel };
            view.ShowDialog();
            ListarCobrancas();
        }

        private void ListarCobrancas()
        {
            var today = DateTime.Now;
            var inicio = JsonConvert.SerializeObject(today.Date.AddDays(-100).ToUniversalTime()).Replace("\"", "");
            var fim = JsonConvert.SerializeObject(today.Date.AddDays(1).AddSeconds(-1).ToUniversalTime()).Replace("\"", "");

            dynamic endpoints = new Endpoints(JObject.Parse(File.ReadAllText("credentials.json")));

            var param = new
            {
                inicio = inicio,
                fim = fim
            };

            try
            {
                var listagemCobrancas = endpoints.PixListCharges(param);

                ListaCobranca = JsonConvert.DeserializeObject<ListaCobrancas>(listagemCobrancas);

                if (ListaCobranca != null)
                {
                    Cobrancas = new ObservableCollection<Cobranca>(ListaCobranca.Cobrancas.OrderBy(o => o.Calendario.Criacao));
                }
            }
            catch (GnException e)
            {
                messageBoxService.Show($"Não Foi Possível Listar As Cobranças Pix. Cheque Sua Conexão Com A Internet.\n\n{e.ErrorType}\n\n{e.Message}", "Listagem De Cobranças Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Debug.WriteLine(e.ErrorType);
                Debug.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                messageBoxService.Show($"Não Foi Possível Listar As Cobranças Pix. Cheque Sua Conexão Com A Internet.\n\n{ex.Message}", "Listagem De Cobranças Pix", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
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

        public ListaCobrancas ListaCobranca
        {
            get
            {
                return _listaCobranca;
            }

            set
            {
                _listaCobranca = value;
                OnPropertyChanged("ListaCobranca");
            }
        }
    }
}
