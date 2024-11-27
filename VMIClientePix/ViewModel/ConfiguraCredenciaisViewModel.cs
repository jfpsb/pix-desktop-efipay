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
        private string _password;
        private string _porta;
        private string _hostRemoto;
        private string _userIDRemoto;
        private string _passwordRemoto;
        private string _portaRemoto;
        private string _databaseRemoto;
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
            Porta = "3306"; //Porta padrão do Mysql
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

                if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(Password))
                {
                    byte[] userIDByteArray = Encoding.Unicode.GetBytes(UserID);
                    byte[] userIDByteArrayEncriptado = ProtectedData.Protect(userIDByteArray, null, DataProtectionScope.LocalMachine);
                    string userIDEncriptado = Convert.ToBase64String(userIDByteArrayEncriptado);

                    byte[] passwordByteArray = Encoding.Unicode.GetBytes(Password);
                    byte[] passwordByteArrayEncriptado = ProtectedData.Protect(passwordByteArray, null, DataProtectionScope.LocalMachine);
                    string passwordEncriptado = Convert.ToBase64String(passwordByteArrayEncriptado);

                    var hibernate_local_config = new
                    {
                        server = "localhost",
                        port = Porta,
                        userid = userIDEncriptado,
                        password = passwordEncriptado,
                        database = "vmiclientepix"
                    };

                    string hibernate_json = JsonConvert.SerializeObject(hibernate_local_config, Formatting.Indented);
                    File.WriteAllText(Path.Combine(Global.AppDocumentsFolder, "hibernate_config_encrypted.json"), hibernate_json);
                }

                if (!string.IsNullOrEmpty(UserIDRemoto) && !string.IsNullOrEmpty(PasswordRemoto))
                {
                    byte[] userIDRemotoByteArray = Encoding.Unicode.GetBytes(UserIDRemoto);
                    byte[] userIDRemotoByteArrayEncriptado = ProtectedData.Protect(userIDRemotoByteArray, null, DataProtectionScope.LocalMachine);
                    string userIDRemotoEncriptado = Convert.ToBase64String(userIDRemotoByteArrayEncriptado);

                    byte[] passwordRemotoByteArray = Encoding.Unicode.GetBytes(PasswordRemoto);
                    byte[] passwordRemotoByteArrayEncriptado = ProtectedData.Protect(passwordRemotoByteArray, null, DataProtectionScope.LocalMachine);
                    string passwordRemotoEncriptado = Convert.ToBase64String(passwordRemotoByteArrayEncriptado);

                    var hibernate_backup_local_config = new
                    {
                        server = HostRemoto,
                        port = PortaRemoto,
                        userid = userIDRemotoEncriptado,
                        password = passwordRemotoEncriptado,
                        database = DatabaseRemoto
                    };

                    string hibernate_backup_json = JsonConvert.SerializeObject(hibernate_backup_local_config, Formatting.Indented);
                    File.WriteAllText(Path.Combine(Global.AppDocumentsFolder, "hibernate_backup_config_encrypted.json"), hibernate_backup_json);
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

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged("Password");
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

        public string HostRemoto
        {
            get
            {
                return _hostRemoto;
            }

            set
            {
                _hostRemoto = value;
                OnPropertyChanged("HostRemoto");
            }
        }

        public string UserIDRemoto
        {
            get
            {
                return _userIDRemoto;
            }

            set
            {
                _userIDRemoto = value;
                OnPropertyChanged("UserIDRemoto");
            }
        }

        public string PasswordRemoto
        {
            get
            {
                return _passwordRemoto;
            }

            set
            {
                _passwordRemoto = value;
                OnPropertyChanged("PasswordRemoto");
            }
        }

        public string PortaRemoto
        {
            get
            {
                return _portaRemoto;
            }

            set
            {
                _portaRemoto = value;
                OnPropertyChanged("PortaRemoto");
            }
        }

        public string DatabaseRemoto
        {
            get
            {
                return _databaseRemoto;
            }

            set
            {
                _databaseRemoto = value;
                OnPropertyChanged("DatabaseRemoto");
            }
        }
    }
}
