using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarPix : ASincronizar<Pix>
    {
        public SincronizarPix(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar()
        {
            IList<Pix> insertsRemotoParaLocal = new List<Pix>();
            IList<Pix> updatesRemotoParaLocal = new List<Pix>();
            IList<Pix> insertsLocalParaRemoto = new List<Pix>();
            IList<Pix> updatesLocalParaRemoto = new List<Pix>();

            Console.WriteLine($"Iniciando sincronização de Pix");

            var inicioSync = DateTime.Now;
            var lastSync = await GetLastSyncTime("pix");

            if (lastSync == null)
            {
                lastSync = new LastSync
                {
                    Tabela = "pix",
                    LastSyncTime = DateTime.MinValue
                };
            }

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Pix>(e.Uuid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Pix eASalvar = new Pix();
                    eASalvar.Copiar(e);

                    Pagador pagadorLocal = null;
                    Cobranca cobrancaLocal = null;

                    if (e.Pagador != null)
                        pagadorLocal = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);

                    if (e.Cobranca != null)
                        cobrancaLocal = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    if (eASalvar.Txid != null && cobrancaLocal == null)
                        throw new Exception("O PIX possui TXID mas a cobrança está nula");

                    eASalvar.Pagador = pagadorLocal;
                    eASalvar.Cobranca = cobrancaLocal;

                    insertsRemotoParaLocal.Add(eASalvar);
                }

                await InsertRemotoParaLocal(insertsRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista pix em banco remoto para insert em local");
                throw;
            }

            //Lista entidades em banco remoto para update em local
            try
            {
                var criteria = _remoto.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorUuidLocal<Pix>(e.Uuid);
                    if (entLocal == null) continue;
                    entLocal.Copiar(e);

                    Pagador pagadorLocal = null;
                    Cobranca cobrancaLocal = null;

                    if (e.Pagador != null)
                        pagadorLocal = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);

                    if (e.Cobranca != null)
                        cobrancaLocal = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    entLocal.Pagador = pagadorLocal;
                    entLocal.Cobranca = cobrancaLocal;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(entLocal);
                }

                await UpdateRemotoParaLocal(updatesRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista pix em banco remoto para update em local");
                throw;
            }

            //Lista entidades em banco local para insert em remoto
            try
            {
                var criteria = _local.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidRemoto<Pix>(e.Uuid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Pix eASalvar = new Pix();
                    eASalvar.Copiar(e);

                    Pagador pagadorRemoto = null;
                    Cobranca cobrancaRemoto = null;

                    if (e.Pagador != null)
                        pagadorRemoto = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);

                    if (e.Cobranca != null)
                        cobrancaRemoto = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    if (eASalvar.Txid != null && cobrancaRemoto == null)
                        throw new Exception("O PIX possui TXID mas a cobrança está nula");

                    eASalvar.Pagador = pagadorRemoto;
                    eASalvar.Cobranca = cobrancaRemoto;

                    insertsLocalParaRemoto.Add(eASalvar);
                }

                await InsertLocalParaRemoto(insertsLocalParaRemoto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista pix em banco local para insert em remoto");
                throw;
            }

            //Lista entidades em banco local para update em remoto
            try
            {
                var criteria = _local.CreateCriteria<Pix>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Pix>();

                foreach (Pix e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorUuidRemoto<Pix>(e.Uuid);
                    if (entRemoto == null) continue;
                    entRemoto.Copiar(e);

                    Pagador pagadorRemoto = null;
                    Cobranca cobrancaRemoto = null;

                    if (e.Pagador != null)
                        pagadorRemoto = await ListarPorUuidRemoto<Pagador>(e.Pagador.Uuid);

                    if (e.Cobranca != null)
                        cobrancaRemoto = await ListarPorUuidRemoto<Cobranca>(e.Cobranca.Uuid);

                    entRemoto.Pagador = pagadorRemoto;
                    entRemoto.Cobranca = cobrancaRemoto;

                    //Coloca na coleção que será salva
                    updatesLocalParaRemoto.Add(entRemoto);
                }

                await UpdateLocalParaRemoto(updatesLocalParaRemoto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista pix em banco local para update em remoto");
                throw;
            }

            await SaveLastSyncTime(lastSync, inicioSync);
        }
    }
}
