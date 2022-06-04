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

        protected async Task<object> ListarPorUuidLocal(string nomeEntidade, Guid uuid)
        {
            var criteria = _local.CreateCriteria(nomeEntidade);
            criteria.Add(Restrictions.Eq("Uuid", uuid));
            return await criteria.UniqueResultAsync();
        }
        protected async Task<object> ListarPorUuidRemoto(string nomeEntidade, Guid uuid)
        {
            var criteria = _remoto.CreateCriteria(nomeEntidade);
            criteria.Add(Restrictions.Eq("Uuid", uuid));
            return await criteria.UniqueResultAsync();
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
                        double i = 0.0;
                        _local.Clear();
                        Console.WriteLine($"{typeof(E).Name} - Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                        foreach (E insert in insertsRemotoParaLocal)
                        {
                            await _local.SaveAsync(insert);
                            Console.WriteLine($"{typeof(E).Name} - Inserindo de banco remoto para local -> Inserindo dados -> Progresso: {Math.Round(i++ / insertsRemotoParaLocal.Count * 100, 2)}%");
                        }
                        Console.WriteLine($"{typeof(E).Name} - Inserindo de banco remoto para local -> Inserindo dados -> Progresso: {Math.Round(i++ / insertsRemotoParaLocal.Count * 100, 2)}%");
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
                        double i = 0.0;
                        _local.Clear();
                        Console.WriteLine($"{typeof(E).Name} - Atualizando {updatesRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                        foreach (E update in updatesRemotoParaLocal)
                        {
                            await _local.UpdateAsync(update);
                            Console.WriteLine($"{typeof(E).Name} - Atualizando de banco remoto para local -> Inserindo dados -> Progresso: {Math.Round(i++ / updatesRemotoParaLocal.Count * 100, 2)}%");
                        }
                        Console.WriteLine($"{typeof(E).Name} - Atualizando de banco remoto para local -> Inserindo dados -> Progresso: {Math.Round(i++ / updatesRemotoParaLocal.Count * 100, 2)}%");
                        await tx.CommitAsync();
                    }
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
                        double i = 0.0;
                        _remoto.Clear();
                        Console.WriteLine($"{typeof(E).Name} - Inserindo {insertsLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                        foreach (E insert in insertsLocalParaRemoto)
                        {
                            await _remoto.SaveAsync(insert);
                            Console.WriteLine($"{typeof(E).Name} - Inserindo de banco local para remoto -> Inserindo dados -> Progresso: {Math.Round(i++ / insertsLocalParaRemoto.Count * 100, 2)}%");
                        }
                        Console.WriteLine($"{typeof(E).Name} - Inserindo de banco local para remoto -> Inserindo dados -> Progresso: {Math.Round(i++ / insertsLocalParaRemoto.Count * 100, 2)}%");
                        await tx.CommitAsync();
                    }
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
                        double i = 0.0;
                        _remoto.Clear();
                        Console.WriteLine($"{typeof(E).Name} - Atualizando {updatesLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                        foreach (E update in updatesLocalParaRemoto)
                        {
                            await _remoto.UpdateAsync(update);
                            Console.WriteLine($"{typeof(E).Name} - Atualizando de banco local para remoto -> Inserindo dados -> Progresso: {Math.Round(i++ / updatesLocalParaRemoto.Count * 100, 2)}%");
                        }
                        Console.WriteLine($"{typeof(E).Name} - Atualizando de banco local para remoto -> Inserindo dados -> Progresso: {Math.Round(i++ / updatesLocalParaRemoto.Count * 100, 2)}%");
                        await tx.CommitAsync();
                    }
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
