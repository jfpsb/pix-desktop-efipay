namespace SincronizacaoServico.Util
{
    public class Log
    {
        public static readonly string LogLocal = Path.Combine("Logs", "BancoLocalLog.txt");
        public static readonly string LogBackup = Path.Combine("Logs", "BancoBackupLog.txt");
        public static readonly string LogGn = Path.Combine("Logs", "GnLog.txt");
        public static readonly string LogCredenciais = Path.Combine("Logs", "CredenciaisLog.txt");
        public static readonly string LogComunicacaoPorRede = Path.Combine("Logs", "CredenciaisLog.txt");
        public static readonly string LogExceptionGenerica = Path.Combine("Logs", "ExceptionGenerica.txt");
        public static void EscreveLogBancoLocal(Exception ex, string operacao)
        {
            string msg = $"Operação: {operacao.ToUpper()};\nData/Hora: {DateTime.Now};\nMensagem: \n{ex.Message}";
            if (ex.InnerException != null)
            {
                msg += $"\nInnerException: {ex.InnerException.Message}";
            }
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogLocal, msg);
        }

        public static void EscreveLogBancoBackup(Exception ex, string operacao)
        {
            string msg = $"Operação: {operacao.ToUpper()};\nData/Hora: {DateTime.Now};\nMensagem: \n{ex.Message}";
            if (ex.InnerException != null)
            {
                msg += $"\nInnerException: {ex.InnerException.Message}";
            }
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogBackup, msg);
        }

        public static void EscreveExceptionGenerica(Exception ex)
        {
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
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogCredenciais, msg);
        }

        public static void EscreveLogComunicacao(Exception ex)
        {
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogComunicacaoPorRede, msg);
        }
    }
}
