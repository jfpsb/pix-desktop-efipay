using ACBrLib.Core.PosPrinter;
using ACBrLib.PosPrinter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VMIClientePix.Util;
using VMIClientePix.ViewModel.Interfaces;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class ConfiguracoesImpressoraViewModel : ObservableObject, IOnClosing
    {
        private IEnumerable<ACBrPosPrinterModelo> _modelosEnum;
        private IList<string> _portas;
        private string _porta;
        private ACBrPosPrinterModelo _modeloSelecionado;
        private ACBrPosPrinter posPrinter;
        private IMessageBoxService _messageBox;

        public ICommand AtualizarPortasComando { get; set; }
        public ICommand SalvarConfigComando { get; set; }

        public ConfiguracoesImpressoraViewModel(IMessageBoxService messageBox)
        {
            _messageBox = messageBox;
            ModelosEnum = Enum.GetValues(typeof(ACBrPosPrinterModelo)).Cast<ACBrPosPrinterModelo>();
            AtualizarPortasComando = new RelayCommand(AtualizarPortas);
            SalvarConfigComando = new RelayCommand(SalvarConfig);
            posPrinter = new ACBrPosPrinter();
            posPrinter.ConfigLer();

            ModeloSelecionado = posPrinter.Config.Modelo;
            ChamaAcharPortas();
        }

        private void SalvarConfig(object obj)
        {
            posPrinter.Config.Modelo = ModeloSelecionado;
            posPrinter.Config.Porta = Porta;

            try
            {
                posPrinter.ConfigGravar();
                _messageBox.Show("Configurações de impressora foram salvas com sucesso! Agora será checado se a configuração é válida.", "Configurações De Impressora", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                //posPrinter.Ativar();
                _messageBox.Show("Configurações de impressora validadas com sucesso!", "Configurações De Impressora", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _messageBox.Show("A configuração foi salva com sucesso, mas a impressora não está respondendo. Cheque se a impressora está conectada corretamente e que está ligada.\n\n" + ex.Message, "Configurações De Impressora", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void AtualizarPortas(object obj)
        {
            ChamaAcharPortas();
        }

        private void ChamaAcharPortas()
        {
            Portas = posPrinter.AcharPortas().ToList();
            Portas.Insert(0, "USB");
            OnPropertyChanged("Portas");
            Porta = posPrinter.Config.Porta;
        }

        public void OnClosing()
        {
            if (posPrinter != null)
            {
                //posPrinter.Desativar();
                posPrinter.Dispose();
            }
        }

        public IEnumerable<ACBrPosPrinterModelo> ModelosEnum
        {
            get
            {
                return _modelosEnum;
            }

            set
            {
                _modelosEnum = value;
                OnPropertyChanged("ModelosEnum");
            }
        }

        public ACBrPosPrinterModelo ModeloSelecionado
        {
            get
            {
                return _modeloSelecionado;
            }

            set
            {
                _modeloSelecionado = value;
                OnPropertyChanged("ModeloSelecionado");
            }
        }

        public IList<string> Portas
        {
            get
            {
                return _portas;
            }

            set
            {
                _portas = value;
                OnPropertyChanged("Portas");
            }
        }

        public string Porta
        {
            get
            {
                return _porta;
            }

            set
            {
                _porta = value;
                OnPropertyChanged("Porta");
            }
        }
    }
}
