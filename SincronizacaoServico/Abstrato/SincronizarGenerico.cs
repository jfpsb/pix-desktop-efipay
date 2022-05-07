using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Model;

namespace SincronizacaoServico.Interfaces
{
    /// <summary>
    /// Usado para sincronizar entidades sem chaves estrangeiras.
    /// </summary>
    public class SincronizarGenerico<E> : ASincronizar<E> where E : AModel
    {
        public SincronizarGenerico(ISession local, ISession remoto, bool isChaveAutoIncremento) : base(local, remoto, isChaveAutoIncremento)
        {
        }

        public async override Task<bool> Sincronizar(DateTime lastSync)
        {
            IList<E> insertsRemotoParaLocal = new List<E>();
            IList<E> updatesRemotoParaLocal = new List<E>();
            IList<E> insertsLocalParaRemoto = new List<E>();
            IList<E> updatesLocalParaRemoto = new List<E>();

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<E>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<E>();

                foreach (E e in lista)
                {
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<E>(e.Uuid);

                    if (ent != null)
                    {
                        e.Uuid = Guid.NewGuid();
                    }

                    if (_isChaveAutoIncremento)
                        e.SetIdentifierToDefault();

                    insertsRemotoParaLocal.Add(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return false;
            }

            //Lista entidades em banco remoto para update em local
            try
            {
                var criteria = _remoto.CreateCriteria<E>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<E>();

                foreach (E e in lista)
                {
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<E>(e.Uuid);

                    //Entidade que será atualizada não existe no banco local, então é criada
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
                return false;
            }

            return true;
        }
    }
}
