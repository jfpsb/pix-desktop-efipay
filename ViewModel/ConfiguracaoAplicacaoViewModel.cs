using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using VMIClientePix.Util;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class ConfiguracaoAplicacaoViewModel : ObservableObject
    {
        private bool _fazBackup;
        private string _fantasia;
        private string _razaoSocial;
        private string _cnpj;
        private string _loja;
        private string _chavePIX;
        private string _chavePIXEstatica;

        private IMessageBoxService messageBox;

        public ICommand SalvaConfigComando { get; set; }

        public ConfiguracaoAplicacaoViewModel(IMessageBoxService messageBoxService)
        {
            messageBox = messageBoxService;
            SalvaConfigComando = new RelayCommand(SalvaConfig);

            if (ArquivosApp.ConfigExists())
            {
                var config = ArquivosApp.GetConfig();
                JObject Jconfig = JObject.Parse(config);

                FazBackup = (bool)Jconfig["fazbackup"];
            }
            else
            {
                FazBackup = true;
            }

            if (ArquivosApp.DadosRecebedorExists())
            {
                var dados = ArquivosApp.GetDadosRecebedor();
                JObject Jdados = JObject.Parse(dados);

                Fantasia = (string)Jdados["fantasia"];
                RazaoSocial = (string)Jdados["razaosocial"];
                Cnpj = (string)Jdados["cnpj"];
                Loja = (string)Jdados["loja"];
                ChavePIX = (string)Jdados["chave"];
                ChavePIXEstatica = (string)Jdados["chave_estatica"];
            }
        }

        private void SalvaConfig(object obj)
        {
            //Salva config.json
            var config = new
            {
                fazbackup = FazBackup
            };

            var configJson = JsonConvert.SerializeObject(config, Formatting.Indented);

            try
            {
                ArquivosApp.WriteConfig(configJson);
                messageBox.Show($"Sucesso ao salvar arquivo de configurações de aplicação.", "Salvar Configurações De Aplicação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                messageBox.Show($"Erro ao salvar arquivo de configurações de aplicação.\n\n{ex.Message}", "Salvar Configurações De Aplicação", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (!string.IsNullOrEmpty(ChavePIX))
            {
                //Salva dados recebedor
                var dados_recebedor = new
                {
                    fantasia = Fantasia,
                    razaosocial = RazaoSocial,
                    cnpj = Cnpj,
                    instituicao = "GERENCIANET",
                    loja = Loja,
                    chave = ChavePIX,
                    chave_estatica = ChavePIXEstatica
                };

                var dadosJson = JsonConvert.SerializeObject(dados_recebedor, Formatting.Indented);

                try
                {
                    ArquivosApp.WriteDadosRecebedor(dadosJson);
                    messageBox.Show($"Sucesso ao salvar dados de recebedor.", "Salvar Configurações De Aplicação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    messageBox.Show($"Erro ao salvar  dados de recebedor.\n\n{ex.Message}", "Salvar Configurações De Aplicação", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

        public string RazaoSocial
        {
            get
            {
                return _razaoSocial;
            }

            set
            {
                _razaoSocial = value;
                OnPropertyChanged("RazaoSocial");
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

        public string Loja
        {
            get
            {
                return _loja;
            }

            set
            {
                _loja = value;
                OnPropertyChanged("Loja");
            }
        }

        public string ChavePIX
        {
            get
            {
                return _chavePIX;
            }

            set
            {
                _chavePIX = value;
                OnPropertyChanged("ChavePIX");
            }
        }

        public bool FazBackup
        {
            get
            {
                return _fazBackup;
            }

            set
            {
                _fazBackup = value;
                OnPropertyChanged("FazBackup");
            }
        }

        public string ChavePIXEstatica
        {
            get
            {
                return _chavePIXEstatica;
            }

            set
            {
                _chavePIXEstatica = value;
                OnPropertyChanged("ChavePIXEstatica");
            }
        }
    }
}
