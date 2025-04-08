using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using VMIClientePix.Util;
using VMIClientePix.View.Interfaces;
using VMIClientePix.ViewModel.Services.Concretos;
using VMIClientePix.ViewModel.Services.Interfaces;

namespace VMIClientePix.ViewModel
{
    public class ConfiguraCredenciaisViewModel : ObservableObject, IRequestClose
    {
        private string _clientID;
        private string _clientSecret;
        private string _userID;
        private string _caminhoCertificado;
        private IMessageBoxService messageBox;

        public event EventHandler<EventArgs> RequestClose;
        public ICommand SalvarCredenciaisComando { get; set; }
        public ICommand AbrirProcurarComando { get; set; }

        public ConfiguraCredenciaisViewModel()
        {
            messageBox = new MessageBoxService();
            SalvarCredenciaisComando = new RelayCommand(SalvarCredenciais);
            AbrirProcurarComando = new RelayCommand(AbrirProcurar);
        }

        private void AbrirProcurar(object obj)
        {
            IOpenFileDialog dialog = obj as IOpenFileDialog;

            if (dialog != null)
            {
                string caminho = dialog.OpenFileDialog();

                if (caminho != null)
                {
                    CaminhoCertificado = caminho;
                }
            }
        }

        private void SalvarCredenciais(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(ClientID) && !string.IsNullOrEmpty(ClientSecret))
                {
                    byte[] clientIDByteArray = Encoding.Unicode.GetBytes(ClientID);
                    byte[] clientIDByteArrayEncriptado = ProtectedData.Protect(clientIDByteArray, null, DataProtectionScope.LocalMachine);
                    string clientIDEncriptado = Convert.ToBase64String(clientIDByteArrayEncriptado);

                    byte[] clientSecretByteArray = Encoding.Unicode.GetBytes(ClientSecret);
                    byte[] clientSecretByteArrayEncriptado = ProtectedData.Protect(clientSecretByteArray, null, DataProtectionScope.LocalMachine);
                    string clientSecretEncriptado = Convert.ToBase64String(clientSecretByteArrayEncriptado);

                    var credentials_encrypted = new
                    {
                        client_id = clientIDEncriptado,
                        client_secret = clientSecretEncriptado,
                        pix_cert = CaminhoCertificado,
                        sandbox = false
                    };

                    string credentials_json = JsonConvert.SerializeObject(credentials_encrypted, Formatting.Indented);
                    File.WriteAllText(Path.Combine(Global.AppDocumentsFolder, "credentials_encrypted.json"), credentials_json);
                }

                messageBox.Show("Credenciais Salvas Com Sucesso!");

                RequestClose?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                messageBox.Show($"Erro ao salvar credenciais!\n\n{ex.Message}");
            }
        }

        public string ClientID
        {
            get
            {
                return _clientID;
            }

            set
            {
                _clientID = value;
                OnPropertyChanged("ClientID");
            }
        }

        public string ClientSecret
        {
            get
            {
                return _clientSecret;
            }

            set
            {
                _clientSecret = value;
                OnPropertyChanged("ClientSecret");
            }
        }

        public string UserID
        {
            get
            {
                return _userID;
            }

            set
            {
                _userID = value;
                OnPropertyChanged("UserID");
            }
        }

        public string CaminhoCertificado
        {
            get
            {
                return _caminhoCertificado;
            }

            set
            {
                _caminhoCertificado = value;
                OnPropertyChanged("CaminhoCertificado");
            }
        }
    }
}
