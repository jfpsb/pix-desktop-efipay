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

        public virtual async Task Inserir(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    var result = await session.SaveAsync(objeto);
                    await transacao.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "inserir único");
                    throw new Exception($"Erro ao inserir em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
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
                    throw new Exception($"Erro ao inserir lista em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
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
                    throw new Exception($"Erro ao inserir ou atualizar em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
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
                    throw new Exception($"Erro ao inserir ou atualizar lista em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public virtual async Task Atualizar(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    await session.UpdateAsync(objeto);
                    await transacao.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "atualizar único");
                    throw new Exception($"Erro ao atualizar em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
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
                    throw new Exception($"Erro ao realizar merge em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
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
                    throw new Exception($"Erro ao realizar merge em lista em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public virtual async Task<bool> Deletar(object objeto)
        {
            using (var transacao = session.BeginTransaction())
            {
                try
                {
                    await session.DeleteAsync(objeto);
                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "deletar único");
                    throw new Exception($"Erro ao deletar em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
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
                        await session.DeleteAsync(e);
                    }

                    await transacao.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transacao.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "deletar lista");
                    throw new Exception($"Erro ao deletar lista em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public virtual async Task<IList<E>> Listar(string orderBy = null)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
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
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar");
                    throw new Exception($"Erro ao listar em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        /// <summary>
        /// Retorna Uma Lista De Itens Baseado No Criteria Informado E Que Não Estejam Deletados
        /// </summary>
        /// <param name="criteria">Criteria Para Ser Usado Na Query</param>
        /// <returns>Lista De Itens Do Tipo E</returns>
        public virtual async Task<IList<E>> Listar(ICriteria criteria)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    //criteria.Add(Restrictions.Eq("Deletado", false));
                    criteria.SetCacheable(true);
                    criteria.SetCacheMode(CacheMode.Normal);
                    var results = await criteria.ListAsync<E>();
                    await tx.CommitAsync();
                    return results;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar");
                    throw new Exception($"Erro ao listar em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public virtual async Task<IList<E>> ListarComNovaSession(ICriteria criteria)
        {
            using (ISession session = SessionProvider.GetSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    try
                    {

                        //criteria.Add(Restrictions.Eq("Deletado", false));
                        criteria.SetCacheable(true);
                        criteria.SetCacheMode(CacheMode.Normal);
                        var results = await criteria.ListAsync<E>();
                        await tx.CommitAsync();
                        return results;
                    }
                    catch (Exception ex)
                    {
                        await tx.RollbackAsync();
                        Log.EscreveLogBancoLocal(ex, "listar com nova session");
                        throw new Exception($"Erro ao listar com nova session em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                    }
                }
            }
        }
        public async Task<E> ListarPorId(object id)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    return await session.GetAsync<E>(id);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar por id");
                    throw new Exception($"Erro ao listar por id em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public async Task<long> RetornaMaiorValor(string idProperty)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    var criteria = session.CreateCriteria<E>();
                    criteria.SetProjection(Projections.Max(idProperty));
                    return await criteria.UniqueResultAsync<long>();

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "retorna maior valor");
                    throw new Exception($"Erro ao retornar maior valor em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
        public async Task RefreshEntidade(object obj)
        {
            try
            {
                await session.RefreshAsync(obj);
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "refresh");
                throw new Exception($"Erro ao dar refresh em entidade em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
            }
        }
        public async Task RefreshEntidade(IList<E> objs)
        {
            try
            {
                foreach (var obj in objs)
                    await session.RefreshAsync(obj);
            }
            catch (Exception ex)
            {
                Log.EscreveLogBancoLocal(ex, "refresh");
                throw new Exception($"Erro ao dar refresh em lista de entidades em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
            }
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
