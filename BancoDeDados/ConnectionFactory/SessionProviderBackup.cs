using NHibernate;
using NHibernate.Cfg;
using System;
using VMIClientePix.Util;

namespace VMIClientePix.BancoDeDados.ConnectionFactory
{
    /// <summary>
    /// Classe estática responsável pelas Sessions necessárias para o uso de banco de dados com NHibernate.
    /// </summary>
    public static class SessionProviderBackup
    {
        /// <summary>
        /// Variável que guardará a configuração necessária contida em hibernate.cfg.xml.
        /// </summary>
        public static Configuration BackupConfiguration;

        /// <summary>
        /// Guarda a Session Factory criada para uso em DAO.
        /// </summary>        
        public static ISessionFactory BackupSessionFactory = null;

        /// <summary>
        /// Método responsável pela criação da Session Factory.
        /// </summary>
        /// <returns>myConfiguration.BuildSessionFactory()</returns>
        public static ISessionFactory BuildSessionFactory()
        {
            BackupConfiguration = new Configuration();
            BackupConfiguration.Configure("hibernateBackup.cfg.xml");

            string connString = Credentials.HibernateBackupConnString();
            const string connectionStringKey = "connection.connection_string";
            BackupConfiguration.SetProperty(connectionStringKey, connString);

            return BackupConfiguration.BuildSessionFactory();
        }

        public static ISession GetSession()
        {
            if (BackupSessionFactory == null)
            {
                BackupSessionFactory = BuildSessionFactory();
            }

            ISession _session = BackupSessionFactory.OpenSession();

            return _session;
        }

        public static IStatelessSession GetStatelessSession()
        {
            if (BackupSessionFactory == null)
            {
                BackupSessionFactory = BuildSessionFactory();
            }

            IStatelessSession _session = BackupSessionFactory.OpenStatelessSession();

            return _session;
        }

        public static void FechaSessionFactory()
        {
            if (BackupSessionFactory != null && !BackupSessionFactory.IsClosed)
            {
                BackupSessionFactory.Close();
                Console.WriteLine("BackupSessionFactory fechada");
            }
        }

        public static void FechaSession(ISession session)
        {
            if (session != null && session.IsOpen)
            {
                session?.Clear();
                session?.Dispose();
            }
            Console.WriteLine($"Sessão Fechada");
        }
    }
}
