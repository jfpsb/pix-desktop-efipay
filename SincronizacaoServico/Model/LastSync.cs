namespace SincronizacaoServico.Model
{
    public class LastSync
    {
        private int _id;
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
    }
}
