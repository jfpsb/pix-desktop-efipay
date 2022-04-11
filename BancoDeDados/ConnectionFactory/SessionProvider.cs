using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.IO;
using System.Text.RegularExpressions;
using VMIClientePix.Util;

namespace VMIClientePix.BancoDeDados.ConnectionFactory
{
    /// <summary>
    /// Classe estática responsável pelas Sessions necessárias para o uso de banco de dados com NHibernate.
    /// </summary>
    public static class SessionProvider
    {
        /// <summary>
        /// Variável que guardará a configuração necessária contida em hibernate.cfg.xml.
        /// </summary>
        public static Configuration Configuration;

        /// <summary>
        /// Guarda a Session Factory criada para uso em DAO.
        /// </summary>        
        public static ISessionFactory SessionFactory = null;

        /// <summary>
        /// Método responsável pela criação da Session Factory.
        /// </summary>
        /// <returns>myConfiguration.BuildSessionFactory()</returns>
        public static ISessionFactory BuildSessionFactory()
        {
            Configuration = new Configuration();
            Configuration.Configure();

            string connString = Credentials.HibernateLocalConnString();
            const string connectionStringKey = "connection.connection_string";
            Configuration.SetProperty(connectionStringKey, connString);

            return Configuration.BuildSessionFactory();
        }

        public static ISession GetSession()
        {
            if (SessionFactory == null)
            {
                SessionFactory = BuildSessionFactory();
            }

            ISession _session = SessionFactory.OpenSession();

            return _session;
        }

        public static void FechaSessionFactory()
        {
            if (SessionFactory != null && !SessionFactory.IsClosed)
            {
                SessionFactory.Close();
                Console.WriteLine("MainSessionFactory fechada");
            }
        }

        public static void FechaSession(ISession session)
        {
            if (session != null && session.IsOpen)
            {
                session.Clear();
                session.Dispose();
                Console.WriteLine($"Sessão Fechada");
            }
        }
    }
}
