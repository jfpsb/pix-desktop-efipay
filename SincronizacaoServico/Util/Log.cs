namespace SincronizacaoServico.Util
{
    public class Log
    {
        public static readonly string LogSync = Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao", "SyncLog.txt");
        public static readonly string LogCredenciais = Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao", "CredenciaisLog.txt");
        public static readonly string LogExceptionGenerica = Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao", "ExceptionGenerica.txt");
        public static void EscreveLogSync(Exception ex, string operacao)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao"));
            string msg = $"Operação: {operacao.ToUpper()};\nData/Hora: {DateTime.Now};\nMensagem: \n{ex.Message}";
            if (ex.InnerException != null)
            {
                msg += $"\nInnerException: {ex.InnerException.Message}";
            }
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogSync, msg);
        }

        public static void EscreveExceptionGenerica(Exception ex)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao"));
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem:\n{ex.Message}\n";

            if (ex.InnerException != null)
            {
                msg += $"InnerException:\n{ex.InnerException.Message}\n";
            }

            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogExceptionGenerica, msg);
        }

        public static void EscreveLogCredenciais(Exception ex)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "Sincronizacao"));
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogCredenciais, msg);
        }
    }
}
