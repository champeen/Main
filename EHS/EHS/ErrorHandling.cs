using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EHS
{
    public static class ErrorHandling
    {
        public static void HandleException(Exception ex)
        {
            try
            {
                // LOG IN WINDOWS EVENT LOG
                try
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        StackTrace st = new StackTrace(ex, true);
                        StackFrame frame = st.GetFrame(0);
                        string fileName = frame?.GetFileName();
                        string methodName = frame?.GetMethod()?.Name;
                        int line = frame?.GetFileLineNumber() ?? 0;

                        eventLog.Source = "Application";
                        eventLog.WriteEntry(
                            "ERROR: Environmental Health & Safety (EH&S)" + Environment.NewLine +
                            "CLASS: " + fileName + Environment.NewLine +
                            "METHOD: " + methodName + Environment.NewLine +
                            "LINE NUMBER: " + line + Environment.NewLine +
                            "MESSAGE: " + ex.Message,
                            EventLogEntryType.Error);
                    }
                }
                catch (Exception logEx)
                {
                    File.AppendAllText("C:\\Temp\\ehs_log_fallback.txt", $"[EventLog Fail] {logEx.Message}\n");
                }

                // TEAMS LOG
                try
                {
                    Initialization.TeamsErrorProvider.SendMessage(
                        "ERROR: Environmental Health & Safety (EH&S)<br>" +
                        "MESSAGE: " + ex.Message + "<br>" +
                        "INNER EXCEPTION: " + ex.InnerException?.ToString());
                }
                catch (Exception teamsEx)
                {
                    File.AppendAllText("C:\\Temp\\ehs_log_fallback.txt", $"[Teams Fail] {teamsEx.Message}\n");
                }

                // EMAIL LOG
                try
                {
                    Initialization.EmailProviderSmtp.SendMessage(
                        "ERROR: Environmental Health & Safety (EH&S)",
                        "ERROR: Environmental Health & Safety (EH&S)<br>" +
                        "MESSAGE: " + ex.Message + "<br>" +
                        "INNER EXCEPTION: " + ex.InnerException?.ToString() + "<br>",
                        Initialization.EmailError,
                        null,
                        null,
                        "High");
                }
                catch (Exception emailEx)
                {
                    File.AppendAllText("C:\\Temp\\ehs_log_fallback.txt", $"[Email Fail] {emailEx.Message}\n");
                }
            }
            catch (Exception failEx)
            {
                // Final fail-safe
                File.AppendAllText("C:\\Temp\\ehs_log_fallback.txt", $"[Global Fail] {failEx.Message}\n");
            }
        }
    }
}
