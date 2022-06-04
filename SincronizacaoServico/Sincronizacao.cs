using MySql.Data.MySqlClient;
using NHibernate;
using SincronizacaoServico.Banco;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using System.Timers;

namespace SincronizacaoServico
{
    public class Sincronizacao
    {
        private System.Timers.Timer timerConexao;

        private Thread threadCalendario;
        private Thread threadCobranca;
        private Thread threadDevolucao;
        private Thread threadHorario;
        private Thread threadLoc;
        private Thread threadPagador;
        private Thread threadPix;
        private Thread threadQRCode;
        private Thread threadValor;

        private IList<Thread> _threads;
        private SemaphoreSlim _semaphoreSlim;

        public Sincronizacao()
        {
            _threads = new List<Thread>();
            _semaphoreSlim = new SemaphoreSlim(1);
            timerConexao = new System.Timers.Timer(3000) { AutoReset = false };
            timerConexao.Elapsed += CriarSessionFactory;
        }

        public void Start()
        {
            timerConexao.Start();
        }

        public void Stop()
        {
            timerConexao.Stop();
            timerConexao.Dispose();
            SessionProvider.FechaSessionFactory();
            SessionProviderBackup.FechaSessionFactory();
        }

        private void CriarSessionFactory(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerConexao.Stop();

                if (SessionProvider.SessionFactory == null)
                    SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
                if (SessionProviderBackup.BackupSessionFactory == null)
                    SessionProviderBackup.BackupSessionFactory = SessionProviderBackup.BuildSessionFactory();

                threadCalendario = new Thread(() => { ElapsedGenerico<Calendario>(); });
                threadCobranca = new Thread(() => { ElapsedGenerico<Cobranca>(); });
                threadDevolucao = new Thread(() => { ElapsedGenerico<Devolucao>(); });
                threadHorario = new Thread(() => ElapsedGenerico<Horario>());
                threadLoc = new Thread(() => { ElapsedGenerico<Loc>(); });
                threadPagador = new Thread(() => { ElapsedGenerico<Pagador>(); });
                threadPix = new Thread(() => { ElapsedGenerico<Pix>(); });
                threadQRCode = new Thread(() => { ElapsedGenerico<QRCode>(); });
                threadValor = new Thread(() => { ElapsedGenerico<Valor>(); });

                _threads.Clear();
                _threads.Add(threadCalendario);
                _threads.Add(threadCobranca);
                _threads.Add(threadDevolucao);
                _threads.Add(threadHorario);
                _threads.Add(threadLoc);
                _threads.Add(threadPagador);
                _threads.Add(threadPix);
                _threads.Add(threadQRCode);
                _threads.Add(threadValor);

                foreach (Thread t in _threads)
                {
                    t.Start();
                }
            }
            catch (MySqlException mex)
            {
                Console.WriteLine("Erro ao abrir conexão com banco de dados local ou remoto." +
                    "\nSem esta conexão a sincronização não é possível.\nNova tentativa em 3 segundos." + mex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Nova tentativa de criar SessionFactory em 3 segundos.");
            }
            finally
            {
                timerConexao.Start();
            }
        }

        private async void ElapsedGenerico<E>() where E : AModel, new()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                Console.WriteLine($"Iniciando sincronização de {typeof(E).Name} em {DateTime.Now}");
                ISession local = SessionProvider.GetSession();
                ISession remote = SessionProviderBackup.GetSession();
                ASincronizar<E> aSync = new SincronizarGenerico<E>(local, remote);
                await aSync.Sincronizar();
                SessionProvider.FechaSession(local);
                SessionProviderBackup.FechaSession(remote);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
