using System;
using SincronizacaoServico.Util;

namespace SincronizacaoServico.Model
{
    public abstract class AModel : ObservableObject
    {
        private Guid _uuid;
        private DateTime? _criadoEm;
        private DateTime? _modificadoEm;
        private DateTime? _deletadoEm;
        private bool _deletado;
        public virtual string Tipo => GetType().Name.ToLower();

        public abstract void InicializaLazy();
        public abstract object GetIdentifier();
        public abstract void SetIdentifierToDefault();
        public abstract void Copiar(object source);

        public virtual bool Deletado
        {
            get => _deletado;
            set
            {
                _deletado = value;
                OnPropertyChanged("Deletado");
            }
        }

        public virtual DateTime? CriadoEm
        {
            get => _criadoEm;
            set
            {
                _criadoEm = value;
                OnPropertyChanged("CriadoEm");
            }
        }
        public virtual DateTime? ModificadoEm
        {
            get => _modificadoEm;
            set
            {
                _modificadoEm = value;
                OnPropertyChanged("ModificadoEm");
            }
        }
        public virtual DateTime? DeletadoEm
        {
            get => _deletadoEm;
            set
            {
                _deletadoEm = value;
                OnPropertyChanged("DeletadoEm");
            }
        }

        public virtual Guid Uuid
        {
            get
            {
                return _uuid;
            }

            set
            {
                _uuid = value;
                OnPropertyChanged("Uuid");
            }
        }
    }
}
