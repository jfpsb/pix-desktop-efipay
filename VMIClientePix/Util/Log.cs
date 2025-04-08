using Efipay;
using System;
using System.IO;

namespace VMIClientePix.Util
{
    public class Log
    {
        public static readonly string LogLocal = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "BancoLocalLog.txt");
        public static readonly string LogBackup = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "BancoBackupLog.txt");
        public static readonly string LogGn = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "GnLog.txt");
        public static readonly string LogCredenciais = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "CredenciaisLog.txt");
        public static readonly string LogComunicacaoPorRede = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "CredenciaisLog.txt");
        public static readonly string LogExceptionGenerica = Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix", "ExceptionGenerica.txt");
        public static void EscreveLogBancoLocal(Exception ex, string operacao)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
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
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
            string msg = $"Operação: {operacao.ToUpper()};\nData/Hora: {DateTime.Now};\nMensagem: \n{ex.Message}";
            if (ex.InnerException != null)
            {
                msg += $"\nInnerException: {ex.InnerException.Message}";
            }
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogBackup, msg);
        }

        public static void EscreveLogEfi(EfiException ex)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};\nErrorType: \n{ex.ErrorType}";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogGn, msg);
        }

        public static void EscreveExceptionGenerica(Exception ex)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
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
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogCredenciais, msg);
        }

        public static void EscreveLogComunicacao(Exception ex)
        {
            Directory.CreateDirectory(Path.Combine(Global.AppDocumentsFolder, "Logs", "ClientePix"));
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogComunicacaoPorRede, msg);
        }
    }
}
