using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMIClientePix.Util
{
    public class ArquivosApp
    {
        private static readonly string AppDocumentsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VMIClientePix");

        public static string GetCredentialsEncrypted()
        {
            string path = Path.Combine(AppDocumentsFolder, "credentials_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de credenciais não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetCredentialsNHibernate()
        {
            string path = Path.Combine(AppDocumentsFolder, "hibernate_config_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de conexão NHibernate local não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetCredentialsNHibernateRemoto()
        {
            string path = Path.Combine(AppDocumentsFolder, "hibernate_backup_config_encrypted.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de conexão NHibernate remoto não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetConfig()
        {
            string path = Path.Combine(AppDocumentsFolder, "Config.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de configuração do app não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static string GetDadosRecebedor()
        {
            string path = Path.Combine(AppDocumentsFolder, "dados_recebedor.json");
            if (!File.Exists(path))
            {
                throw new IOException("Arquivo de dados de recebdor não encontrado.");
            }
            return File.ReadAllText(path);
        }

        public static bool ConfigExists()
        {
            return File.Exists(Path.Combine(AppDocumentsFolder, "Config.json"));
        }

        public static bool DadosRecebedorExists()
        {
            return File.Exists(Path.Combine(AppDocumentsFolder, "dados_recebedor.json"));
        }

        public static void WriteConfig(string text)
        {
            File.WriteAllText(Path.Combine(AppDocumentsFolder, "Config.json"), text);
        }

        public static void WriteDadosRecebedor(string text)
        {
            File.WriteAllText(Path.Combine(AppDocumentsFolder, "dados_recebedor.json"), text);
        }
    }
}
