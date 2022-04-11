using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VMIClientePix.BancoDeDados.ConnectionFactory;
using VMIClientePix.Model;
using VMIClientePix.Util;

namespace VMIClientePix.BancoDeDados.BackupRemoto
{
    public class Sync
    {
        public Sync()
        {
        }

        public static readonly string ArquivoLog = Path.Combine("Logs", "SyncLog.txt");

        public static async Task<bool> Sincronizar<E>() where E : AModel
        {
            DateTime ultCriadoEm, ultModificadoEm;
            try
            {
                ultCriadoEm = await GetLastSyncTime<E>("CriadoEm");
                ultModificadoEm = await GetLastSyncTime<E>("ModificadoEm");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO RECUPERAR DATAS DE BANCO REMOTO.\n{ex.Message}");
                File.AppendAllText(ArquivoLog, $"\nOperação: GETLASTSYNC\nData/Hora: {DateTime.Now}\nEntidade: {typeof(E).FullName}\nERRO AO RECUPEAR DATAS DE BANCO REMOTO.\n{ex.Message}");
                return false;
            }

            IList<E> inserts = new List<E>();
            IList<E> updates = new List<E>();

            using (var session = SessionProvider.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    try
                    {
                        var criteria = session.CreateCriteria<E>();
                        criteria.Add(Restrictions.Gt("CriadoEm", ultCriadoEm));
                        inserts = await criteria.ListAsync<E>();
                        await tx.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.EscreveLogBancoLocal(ex, "listar inserts criadoem");
                    }
                }
            }

            using (var session = SessionProvider.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    try
                    {
                        var criteria = session.CreateCriteria<E>();
                        criteria.Add(Restrictions.Gt("ModificadoEm", ultModificadoEm));
                        updates = await criteria.ListAsync<E>();
                        await tx.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.EscreveLogBancoLocal(ex, "listar updates criadoem");
                    }
                }
            }

            try
            {
                if (inserts.Count > 0)
                {
                    using (var session = SessionProviderBackup.GetSession())
                    {
                        using (var tx = session.BeginTransaction())
                        {
                            foreach (var insert in inserts)
                            {
                                await session.ReplicateAsync(insert, ReplicationMode.Overwrite);
                            }

                            await tx.CommitAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = $"\nOperação: INSERT\nData/Hora: {DateTime.Now}\nEntidade: {typeof(E).FullName}\n{ex.Message}\n";
                if (ex.InnerException != null)
                {
                    msg += $"InnerException: \n{ex.InnerException.Message}\n\n";
                }
                File.AppendAllText(ArquivoLog, msg);
                return false;
            }

            try
            {
                if (updates.Count > 0)
                {
                    using (var session = SessionProviderBackup.GetSession())
                    {
                        using (var tx = session.BeginTransaction())
                        {
                            foreach (var update in updates)
                            {
                                await session.ReplicateAsync(update, ReplicationMode.Overwrite);
                            }

                            await tx.CommitAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = $"\nOperação: UPDATE\nData/Hora: {DateTime.Now}\nEntidade: {typeof(E).FullName}\n{ex.Message}\n";
                if (ex.InnerException != null)
                {
                    msg += $"InnerException: \n{ex.InnerException.Message}\n\n";
                }
                File.AppendAllText(ArquivoLog, msg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propriedade"></param>
        /// <param name="entidade"></param>
        /// <returns></returns>
        private static async Task<DateTime> GetLastSyncTime<E>(string propriedade) where E : AModel
        {
            using (var session = SessionProviderBackup.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var criteria = session.CreateCriteria<E>();
                    criteria.SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property(propriedade), propriedade));
                    criteria.AddOrder(Order.Desc(propriedade));
                    criteria.SetMaxResults(1);
                    return await criteria.UniqueResultAsync<DateTime>();
                }
            }
        }
    }
}
