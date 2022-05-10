using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Model;

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

        public abstract Task Sincronizar(DateTime lastSync);

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
    }
}
