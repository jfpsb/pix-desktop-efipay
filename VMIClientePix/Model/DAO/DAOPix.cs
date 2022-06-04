using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMIClientePix.Util;

namespace VMIClientePix.Model.DAO
{
    public class DAOPix : DAO<Pix>
    {
        public DAOPix(ISession session) : base(session)
        {
        }

        public async override Task<IList<Pix>> Listar(string orderBy = null)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    var criteria = CriarCriteria();
                    criteria.AddOrder(Order.Asc("Horario"));
                    criteria.SetCacheable(true);
                    criteria.SetCacheMode(CacheMode.Normal);
                    var results = await criteria.ListAsync<Pix>();
                    await tx.CommitAsync();
                    return results;

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar em daopix");
                    throw new Exception($"Erro ao listar daopix em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }

        public async Task<IList<Pix>> ListarPorDiaPorChave(DateTime dia, string chaveEstatica)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {

                    var criteria = CriarCriteria();
                    criteria.AddOrder(Order.Asc("Horario"));
                    criteria.Add(Restrictions.Between("Horario", dia.Date.ToUniversalTime(), dia.Date.ToUniversalTime().AddDays(1).AddSeconds(-1)));
                    criteria.Add(Restrictions.Eq("Chave", chaveEstatica));

                    return await Listar(criteria);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    Log.EscreveLogBancoLocal(ex, "listar por dia em daopix");
                    throw new Exception($"Erro ao listar por dia em daopix em banco de dados local. Acesse {Log.LogLocal} para mais detalhes.", ex);
                }
            }
        }
    }
}
