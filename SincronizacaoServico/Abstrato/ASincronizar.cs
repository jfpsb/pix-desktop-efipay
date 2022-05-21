using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Model;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Interfaces
{
    public abstract class ASincronizar<E> where E : AModel
    {
        protected ISession _local, _remoto;
        protected bool _isChaveAutoIncremento;
        protected ASincronizar(ISession local, ISession remoto, bool isChaveAutoIncremento)
        {
            _local = local;
            _remoto = remoto;
            _isChaveAutoIncremento = isChaveAutoIncremento;
        }

        public abstract Task Sincronizar();

        protected async Task<T> ListarPorIdLocal<T>(object id)
        {
            return await _local.GetAsync<T>(id);
        }

        protected async Task<T> ListarPorIdRemoto<T>(object id)
        {
            return await _remoto.GetAsync<T>(id);
        }

        protected async Task<T> ListarPorUuidLocal<T>(Guid uuid) where T : AModel
        {
            var criteria = _local.CreateCriteria<T>();
            criteria.Add(Restrictions.Eq("Uuid", uuid));
            return await criteria.UniqueResultAsync<T>();
        }

        protected async Task<T> ListarPorUuidRemoto<T>(Guid uuid) where T : AModel
        {
            var criteria = _remoto.CreateCriteria<T>();
            criteria.Add(Restrictions.Eq("Uuid", uuid));
            return await criteria.UniqueResultAsync<T>();
        }

        protected async Task<LastSync> GetLastSyncTime(string tabela)
        {
            var criteria = _local.CreateCriteria<LastSync>();
            criteria.Add(Restrictions.Eq("Tabela", tabela));
            return await criteria.UniqueResultAsync<LastSync>();
        }

        protected async Task SaveLastSyncTime(LastSync lastSync, DateTime inicioSync)
        {
            using (ITransaction tx = _local.BeginTransaction())
            {
                try
                {
                    lastSync.LastSyncTime = inicioSync;
                    await _local.SaveOrUpdateAsync(lastSync);
                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogSync(ex, "savelastsynctime - " + lastSync.Tabela);
                    Console.WriteLine(ex.Message, ex);
                    throw;
                }
            }
        }

        protected async Task InsertRemotoParaLocal(IList<E> insertsRemotoParaLocal)
        {
            using (ITransaction tx = _local.BeginTransaction())
            {
                try
                {
                    if (insertsRemotoParaLocal.Count > 0)
                    {
                        _local.Clear();
                        Console.WriteLine($"Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                        foreach (E insert in insertsRemotoParaLocal)
                        {
                            await _local.SaveAsync(insert);
                        }
                        await tx.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogSync(ex, "InsertRemotoParaLocal");
                    Console.WriteLine(ex.Message, ex);
                    throw;
                }
            }
        }
        protected async Task UpdateRemotoParaLocal(IList<E> updatesRemotoParaLocal)
        {
            using (ITransaction tx = _local.BeginTransaction())
            {
                try
                {
                    if (updatesRemotoParaLocal.Count > 0)
                    {
                        _local.Clear();
                        Console.WriteLine($"Atualizando {updatesRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                        foreach (E update in updatesRemotoParaLocal)
                        {
                            await _local.UpdateAsync(update);
                        }
                    }

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogSync(ex, "UpdateRemotoParaLocal");
                    Console.WriteLine(ex.Message, ex);
                    throw;
                }
            }
        }
        protected async Task InsertLocalParaRemoto(IList<E> insertsLocalParaRemoto)
        {
            using (ITransaction tx = _remoto.BeginTransaction())
            {
                try
                {
                    if (insertsLocalParaRemoto.Count > 0)
                    {
                        _remoto.Clear();
                        Console.WriteLine($"Inserindo {insertsLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                        foreach (E insert in insertsLocalParaRemoto)
                        {
                            await _remoto.SaveAsync(insert);
                        }
                    }

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogSync(ex, "InsertLocalParaRemoto");
                    Console.WriteLine(ex.Message, ex);
                    throw;
                }
            }
        }
        protected async Task UpdateLocalParaRemoto(IList<E> updatesLocalParaRemoto)
        {
            using (ITransaction tx = _remoto.BeginTransaction())
            {
                try
                {
                    if (updatesLocalParaRemoto.Count > 0)
                    {
                        _remoto.Clear();
                        Console.WriteLine($"Atualizando {updatesLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                        foreach (E update in updatesLocalParaRemoto)
                        {
                            await _remoto.UpdateAsync(update);
                        }
                    }

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogSync(ex, "UpdateLocalParaRemoto");
                    Console.WriteLine(ex.Message, ex);
                    throw;
                }
            }
        }
    }
}
