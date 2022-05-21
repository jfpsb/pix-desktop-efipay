using MySql.Data.MySqlClient;
using NHibernate;
using SincronizacaoServico.Abstrato;
using SincronizacaoServico.Banco;
using SincronizacaoServico.Interfaces;
using SincronizacaoServico.Model;
using System.Timers;

namespace SincronizacaoServico
{
    public class Sincronizacao
    {
        private System.Timers.Timer timer;

        public Sincronizacao()
        {
            timer = new System.Timers.Timer(2000) { AutoReset = false };
            timer.Elapsed += ExecutaSync;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            SessionProvider.FechaSessionFactory();
            SessionProviderBackup.FechaSessionFactory();
        }

        private async void ExecutaSync(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();
                if (SessionProvider.SessionFactory == null)
                    SessionProvider.SessionFactory = SessionProvider.BuildSessionFactory();
                if (SessionProviderBackup.BackupSessionFactory == null)
                    SessionProviderBackup.BackupSessionFactory = SessionProviderBackup.BuildSessionFactory();
                Console.WriteLine($"Iniciando sincronização em {e.SignalTime}");
                await StartSync();
                Console.WriteLine($"Fim de sincronização em {DateTime.Now}");
            }
            catch (MySqlException mex)
            {
                Console.WriteLine("Erro ao abrir conexão com banco de dados local ou remoto." +
                    "\nSem esta conexão a sincronização não é possível.\n" + mex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                timer.Start();
            }
        }

        private async Task StartSync()
        {
            ISession local = SessionProvider.GetSession();
            ISession remote = SessionProviderBackup.GetSession();

            try
            {
                ASincronizar<Calendario> aCalendario = new SincronizarGenerico<Calendario>(local, remote);
                ASincronizar<Valor> aValor = new SincronizarGenerico<Valor>(local, remote);
                ASincronizar<Loc> aLoc = new SincronizarGenerico<Loc>(local, remote, false);
                ASincronizar<QRCode> aQRCode = new SincronizarGenerico<QRCode>(local, remote);
                SincronizarCobranca aCobranca = new SincronizarCobranca(local, remote);
                ASincronizar<Pagador> aPagador = new SincronizarGenerico<Pagador>(local, remote);
                SincronizarPix aPix = new SincronizarPix(local, remote);
                ASincronizar<Horario> aHorario = new SincronizarGenerico<Horario>(local, remote);
                SincronizarDevolucao aDevolucao = new SincronizarDevolucao(local, remote);

                await aCalendario.Sincronizar();
                await aValor.Sincronizar();
                await aLoc.Sincronizar();
                await aQRCode.Sincronizar();
                await aCobranca.Sincronizar();
                await aPagador.Sincronizar();
                await aPix.Sincronizar();
                await aHorario.Sincronizar();
                await aDevolucao.Sincronizar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                SessionProvider.FechaSession(local);
                SessionProviderBackup.FechaSession(remote);
            }
        }
    }
}
