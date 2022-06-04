using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarDevolucao : ASincronizar<Devolucao>
    {
        public SincronizarDevolucao(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar()
        {
            IList<Devolucao> insertsRemotoParaLocal = new List<Devolucao>();
            IList<Devolucao> updatesRemotoParaLocal = new List<Devolucao>();
            IList<Devolucao> insertsLocalParaRemoto = new List<Devolucao>();
            IList<Devolucao> updatesLocalParaRemoto = new List<Devolucao>();

            Console.WriteLine($"Iniciando sincronização de Devolução");

            var inicioSync = DateTime.Now;
            var lastSync = await GetLastSyncTime("devolucao");

            if (lastSync == null)
            {
                lastSync = new LastSync
                {
                    Tabela = "devolucao",
                    LastSyncTime = DateTime.MinValue
                };
            }

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Devolucao>(e.Uuid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Devolucao eASalvar = new Devolucao();
                    eASalvar.Copiar(e);

                    Pix pixLocal = null;
                    Horario horarioLocal = null;

                    if (e.Pix != null)
                        pixLocal = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    if (e.Horario != null)
                        horarioLocal = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    if (pixLocal == null)
                        throw new Exception("Pix não pode ser nulo");
                    if (horarioLocal == null)
                        throw new Exception("Horario não pode ser nulo");

                    eASalvar.Pix = pixLocal;
                    eASalvar.Horario = horarioLocal;

                    insertsRemotoParaLocal.Add(eASalvar);
                }

                await InsertRemotoParaLocal(insertsRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista devoluções em banco remoto para insert em local");
                throw;
            }

            //Lista entidades em banco remoto para update em local
            try
            {
                var criteria = _remoto.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorUuidLocal<Devolucao>(e.Uuid);
                    if (entLocal == null) continue;
                    entLocal.Copiar(e);

                    Pix pixLocal = null;
                    Horario horarioLocal = null;

                    if (e.Pix != null)
                        pixLocal = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    if (e.Horario != null)
                        horarioLocal = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    if (pixLocal == null)
                        throw new Exception("Pix não pode ser nulo");
                    if (horarioLocal == null)
                        throw new Exception("Horario não pode ser nulo");

                    entLocal.Pix = pixLocal;
                    entLocal.Horario = horarioLocal;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(entLocal);
                }

                await UpdateRemotoParaLocal(updatesRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista devoluções em banco remoto para update em local");
                throw;
            }

            //Lista entidades em banco local para insert em remoto
            try
            {
                var criteria = _local.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidRemoto<Devolucao>(e.Uuid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Devolucao eASalvar = new Devolucao();
                    eASalvar.Copiar(e);

                    Pix pixRemoto = null;
                    Horario horarioRemoto = null;

                    if (e.Pix != null)
                        pixRemoto = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    if (e.Horario != null)
                        horarioRemoto = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    if (pixRemoto == null)
                        throw new Exception("Pix não pode ser nulo");
                    if (horarioRemoto == null)
                        throw new Exception("Horario não pode ser nulo");

                    eASalvar.Pix = pixRemoto;
                    eASalvar.Horario = horarioRemoto;

                    insertsLocalParaRemoto.Add(eASalvar);
                }

                await InsertLocalParaRemoto(insertsLocalParaRemoto);
            }
            catch (Exception ex)
            {
                Log.EscreveLogSync(ex, "Lista devoluções em banco local para insert em remoto");
                Console.WriteLine(ex.Message, ex);
                throw;
            }

            //Lista entidades em banco local para update em remoto
            try
            {
                var criteria = _local.CreateCriteria<Devolucao>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Devolucao>();

                foreach (Devolucao e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorUuidRemoto<Devolucao>(e.Uuid);
                    if (entRemoto == null) continue;
                    entRemoto.Copiar(e);

                    Pix pixRemoto = null;
                    Horario horarioRemoto = null;

                    if (e.Pix != null)
                        pixRemoto = await ListarPorUuidLocal<Pix>(e.Pix.Uuid);
                    if (e.Horario != null)
                        horarioRemoto = await ListarPorUuidLocal<Horario>(e.Horario.Uuid);

                    if (pixRemoto == null)
                        throw new Exception("Pix não pode ser nulo");
                    if (horarioRemoto == null)
                        throw new Exception("Horario não pode ser nulo");

                    entRemoto.Pix = pixRemoto;
                    entRemoto.Horario = horarioRemoto;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(entRemoto);
                }

                await UpdateRemotoParaLocal(updatesRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista devoluções em banco local para update em remoto");
                throw;
            }

            await SaveLastSyncTime(lastSync, inicioSync);
        }
    }
}
