using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMIClientePix.Model.DAO
{
    public class DAOCobranca : DAO<Cobranca>
    {
        public DAOCobranca(ISession session) : base(session)
        {

        }

        public async override Task<IList<Cobranca>> Listar(string orderBy = null)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public async Task<IList<Cobranca>> ListarPorDia(DateTime dia)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var criteria = CriarCriteria();
                    criteria.CreateAlias("Calendario", "Calendario");
                    criteria.AddOrder(Order.Asc("Calendario.Criacao"));
                    criteria.Add(Restrictions.Between("Calendario.Criacao", dia.Date.ToUniversalTime(), dia.Date.ToUniversalTime().AddDays(1).AddSeconds(-1)));

                    return await Listar(criteria);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
