using NHibernate.Event;
using System;
using System.Threading;
using System.Threading.Tasks;
using VMIClientePix.Model;

namespace VMIClientePix.BancoDeDados.ConnectionFactory
{
    public class NHibernatePreEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var entidade = @event.Entity;
            var state = @event.State;
            var model = entidade as AModel;

            //Coloca valores na entidade
            model.CriadoEm = DateTime.Now;
            model.Uuid = Guid.NewGuid();

            //Coloca valores no state
            int IndexCriadoEm = Array.IndexOf(@event.Persister.PropertyNames, "CriadoEm");
            int IndexUuid = Array.IndexOf(@event.Persister.PropertyNames, "Uuid");
            state[IndexCriadoEm] = model.CriadoEm;
            state[IndexUuid] = model.Uuid;

            return false;
        }
        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            var entidade = @event.Entity;
            var state = @event.State;
            var model = entidade as AModel;

            //Coloca valores na entidade
            model.CriadoEm = DateTime.Now;
            model.Uuid = Guid.NewGuid();

            //Coloca valores no state
            int IndexCriadoEm = Array.IndexOf(@event.Persister.PropertyNames, "CriadoEm");
            int IndexUuid = Array.IndexOf(@event.Persister.PropertyNames, "Uuid");
            state[IndexCriadoEm] = model.CriadoEm;
            state[IndexUuid] = model.Uuid;

            return Task.FromResult(false);
        }
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var entidade = @event.Entity;
            var state = @event.State;
            var model = entidade as AModel;

            //Coloca valores na entidade
            var now = DateTime.Now;
            model.ModificadoEm = now;
            if (model.Deletado)
                model.DeletadoEm = now;

            //Coloca valores no state
            int IndexModificadoEm = Array.IndexOf(@event.Persister.PropertyNames, "ModificadoEm");
            int IndexDeletadoEm = Array.IndexOf(@event.Persister.PropertyNames, "DeletadoEm");
            state[IndexModificadoEm] = model.ModificadoEm;
            state[IndexDeletadoEm] = model.DeletadoEm;

            return false;
        }
        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        {
            var entidade = @event.Entity;
            var state = @event.State;
            var model = entidade as AModel;

            //Coloca valores na entidade
            var now = DateTime.Now;
            model.ModificadoEm = now;

            if (model.Deletado)
                model.DeletadoEm = now;

            //Coloca valores no state
            int IndexModificadoEm = Array.IndexOf(@event.Persister.PropertyNames, "ModificadoEm");
            int IndexDeletadoEm = Array.IndexOf(@event.Persister.PropertyNames, "DeletadoEm");
            state[IndexModificadoEm] = model.ModificadoEm;

            if (model.Deletado)
                state[IndexDeletadoEm] = model.DeletadoEm;

            return Task.FromResult(false);
        }
    }
}
