using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SincronizacaoServico.Util
{
    public class Credentials
    {
        public static string HibernateLocalConnString()
        {
            try
            {
                JObject encrypted = JObject.Parse(File.ReadAllText("hibernate_config_encrypted.json"));

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
                Log.EscreveLogCredenciais(ex);
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
                Log.EscreveLogCredenciais(ex);
                return null;
            }
        }
    }
}
