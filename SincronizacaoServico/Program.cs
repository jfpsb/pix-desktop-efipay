using Topshelf;

namespace SincronizacaoServico
{
    public class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(run =>
            {
                run.Service<Sincronizacao>(s =>
                {
                    s.ConstructUsing(sync => new Sincronizacao());
                    s.WhenStarted(sync => sync.Start());
                    s.WhenStopped(sync => sync.Stop());
                });

                run.RunAsLocalSystem();
                run.SetServiceName("SyncVMIPixService");
                run.SetDisplayName("Sincronização De Dados VMIClientePix");
                run.SetDescription("Serviço que realiza a sincronização dos bancos de dados local e remoto da aplicação VMIClientePix");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}