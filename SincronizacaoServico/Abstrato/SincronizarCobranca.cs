using NHibernate;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;

namespace SincronizacaoServico.Abstrato
{
    public class SincronizarCobranca : ASincronizar<Cobranca>
    {
        public SincronizarCobranca(ISession local, ISession remoto) : base(local, remoto)
        {
        }

        public override Task<bool> Sincronizar(DateTime lastSync)
        {
            throw new NotImplementedException();
        }
    }
}
