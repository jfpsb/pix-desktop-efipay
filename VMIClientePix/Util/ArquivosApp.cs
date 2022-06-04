using System.IO;

namespace VMIClientePix.Util
{
    public class ArquivosApp
    {
        public static string GetCredentialsEncrypted()
        {
            string path = Path.Combine(Global.AppDocumentsFolder, "credentials_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de credenciais não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetCredentialsNHibernate()
        {
            string path = Path.Combine(Global.AppDocumentsFolder, "hibernate_config_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de conexão NHibernate local não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetCredentialsNHibernateRemoto()
        {
            string path = Path.Combine(Global.AppDocumentsFolder, "hibernate_backup_config_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de conexão NHibernate remoto não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetConfig()
        {
            string path = Path.Combine(Global.AppDocumentsFolder, "Config.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de configuração do app não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetDadosRecebedor()
        {
            string path = Path.Combine(Global.AppDocumentsFolder, "dados_recebedor.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de dados de recebdor não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static bool ConfigExists()
        {
            return File.Exists(Path.Combine(Global.AppDocumentsFolder, "Config.json"));
        }

        public static bool DadosRecebedorExists()
        {
            return File.Exists(Path.Combine(Global.AppDocumentsFolder, "dados_recebedor.json"));
        }

        public static void WriteConfig(string text)
        {
            File.WriteAllText(Path.Combine(Global.AppDocumentsFolder, "Config.json"), text);
        }

        public static void WriteDadosRecebedor(string text)
        {
            File.WriteAllText(Path.Combine(Global.AppDocumentsFolder, "dados_recebedor.json"), text);
        }
    }
}
