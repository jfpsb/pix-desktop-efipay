using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarPix : ASincronizar<Pix>
    {
        public SincronizarPix(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar(DateTime lastSync)
        {
            IList<Pix> insertsRemotoParaLocal = new List<Pix>();
            IList<Pix> updatesRemotoParaLocal = new List<Pix>();
            IList<Pix> insertsLocalParaRemoto = new List<Pix>();
            IList<Pix> updatesLocalParaRemoto = new List<Pix>();

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {

                    Pix eASalvar = new Pix();
                    eASalvar.Copiar(e);

                    Pagador pagadorLocal = await ListarPorUuidLocal<Pagador>(e.Pagador.Uuid);
                    Cobranca cobrancaLocal = await ListarPorUuidLocal<Cobranca>(e.Cobranca.Uuid);

                    eASalvar.Pagador = pagadorLocal;
                    eASalvar.Cobranca = cobrancaLocal;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Pix>(e.Uuid);
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
                var criteria = _remoto.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    Pix eASalvar = new Pix();
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorUuidLocal<Pix>(e.Uuid);
                    eASalvar.Copiar(entLocal);

                    Pagador pagadorLocal = await ListarPorUuidLocal<Pagador>(e.Pagador.Uuid);
                    Cobranca cobrancaLocal = await ListarPorUuidLocal<Cobranca>(e.Cobranca.Uuid);

                    eASalvar.Pagador = pagadorLocal;
                    eASalvar.Cobranca = cobrancaLocal;

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
                var criteria = _local.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    Pix eASalvar = new Pix();
                    eASalvar.Copiar(e);

                    Pagador pagadorRemoto = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);
                    Cobranca cobrancaRemoto = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    eASalvar.Pagador = pagadorRemoto;
                    eASalvar.Cobranca = cobrancaRemoto;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidRemoto<Pix>(e.Uuid);
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
                var criteria = _local.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    Pix eASalvar = new Pix();
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorUuidRemoto<Pix>(e.Uuid);
                    eASalvar.Copiar(entRemoto);

                    Pagador pagadorRemoto = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);
                    Cobranca cobrancaRemoto = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    eASalvar.Pagador = pagadorRemoto;
                    eASalvar.Cobranca = cobrancaRemoto;

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
                    foreach (Pix insert in insertsRemotoParaLocal)
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
                    foreach (Pix update in updatesRemotoParaLocal)
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
                    foreach (Pix insert in insertsLocalParaRemoto)
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
                    foreach (Pix update in updatesLocalParaRemoto)
                    {
                        await _remoto.UpdateAsync(update);
                    }
                    await tx.CommitAsync();
                }
            }
        }
    }
}
