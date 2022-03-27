using NHibernate;
using NHibernate.Cfg;
using System;

namespace VandaModaIntimaWpf.BancoDeDados.ConnectionFactory
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
            if (session.IsOpen)
            {
                session?.Clear();
                session?.Dispose();
            }
            Console.WriteLine($"Sessão Fechada");
        }
    }
}
