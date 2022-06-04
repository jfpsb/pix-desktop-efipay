using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMIClientePix.Util;

namespace VMIClientePix.Model.DAO
{
    public class DAOCobranca : DAO<Cobranca>
    {
        public DAOCobranca(ISession session) : base(session)
        {

        }

        public async override Task<IList<Cobranca>> Listar(string orderBy = null)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    var criteria = CriarCriteria();
                    criteria.CreateAlias("Calendario", "Calendario");
                    criteria.AddOrder(Order.Asc("Calendario.Criacao"));
                    criteria.SetCacheable(true);
                    criteria.SetCacheMode(CacheMode.Normal);
                    var results = await criteria.ListAsync<Cobranca>();
                    await tx.CommitAsync();
                    return results;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar em daocobranca");
                    throw new Exception($"Erro ao listar daocobranca em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }

        public async Task<IList<Cobranca>> ListarPorDia(DateTime dia)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    var criteria = CriarCriteria();
                    criteria.CreateAlias("Calendario", "Calendario");
                    criteria.AddOrder(Order.Asc("Calendario.Criacao"));
                    criteria.Add(Restrictions.Between("Calendario.Criacao", dia.Date.ToUniversalTime(), dia.Date.ToUniversalTime().AddDays(1).AddSeconds(-1)));
                    return await Listar(criteria);

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar por dia em daocobranca");
                    throw new Exception($"Erro ao listar por dia em daocobranca em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
    }
}
