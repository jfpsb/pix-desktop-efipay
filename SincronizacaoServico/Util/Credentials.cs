using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SincronizacaoServico.Util
{
    public class Credentials
    {
        public static string GetLocalHost()
        {
            try
            {
                JObject encrypted = JObject.Parse(File.ReadAllText("hibernate_config_encrypted.json"));
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
