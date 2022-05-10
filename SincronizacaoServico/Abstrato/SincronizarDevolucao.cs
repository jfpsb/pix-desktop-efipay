using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarDevolucao : ASincronizar<Devolucao>
    {
        public SincronizarDevolucao(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar(DateTime lastSync)
        {
            IList<Devolucao> insertsRemotoParaLocal = new List<Devolucao>();
            IList<Devolucao> updatesRemotoParaLocal = new List<Devolucao>();
            IList<Devolucao> insertsLocalParaRemoto = new List<Devolucao>();
            IList<Devolucao> updatesLocalParaRemoto = new List<Devolucao>();

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {

                    Devolucao eASalvar = new Devolucao();
                    eASalvar.Copiar(e);

                    Pix pixLocal = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    Horario horarioLocal = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    eASalvar.Pix = pixLocal;
                    eASalvar.Horario = horarioLocal;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Devolucao>(e.Uuid);
                    if (ent != null)
                    {
                        eASalvar.Uuid = Guid.NewGuid();
                    }

                    insertsRemotoParaLocal.Add(eASalvar);
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
                var criteria = _remoto.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    Devolucao eASalvar = new Devolucao();
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorUuidLocal<Devolucao>(e.Uuid);
                    eASalvar.Copiar(entLocal);

                    Pix pixLocal = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    Horario horarioLocal = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    eASalvar.Pix = pixLocal;
                    eASalvar.Horario = horarioLocal;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(eASalvar);
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
                var criteria = _local.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    Devolucao eASalvar = new Devolucao();
                    eASalvar.Copiar(e);

                    Pix pixRemoto = await ListarPorUuidRemoto<Pix>(e.Pix.Uuid);
                    Horario horarioRemoto = await ListarPorUuidRemoto<Horario>(e.Horario.Uuid);

                    eASalvar.Pix = pixRemoto;
                    eASalvar.Horario = horarioRemoto;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidRemoto<Devolucao>(e.Uuid);
                    if (ent != null)
                    {
                        eASalvar.Uuid = Guid.NewGuid();
                    }

                    insertsLocalParaRemoto.Add(eASalvar);
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
                var criteria = _local.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    Devolucao eASalvar = new Devolucao();
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorUuidRemoto<Devolucao>(e.Uuid);
                    eASalvar.Copiar(entRemoto);

                    Pix pixRemoto = await ListarPorUuidRemoto<Pix>(e.Pix.Uuid);
                    Horario horarioRemoto = await ListarPorUuidRemoto<Horario>(e.Horario.Uuid);

                    eASalvar.Pix = pixRemoto;
                    eASalvar.Horario = horarioRemoto;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(eASalvar);
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
                    foreach (Devolucao insert in insertsRemotoParaLocal)
                    {
                        await _local.SaveAsync(insert);
                    }

                    await tx.CommitAsync();
                }
            }

            if (updatesRemotoParaLocal.Count > 0)
            {
                Console.WriteLine($"Atualizando {updatesRemotoParaLocal.Count} registro(s) do banco remoto para o local.");
                using (var tx = _local.BeginTransaction())
                {
                    foreach (Devolucao update in updatesRemotoParaLocal)
                    {
                        await _local.UpdateAsync(update);
                    }

                    await tx.CommitAsync();
                }
            }

            if (insertsLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Inserindo {insertsLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                using (var tx = _remoto.BeginTransaction())
                {
                    foreach (Devolucao insert in insertsLocalParaRemoto)
                    {
                        await _remoto.SaveAsync(insert);
                    }
                    await tx.CommitAsync();
                }
            }

            if (updatesLocalParaRemoto.Count > 0)
            {
                Console.WriteLine($"Atualizando {updatesLocalParaRemoto.Count} registro(s) do banco local para o remoto.");
                using (var tx = _remoto.BeginTransaction())
                {
                    foreach (Devolucao update in updatesLocalParaRemoto)
                    {
                        await _remoto.UpdateAsync(update);
                    }
                    await tx.CommitAsync();
                }
            }
        }
    }
}
