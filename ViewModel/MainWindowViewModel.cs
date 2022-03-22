using Gerencianet.NETCore.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMIClientePix.Model;
using VMIClientePix.Util;
using VMIClientePix.View;

namespace VMIClientePix.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Cobranca> _cobrancas = new ObservableCollection<Cobranca>();
        private ListaCobrancas _listaCobranca;
        public ICommand CriarCobrancaPixComando { get; set; }

        public MainWindowViewModel()
        {
            CriarCobrancaPixComando = new RelayCommand(CriarCobrancaPix);

            ListarCobrancas();
        }

        private void CriarCobrancaPix(object obj)
        {
            InformaValorPixViewModel viewModel = new InformaValorPixViewModel();
            InformaValorPix view = new InformaValorPix() { DataContext = viewModel };
            view.ShowDialog();

            //TODO:Atualizar listas de pix ao fechar
        }

        private void ListarCobrancas()
        {
            var today = DateTime.Now.AddDays(-1);
            var inicio = JsonConvert.SerializeObject(today.Date.ToUniversalTime()).Replace("\"", "");
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
                    Cobrancas = new ObservableCollection<Cobranca>(ListaCobranca.Cobrancas);
                }
            }
            catch (GnException e)
            {
                Console.WriteLine(e.ErrorType);
                Console.WriteLine(e.Message);
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
