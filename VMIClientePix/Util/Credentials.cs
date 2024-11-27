using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using VMIClientePix.ViewModel.Services.Concretos;

namespace VMIClientePix.Util
{
    public class Credentials
    {
        public static JObject GNEndpoints()
        {
            try
            {
                JObject credentials_encrypted = JObject.Parse(ArquivosApp.GetCredentialsEncrypted());

                byte[] clientIDEncrypted = Convert.FromBase64String((string)credentials_encrypted["client_id"]);
                byte[] clientSecretEncrypted = Convert.FromBase64String((string)credentials_encrypted["client_secret"]);

                var credentials = new
                {
                    client_id = Encoding.Unicode.GetString(ProtectedData.Unprotect(clientIDEncrypted, null, DataProtectionScope.LocalMachine)),
                    client_secret = Encoding.Unicode.GetString(ProtectedData.Unprotect(clientSecretEncrypted, null, DataProtectionScope.LocalMachine)),
                    pix_cert = (string)credentials_encrypted["pix_cert"],
                    sandbox = (string)credentials_encrypted["sandbox"]
                };

                return JObject.Parse(JsonConvert.SerializeObject(credentials));
            }
            catch (Exception ex)
            {
                Log.EscreveLogCredenciais(ex);
                return null;
            }
        }

        public static string GetLocalHost()
        {
            try
            {
                JObject encrypted = JObject.Parse(ArquivosApp.GetCredentialsNHibernate());
                var addresses = Dns.GetHostAddresses((string)encrypted["server"]);

                if (addresses != null)
                {
                    var ipv4 = addresses.Where(w => w.AddressFamily == AddressFamily.InterNetwork && IPAddress.IsLoopback(w) == false).ToList();

                    if (ipv4.Count > 1)
                    {
                        throw new Exception("Mais de um IP IPV4 encontrado");
                    }

                    if (ipv4.Count == 1)
                    {
                        return ipv4[0].ToString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                new MessageBoxService().Show(ex.Message);
                return null;
            }
        }

        public static string HibernateLocalConnString()
        {
            try
            {
                JObject encrypted = JObject.Parse(ArquivosApp.GetCredentialsNHibernate());

                byte[] userIdEncrypted = Convert.FromBase64String((string)encrypted["userid"]);
                byte[] passwordEncrypted = Convert.FromBase64String((string)encrypted["password"]);

                var credentials = new
                {
                    server = (string)encrypted["server"],
                    port = (string)encrypted["port"],
                    userid = Encoding.Unicode.GetString(ProtectedData.Unprotect(userIdEncrypted, null, DataProtectionScope.LocalMachine)),
                    password = Encoding.Unicode.GetString(ProtectedData.Unprotect(passwordEncrypted, null, DataProtectionScope.LocalMachine)),
                    database = (string)encrypted["database"]
                };

                return $"server={credentials.server};" +
                    $"port={credentials.port};" +
                    $"userid={credentials.userid};" +
                    $"password={credentials.password};" +
                    $"database={credentials.database}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                new MessageBoxService().Show(ex.Message);
                return null;
            }
        }

        public static string HibernateRemotoConnString()
        {
            try
            {
                JObject encrypted = JObject.Parse(ArquivosApp.GetCredentialsNHibernateRemoto());

                byte[] userIdEncrypted = Convert.FromBase64String((string)encrypted["userid"]);
                byte[] passwordEncrypted = Convert.FromBase64String((string)encrypted["password"]);

                var credentials = new
                {
                    server = (string)encrypted["server"],
                    port = (string)encrypted["port"],
                    userid = Encoding.Unicode.GetString(ProtectedData.Unprotect(userIdEncrypted, null, DataProtectionScope.LocalMachine)),
                    password = Encoding.Unicode.GetString(ProtectedData.Unprotect(passwordEncrypted, null, DataProtectionScope.LocalMachine)),
                    database = (string)encrypted["database"]
                };

                return $"server={credentials.server};" +
                    $"port={credentials.port};" +
                    $"userid={credentials.userid};" +
                    $"password={credentials.password};" +
                    $"database={credentials.database}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
