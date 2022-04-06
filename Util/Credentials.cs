using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
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
                JObject credentials_encrypted = JObject.Parse(File.ReadAllText("credentials_encrypted.json"));

                byte[] clientIDEncrypted = Convert.FromBase64String((string)credentials_encrypted["client_id"]);
                byte[] clientSecretEncrypted = Convert.FromBase64String((string)credentials_encrypted["client_secret"]);

                var credentials = new
                {
                    client_id = Encoding.Unicode.GetString(ProtectedData.Unprotect(clientIDEncrypted, null, DataProtectionScope.CurrentUser)),
                    client_secret = Encoding.Unicode.GetString(ProtectedData.Unprotect(clientSecretEncrypted, null, DataProtectionScope.CurrentUser)),
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

        public static string HibernateLocalConnString()
        {
            try
            {
                JObject encrypted = JObject.Parse(File.ReadAllText("hibernate_config_encrypted.json"));

                byte[] userIdEncrypted = Convert.FromBase64String((string)encrypted["userid"]);
                byte[] passwordEncrypted = Convert.FromBase64String((string)encrypted["password"]);

                var credentials = new
                {
                    server = "localhost",
                    port = (string)encrypted["port"],
                    userid = Encoding.Unicode.GetString(ProtectedData.Unprotect(userIdEncrypted, null, DataProtectionScope.CurrentUser)),
                    password = Encoding.Unicode.GetString(ProtectedData.Unprotect(passwordEncrypted, null, DataProtectionScope.CurrentUser)),
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

        public static string HibernateBackupConnString()
        {
            try
            {
                JObject encrypted = JObject.Parse(File.ReadAllText("hibernate_backup_config_encrypted.json"));

                byte[] userIdEncrypted = Convert.FromBase64String((string)encrypted["userid"]);
                byte[] passwordEncrypted = Convert.FromBase64String((string)encrypted["password"]);

                var credentials = new
                {
                    server = (string)encrypted["server"],
                    port = (string)encrypted["port"],
                    userid = Encoding.Unicode.GetString(ProtectedData.Unprotect(userIdEncrypted, null, DataProtectionScope.CurrentUser)),
                    password = Encoding.Unicode.GetString(ProtectedData.Unprotect(passwordEncrypted, null, DataProtectionScope.CurrentUser)),
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
