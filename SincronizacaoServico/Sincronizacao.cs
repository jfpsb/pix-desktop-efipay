using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Abstrato;
using SincronizacaoServico.Banco;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using System.Timers;

namespace SincronizacaoServico
{
    public class Sincronizacao
    {
        private System.Timers.Timer timer;

        public Sincronizacao()
        {
            //TODO: tratar caso build session factory dê erro
            SessionProvider.BuildSessionFactory();
            SessionProviderBackup.BuildSessionFactory();
            timer = new System.Timers.Timer(2000) { AutoReset = true };
            timer.Elapsed += ExecutaSync;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void ExecutaSync(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Iniciando sincronização em {e.SignalTime}");
            timer.Stop();
            StartSync();
            Console.WriteLine($"Fim de sincronização em {DateTime.Now}");
            timer.Start();
        }

        private async void StartSync()
        {
            LastSync lastSync;
            DateTime inicioSync = DateTime.Now;

            try
            {

                lastSync = await GetLastSyncTime();

                DateTime syncTime = lastSync != null ? lastSync.LastSyncTime : DateTime.MinValue;

                ISession local = SessionProvider.GetSession();
                ISession remote = SessionProviderBackup.GetSession();

                ASincronizar<Calendario> aCalendario = new SincronizarGenerico<Calendario>(local, remote);
                ASincronizar<Valor> aValor = new SincronizarGenerico<Valor>(local, remote);
                ASincronizar<Loc> aLoc = new SincronizarGenerico<Loc>(local, remote);
                ASincronizar<QRCode> aQRCode = new SincronizarGenerico<QRCode>(local, remote);
                SincronizarCobranca aCobranca = new SincronizarCobranca(local, remote);
                ASincronizar<Pagador> aPagador = new SincronizarGenerico<Pagador>(local, remote);
                SincronizarPix aPix = new SincronizarPix(local, remote);
                ASincronizar<Horario> aHorario = new SincronizarGenerico<Horario>(local, remote);
                SincronizarDevolucao aDevolucao = new SincronizarDevolucao(local, remote);

                await aCalendario.Sincronizar(syncTime);
                await aValor.Sincronizar(syncTime);
                await aLoc.Sincronizar(syncTime);
                await aQRCode.Sincronizar(syncTime);
                await aCobranca.Sincronizar(syncTime);
                await aPagador.Sincronizar(syncTime);
                await aPix.Sincronizar(syncTime);
                await aHorario.Sincronizar(syncTime);
                await aDevolucao.Sincronizar(syncTime);

                SessionProvider.FechaSession(local);
                SessionProviderBackup.FechaSession(remote);

                if (lastSync == null)
                {
                    lastSync = new LastSync
                    {
                        LastSyncTime = inicioSync
                    };
                }
                else
                {
                    lastSync.LastSyncTime = inicioSync;
                }

                using (var session = SessionProvider.GetSession())
                {
                    using (ITransaction tx = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(lastSync);
                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propriedade"></param>
        /// <param name="entidade"></param>
        /// <returns></returns>
        private async Task<LastSync> GetLastSyncTime()
        {
            using (var session = SessionProvider.GetSession())
            {
                var criteria = session.CreateCriteria<LastSync>();
                return await criteria.UniqueResultAsync<LastSync>();
            }
        }

        private async Task<E> ListarPorUuidLocal<E>(Guid uuid) where E : AModel
        {
            using (var session = SessionProvider.GetSession())
            {
                var criteria = session.CreateCriteria<E>();
                criteria.Add(Restrictions.Eq("Uuid", uuid));
                return await criteria.UniqueResultAsync<E>();
            }
        }

        private async Task<E> ListarPorUuidRemoto<E>(Guid uuid) where E : AModel
        {
            using (var session = SessionProviderBackup.GetSession())
            {
                var criteria = session.CreateCriteria<E>();
                criteria.Add(Restrictions.Eq("Uuid", uuid));
                return await criteria.UniqueResultAsync<E>();
            }
        }
    }
}
