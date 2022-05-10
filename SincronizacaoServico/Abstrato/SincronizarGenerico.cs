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
        public SincronizarGenerico(ISession local, ISession remoto, bool isChaveAutoIncremento = true) : base(local, remoto, isChaveAutoIncremento)
        {
        }

        public async override Task Sincronizar(DateTime lastSync)
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
                throw;
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
                throw;
            }

            //Lista entidades em banco local para insert em remoto
            try
            {
                var criteria = _local.CreateCriteria<E>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<E>();

                foreach (E e in lista)
                {
                    //Entidade com mesmo UUID no banco remoto
                    var ent = await ListarPorUuidRemoto<E>(e.Uuid);

                    if (ent == null)
                    {
                        insertsLocalParaRemoto.Add(e);
                        continue;
                    }

                    if (_isChaveAutoIncremento)
                        e.SetIdentifierToDefault();
                    e.Uuid = Guid.NewGuid();

                    insertsLocalParaRemoto.Add(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                throw;
            }

            //Lista entidades em banco local para update em remoto
            try
            {
                var criteria = _local.CreateCriteria<E>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<E>();

                foreach (E e in lista)
                {
                    //Entidade com mesmo UUID no banco remoto
                    var ent = await ListarPorUuidRemoto<E>(e.Uuid);

                    if (ent == null)
                    {
                        insertsLocalParaRemoto.Add(e);
                        continue;
                    }

                    //Copia os dados do banco local para atualizar no remoto, preservando uuid e id remota
                    ent.Copiar(e);
                    //Coloca na coleção que será salva
                    updatesLocalParaRemoto.Add(ent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                throw;
            }

            if (insertsRemotoParaLocal.Count > 0)
            {
                Console.WriteLine($"Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                using (var tx = _local.BeginTransaction())
                {
                    foreach (E insert in insertsRemotoParaLocal)
                    {
                        if (_isChaveAutoIncremento)
                            insert.SetIdentifierToDefault();
                        await _local.SaveAsync(insert);
                    }

                    await tx.CommitAsync();
                }
            }

            if (updatesRemotoParaLocal.Count > 0)
            {
                Console.WriteLine($"Atualizando {insertsRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                using (var tx = _local.BeginTransaction())
                {
                    foreach (E update in updatesRemotoParaLocal)
                    {
                        await _local.UpdateAsync(update);
                    }

                    await tx.CommitAsync();
                }
            }

            if (insertsLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Inserindo {insertsRemotoParaLocal.Count} registro(s) do banco local para o remoto.");
                using (var tx = _remoto.BeginTransaction())
                {
                    foreach (E insert in insertsLocalParaRemoto)
                    {
                        if (_isChaveAutoIncremento)
                            insert.SetIdentifierToDefault();
                        await _remoto.SaveAsync(insert);
                    }
                    await tx.CommitAsync();
                }
            }

            if (updatesLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Atualizando {insertsRemotoParaLocal.Count} registro(s) do banco local para o remoto.");
                using (var tx = _remoto.BeginTransaction())
                {
                    foreach (E update in updatesLocalParaRemoto)
                    {
                        await _remoto.UpdateAsync(update);
                    }
                    await tx.CommitAsync();
                }
            }
        }
    }
}
