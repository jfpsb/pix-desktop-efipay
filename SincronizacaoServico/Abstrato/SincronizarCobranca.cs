using NHibernate;
using NHibernate.Criterion;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarCobranca : ASincronizar<Cobranca>
    {
        public SincronizarCobranca(ISession local, ISession remoto) : base(local, remoto, false)
        {
        }

        public async override Task Sincronizar(DateTime lastSync)
        {
            IList<Cobranca> insertsRemotoParaLocal = new List<Cobranca>();
            IList<Cobranca> updatesRemotoParaLocal = new List<Cobranca>();
            IList<Cobranca> insertsLocalParaRemoto = new List<Cobranca>();
            IList<Cobranca> updatesLocalParaRemoto = new List<Cobranca>();

            //Lista entidades em banco remoto para insert em local
            try
            {
                var criteria = _remoto.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {

                    Cobranca eASalvar = new Cobranca();
                    eASalvar.Copiar(e);

                    Calendario calendarioLocal = await ListarPorUuidLocal<Calendario>(e.Calendario.Uuid);
                    Valor valorLocal = await ListarPorUuidLocal<Valor>(e.Valor.Uuid);
                    Loc locLocal = await ListarPorUuidLocal<Loc>(e.Loc.Uuid);
                    QRCode qrcodeLocal = await ListarPorUuidLocal<QRCode>(e.QrCode.Uuid);

                    eASalvar.Calendario = calendarioLocal;
                    eASalvar.Valor = valorLocal;
                    eASalvar.Loc = locLocal;
                    eASalvar.QrCode = qrcodeLocal;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Cobranca>(e.Uuid);
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
                var criteria = _remoto.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    Cobranca eASalvar = new Cobranca();
                    //Entidade com mesmo UUID no banco local
                    var entLocal = await ListarPorUuidLocal<Cobranca>(e.Uuid);
                    eASalvar.Copiar(entLocal);

                    Calendario calendarioLocal = await ListarPorUuidLocal<Calendario>(e.Calendario.Uuid);
                    Valor valorLocal = await ListarPorUuidLocal<Valor>(e.Valor.Uuid);
                    Loc locLocal = await ListarPorUuidLocal<Loc>(e.Loc.Uuid);
                    QRCode qrcodeLocal = await ListarPorUuidLocal<QRCode>(e.QrCode.Uuid);

                    eASalvar.Calendario = calendarioLocal;
                    eASalvar.Valor = valorLocal;
                    eASalvar.Loc = locLocal;
                    eASalvar.QrCode = qrcodeLocal;

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
                var criteria = _local.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("CriadoEm", lastSync));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    Cobranca eASalvar = new Cobranca();
                    eASalvar.Copiar(e);

                    Calendario calendarioRemoto = await ListarPorUuidRemoto<Calendario>(eASalvar.Calendario.Uuid);
                    Valor valorRemoto = await ListarPorUuidRemoto<Valor>(eASalvar.Valor.Uuid);
                    Loc locRemoto = await ListarPorUuidRemoto<Loc>(eASalvar.Loc.Uuid);
                    QRCode qrcodeRemoto = await ListarPorUuidRemoto<QRCode>(eASalvar.QrCode.Uuid);

                    eASalvar.Calendario = calendarioRemoto;
                    eASalvar.Valor = valorRemoto;
                    eASalvar.Loc = locRemoto;
                    eASalvar.QrCode = qrcodeRemoto;

                    //Entidade com mesmo UUID no banco local
                    var ent = await ListarPorUuidLocal<Cobranca>(e.Uuid);
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
                var criteria = _local.CreateCriteria<Cobranca>();
                criteria.Add(Restrictions.Ge("ModificadoEm", lastSync));
                var lista = await criteria.ListAsync<Cobranca>();

                foreach (Cobranca e in lista)
                {
                    Cobranca eASalvar = new Cobranca();
                    //Entidade com mesmo UUID no banco remoto
                    var entRemoto = await ListarPorUuidRemoto<Cobranca>(e.Uuid);
                    eASalvar.Copiar(entRemoto);

                    Calendario calendarioRemoto = await ListarPorUuidRemoto<Calendario>(e.Calendario.Uuid);
                    Valor valorRemoto = await ListarPorUuidRemoto<Valor>(e.Valor.Uuid);
                    Loc locRemoto = await ListarPorUuidRemoto<Loc>(e.Loc.Uuid);
                    QRCode qrcodeRemoto = await ListarPorUuidRemoto<QRCode>(e.QrCode.Uuid);

                    eASalvar.Calendario = calendarioRemoto;
                    eASalvar.Valor = valorRemoto;
                    eASalvar.Loc = locRemoto;
                    eASalvar.QrCode = qrcodeRemoto;

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
                    foreach (Cobranca insert in insertsRemotoParaLocal)
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
                    foreach (Cobranca update in updatesRemotoParaLocal)
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
                    foreach (Cobranca insert in insertsLocalParaRemoto)
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
                    foreach (Cobranca update in updatesLocalParaRemoto)
                    {
                        await _remoto.UpdateAsync(update);
                    }
                    await tx.CommitAsync();
                }
            }
        }

        private async Task<IList<Pix>> ListarPixPorCobranca(ISession session, Cobranca cob)
        {
            var criteria = session.CreateCriteria<Pix>();
            criteria.Add(Restrictions.Eq("Cobranca", cob));
            return await criteria.ListAsync<Pix>();
        }
    }
}
