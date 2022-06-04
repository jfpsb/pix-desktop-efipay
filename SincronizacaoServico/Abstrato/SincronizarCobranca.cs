using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarCobranca : ASincronizar<Cobranca>
    {
        public SincronizarCobranca(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar()
        {
            IList<Cobranca> insertsRemotoParaLocal = new List<Cobranca>();
            IList<Cobranca> updatesRemotoParaLocal = new List<Cobranca>();
            IList<Cobranca> insertsLocalParaRemoto = new List<Cobranca>();
            IList<Cobranca> updatesLocalParaRemoto = new List<Cobranca>();

            Console.WriteLine($"Iniciando sincronização de Cobrança");

            var inicioSync = DateTime.Now;
            var lastSync = await GetLastSyncTime("cobranca");

            if (lastSync == null)
            {
                lastSync = new LastSync
                {
                    Tabela = "cobranca",
                    LastSyncTime = DateTime.MinValue
                };
            }

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorIdLocal<Cobranca>(e.Txid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Cobranca eASalvar = new Cobranca();
                    eASalvar.Copiar(e);

                    Calendario calendarioLocal = await ListarPorUuidLocal<Calendario>(e.Calendario.Uuid);
                    Valor valorLocal = await ListarPorUuidLocal<Valor>(e.Valor.Uuid);
                    Loc locLocal = await ListarPorUuidLocal<Loc>(e.Loc.Uuid);
                    QRCode qrcodeLocal = null;
                    if (e.QrCode != null)
                        qrcodeLocal = await ListarPorUuidLocal<QRCode>(e.QrCode.Uuid);

                    if (calendarioLocal == null)
                        throw new Exception("Calendario não pode ser nulo");
                    if (valorLocal == null)
                        throw new Exception("Valor não pode ser nulo");
                    if (locLocal == null)
                        throw new Exception("Loc não pode ser nulo");

                    eASalvar.Calendario = calendarioLocal;
                    eASalvar.Valor = valorLocal;
                    eASalvar.Loc = locLocal;
                    eASalvar.QrCode = qrcodeLocal;

                    insertsRemotoParaLocal.Add(eASalvar);
                }

                await InsertRemotoParaLocal(insertsRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista cobranças em banco remoto para insert em local");
                throw;
            }

            //Lista entidades em banco remoto para update em local
            try
            {
                var criteria = _remoto.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorIdLocal<Cobranca>(e.Txid);
                    if (entLocal == null) continue;
                    entLocal.Copiar(e);

                    Calendario calendarioLocal = await ListarPorUuidLocal<Calendario>(e.Calendario.Uuid);
                    Valor valorLocal = await ListarPorUuidLocal<Valor>(e.Valor.Uuid);
                    Loc locLocal = await ListarPorUuidLocal<Loc>(e.Loc.Uuid);
                    QRCode qrcodeLocal = null;
                    if (e.QrCode != null)
                        qrcodeLocal = await ListarPorUuidLocal<QRCode>(e.QrCode.Uuid);

                    if (calendarioLocal == null)
                        throw new Exception("Calendario não pode ser nulo");
                    if (valorLocal == null)
                        throw new Exception("Valor não pode ser nulo");
                    if (locLocal == null)
                        throw new Exception("Loc não pode ser nulo");

                    entLocal.Calendario = calendarioLocal;
                    entLocal.Valor = valorLocal;
                    entLocal.Loc = locLocal;
                    entLocal.QrCode = qrcodeLocal;

                    //Coloca na coleção que será salva
                    updatesRemotoParaLocal.Add(entLocal);
                }

                await UpdateRemotoParaLocal(updatesRemotoParaLocal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista cobranças em banco remoto para update em local");
                throw;
            }

            //Lista entidades em banco local para insert em remoto
            try
            {
                var criteria = _local.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorIdRemoto<Cobranca>(e.Txid);
                    if (ent != null)
                    {
                        continue;
                    }

                    Cobranca eASalvar = new Cobranca();
                    eASalvar.Copiar(e);

                    Calendario calendarioRemoto = await ListarPorUuidRemoto<Calendario>(eASalvar.Calendario.Uuid);
                    Valor valorRemoto = await ListarPorUuidRemoto<Valor>(eASalvar.Valor.Uuid);
                    Loc locRemoto = await ListarPorUuidRemoto<Loc>(eASalvar.Loc.Uuid);
                    QRCode qrcodeRemoto = null;
                    if (eASalvar.QrCode != null)
                        qrcodeRemoto = await ListarPorUuidRemoto<QRCode>(eASalvar.QrCode.Uuid);

                    if (calendarioRemoto == null)
                        throw new Exception("Calendario não pode ser nulo");
                    if (valorRemoto == null)
                        throw new Exception("Valor não pode ser nulo");
                    if (locRemoto == null)
                        throw new Exception("Loc não pode ser nulo");

                    eASalvar.Calendario = calendarioRemoto;
                    eASalvar.Valor = valorRemoto;
                    eASalvar.Loc = locRemoto;
                    eASalvar.QrCode = qrcodeRemoto;

                    insertsLocalParaRemoto.Add(eASalvar);
                }

                await InsertLocalParaRemoto(insertsLocalParaRemoto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista cobranças em banco local para insert em remoto");
                throw;
            }

            //Lista entidades em banco local para update em remoto
            try
            {
                var criteria = _local.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync.LastSyncTime));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    if (e == null) continue;
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorIdRemoto<Cobranca>(e.Txid);
                    if (entRemoto == null) throw new Exception("Cobrança não encontrada no banco remoto. Impossível atualizar.");
                    entRemoto.Copiar(e);

                    Calendario calendarioRemoto = await ListarPorUuidRemoto<Calendario>(e.Calendario.Uuid);
                    Valor valorRemoto = await ListarPorUuidRemoto<Valor>(e.Valor.Uuid);
                    Loc locRemoto = await ListarPorUuidRemoto<Loc>(e.Loc.Uuid);
                    QRCode qrcodeRemoto = null;
                    if (e.QrCode != null)
                        qrcodeRemoto = await ListarPorUuidRemoto<QRCode>(e.QrCode.Uuid);

                    if (calendarioRemoto == null)
                        throw new Exception("Calendario não pode ser nulo");
                    if (valorRemoto == null)
                        throw new Exception("Valor não pode ser nulo");
                    if (locRemoto == null)
                        throw new Exception("Loc não pode ser nulo");

                    entRemoto.Calendario = calendarioRemoto;
                    entRemoto.Valor = valorRemoto;
                    entRemoto.Loc = locRemoto;
                    entRemoto.QrCode = qrcodeRemoto;

                    //Coloca na coleção que será salva
                    updatesLocalParaRemoto.Add(entRemoto);
                }

                await UpdateLocalParaRemoto(updatesLocalParaRemoto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                Log.EscreveLogSync(ex, "Lista cobranças em banco local para update em remoto");
                throw;
            }

            await SaveLastSyncTime(lastSync, inicioSync);
        }

        private async Task<IList<Pix>> ListarPixPorCobranca(ISession session, Cobranca cob)
        {
            var criteria = session.CreateCriteria<Pix>();
            criteria.Add(Restrictions.Eq("Cobranca", cob));
            return await criteria.ListAsync<Pix>();
        }
    }
}
