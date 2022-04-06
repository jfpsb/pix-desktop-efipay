using Gerencianet.NETCore.SDK;
using System;
using System.IO;

namespace VMIClientePix.Util
{
    public class Log
    {
        public static readonly string LogLocal = "BancoLocalLog.txt";
        public static readonly string LogBackup = "BancoBackupLog.txt";
        public static readonly string LogGn = "GnLog.txt";
        public static readonly string LogCredenciais = "CredenciaisLog.txt";
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

        public static void EscreveLogGn(GnException ex)
        {
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};\nErrorType: \n{ex.ErrorType}";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogGn, msg);
        }

        public static void EscreveLogCredenciais(Exception ex)
        {
            string msg = $"Data/Hora: {DateTime.Now};\nMensagem: \n{ex.Message};";
            msg += $"\nStackTrace: \n{ex.StackTrace}\n\n";
            File.AppendAllText(LogCredenciais, msg);
        }
    }
}
