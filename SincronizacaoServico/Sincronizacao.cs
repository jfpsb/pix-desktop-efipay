using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Banco;
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

            lastSync = await GetLastSyncTime();

            DateTime syncTime = lastSync != null ? lastSync.LastSyncTime : DateTime.MinValue;

            SyncEntidade<Calendario>(syncTime);
            SyncEntidade<Valor>(syncTime);
            SyncEntidade<Loc>(syncTime);
            SyncEntidade<QRCode>(syncTime);
            SyncEntidade<Cobranca>(syncTime);
            SyncEntidade<Pagador>(syncTime);
            SyncEntidade<Pix>(syncTime);
            SyncEntidade<Horario>(syncTime);
            SyncEntidade<Devolucao>(syncTime);

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
        private async void SyncEntidade<E>(DateTime lastSync, bool setIdToDefault = false) where E : AModel
        {
            Console.WriteLine($"Iniciando sincronização de {typeof(E).Name}");

            IList<E> insertsRemotoParaLocal = new List<E>();
            IList<E> updatesRemotoParaLocal = new List<E>();
            IList<E> insertsLocalParaRemoto = new List<E>();
            IList<E> updatesLocalParaRemoto = new List<E>();

            using (var session = SessionProviderBackup.GetStatelessSession())
            {
                try
                {
                    var criteria = session.CreateCriteria<E>();
                    criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                    var lista = await criteria.ListAsync<E>();

                    foreach (E e in lista)
                    {
                        //Entidade com mesmo UUID no banco local
                        var ent = await ListarPorUuidLocal<E>(e.Uuid);
                        bool AIIdEnt = ent is not Cobranca && ent is not Pix && ent is not Loc && ent is not Devolucao;

                        if (ent != null)
                        {
                            if (AIIdEnt)
                                e.Uuid = Guid.NewGuid();
                        }

                        if (setIdToDefault)
                            e.SetIdentifierToDefault();

                        insertsRemotoParaLocal.Add(e);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }

            using (var session = SessionProviderBackup.GetStatelessSession())
            {
                try
                {
                    var criteria = session.CreateCriteria<E>();
                    criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                    var lista = await criteria.ListAsync<E>();

                    foreach (E e in lista)
                    {
                        //Entidade com mesmo UUID no banco local
                        var ent = await ListarPorUuidLocal<E>(e.Uuid);

                        if (ent == null)
                        {
                            insertsRemotoParaLocal.Add(e);
                            continue;
                        }

                        //Copia os dados do banco remoto para atualizar no local, preservando uuid e id local
                        ent.Copiar(e);
                        //Coloca na coleção que será salva
                        updatesRemotoParaLocal.Add(ent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }

            using (var session = SessionProvider.GetStatelessSession())
            {
                try
                {
                    var criteria = session.CreateCriteria<E>();
                    criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                    var lista = await criteria.ListAsync<E>();

                    foreach (E e in lista)
                    {
                        //Entidade com mesmo UUID no banco remoto
                        var ent = await ListarPorUuidRemoto<E>(e.Uuid);

                        if (ent == null)
                        {
                            insertsLocalParaRemoto.Add(e);
                        }
                        else
                        {
                            if (setIdToDefault)
                                e.SetIdentifierToDefault();
                            e.Uuid = Guid.NewGuid();

                            using (ITransaction tx = session.BeginTransaction())
                            {
                                await session.InsertAsync(e);
                                await tx.CommitAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }

            using (var session = SessionProviderBackup.GetStatelessSession())
            {
                try
                {
                    var criteria = session.CreateCriteria<E>();
                    criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                    var lista = await criteria.ListAsync<E>();

                    foreach (E e in lista)
                    {
                        //Entidade com mesmo UUID no banco remoto
                        var ent = await ListarPorUuidRemoto<E>(e.Uuid);

                        if (ent == null)
                        {
                            insertsLocalParaRemoto.Add(e);
                            continue;
                        }

                        //Copia os dados do banco local para atualizar no remoto, preservando uuid e id remota
                        ent.Copiar(e);
                        //Coloca na coleção que será salva
                        updatesLocalParaRemoto.Add(ent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }

            if (insertsRemotoParaLocal.Count > 0)
            {
                Console.WriteLine($"Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                using (var session = SessionProvider.GetStatelessSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        foreach (E insert in insertsRemotoParaLocal)
                        {
                            if (setIdToDefault)
                                insert.SetIdentifierToDefault();
                            await session.InsertAsync(insert);
                        }

                        await tx.CommitAsync();
                    }
                }
            }

            if (updatesRemotoParaLocal.Count > 0)
            {
                Console.WriteLine($"Atualizando {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                using (var session = SessionProvider.GetStatelessSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        foreach (E update in updatesRemotoParaLocal)
                        {
                            await session.UpdateAsync(update);
                        }

                        await tx.CommitAsync();
                    }
                }
            }

            if (insertsLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco local para o remoto.");
                using (var session = SessionProviderBackup.GetStatelessSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        foreach (E insert in insertsLocalParaRemoto)
                        {
                            if (setIdToDefault)
                                insert.SetIdentifierToDefault();
                            await session.InsertAsync(insert);
                        }

                        await tx.CommitAsync();
                    }
                }
            }

            if (updatesLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Atualizando {insertsRemotoParaLocal.Count} registro(s) do banco local para o remoto.");
                using (var session = SessionProviderBackup.GetStatelessSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        foreach (E update in updatesLocalParaRemoto)
                        {
                            await session.UpdateAsync(update);
                        }

                        await tx.CommitAsync();
                    }
                }
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
