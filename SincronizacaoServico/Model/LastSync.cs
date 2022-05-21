namespace SincronizacaoServico.Model
{
    public class LastSync
    {
        private int _id;
        private string _tabela;
        private DateTime _lastSyncTime;

        public virtual int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public virtual DateTime LastSyncTime
        {
            get
            {
                return _lastSyncTime;
            }

            set
            {
                _lastSyncTime = value;
            }
        }

        public virtual string Tabela
        {
            get
            {
                return _tabela;
            }

            set
            {
                _tabela = value;
            }
        }
    }
}
