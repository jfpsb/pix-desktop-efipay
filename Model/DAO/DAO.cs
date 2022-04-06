using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMIClientePix.BancoDeDados.ConnectionFactory;
using VMIClientePix.Util;

namespace VMIClientePix.Model.DAO
{
    public class DAO<E> where E : class
    {
        protected ISession session;
        public DAO(ISession session)
        {
            this.session = session;
        }

        public virtual async Task<bool> Inserir(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    var result = await session.SaveAsync(objeto);
                    await transacao.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "inserir único");
                }

                return false;
            }
        }
        public virtual async Task<bool> Inserir(IList<E> objetos)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    foreach (E e in objetos)
                    {
                        await session.SaveOrUpdateAsync(e);
                    }

                    await transacao.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "inserir lista");
                }

                return false;
            }
        }
        public virtual async Task<bool> InserirOuAtualizar(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    await session.SaveOrUpdateAsync(objeto);
                    await transacao.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "inserir ou atualizar único");
                }

                return false;
            }
        }
        public virtual async Task<bool> InserirOuAtualizar(IList<E> objetos)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    foreach (E e in objetos)
                    {
                        await session.SaveOrUpdateAsync(e);
                    }

                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "inserir ou atualizar lista");
                }

                return false;
            }
        }
        public virtual async Task<bool> Atualizar(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    await session.UpdateAsync(objeto);
                    await transacao.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "atualizar único");
                }

                return false;
            }
        }
        public virtual async Task<bool> Merge(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    await session.MergeAsync(objeto);
                    await transacao.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "merge único");
                    return false;
                }
            }
        }
        public virtual async Task<bool> Merge(IList<E> objetos)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    foreach (E e in objetos)
                    {
                        await session.MergeAsync(e);
                    }

                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "merge lista");
                }

                return false;
            }
        }
        public virtual async Task<bool> Deletar(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    //AModel model = objeto as AModel;
                    //model.Deletado = true;
                    await session.DeleteAsync(objeto);
                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "deletar único");
                }

                return false;
            }
        }
        public virtual async Task<bool> Deletar(IList<E> objetos)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    foreach (E e in objetos)
                    {
                        //AModel model = e as AModel;
                        //model.Deletado = true;
                        await session.DeleteAsync(e);
                    }

                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "deletar lista");
                }

                return false;
            }
        }
        public virtual async Task<IList<E>> Listar(string orderBy = null)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var criteria = CriarCriteria();
                    if (orderBy != null)
                    {
                        criteria.AddOrder(Order.Asc(orderBy));
                    }
                    criteria.SetCacheable(true);
                    criteria.SetCacheMode(CacheMode.Normal);
                    var results = await criteria.ListAsync<E>();
                    await tx.CommitAsync();
                    return results;
                }
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "listar");
            }

            return null;
        }
        /// <summary>
        /// Retorna Uma Lista De Itens Baseado No Criteria Informado E Que Não Estejam Deletados
        /// </summary>
        /// <param name="criteria">Criteria Para Ser Usado Na Query</param>
        /// <returns>Lista De Itens Do Tipo E</returns>
        public virtual async Task<IList<E>> Listar(ICriteria criteria)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    //criteria.Add(Restrictions.Eq("Deletado", false));
                    criteria.SetCacheable(true);
                    criteria.SetCacheMode(CacheMode.Normal);
                    var results = await criteria.ListAsync<E>();
                    await tx.CommitAsync();
                    return results;
                }
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "listar");
            }

            return null;
        }
        public virtual async Task<IList<E>> ListarComNovaSession(ICriteria criteria)
        {
            try
            {
                using (ISession session = SessionProvider.GetSession())
                {
                    using (ITransaction tx = session.BeginTransaction())
                    {
                        //criteria.Add(Restrictions.Eq("Deletado", false));
                        criteria.SetCacheable(true);
                        criteria.SetCacheMode(CacheMode.Normal);
                        var results = await criteria.ListAsync<E>();
                        await tx.CommitAsync();
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "listar com nova session");
            }

            return null;
        }
        public async Task<E> ListarPorId(object id)
        {
            return await session.GetAsync<E>(id);
        }
        public async Task<long> RetornaMaiorValor(string idProperty)
        {
            var criteria = session.CreateCriteria<E>();
            criteria.SetProjection(Projections.Max(idProperty));
            return await criteria.UniqueResultAsync<long>();
        }
        public ICriteria CriarCriteria()
        {
            return session.CreateCriteria<E>();
        }

        public ICriteria CriarCriteria(string alias)
        {
            return session.CreateCriteria<E>(alias);
        }
    }
}
