using EHS.Utilities;
using System.Diagnostics;

namespace EHS
{
    public class ErrorHandling
    {
        public static void HandleException(Exception ex)
        {
            // LOG IN WINDOWS EVENT LOG
            using (EventLog eventLog = new EventLog("Application"))
            {
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(0);
                string fileName = frame.GetFileName();
                string methodName = frame.GetMethod().Name;
                int line = frame.GetFileLineNumber();

                eventLog.Source = "Application";
                eventLog.WriteEntry(
                    "ERROR: Environmental Health & Safety (EH&S)" + Environment.NewLine +
                    "CLASS: " + fileName + Environment.NewLine +
                    "METHOD: " + methodName + Environment.NewLine +
                    "LINE NUMBER: " + line.ToString() + Environment.NewLine +
                    "MESSAGE: " + ex.Message,
                    EventLogEntryType.Error);
            }

            // SEND TEAMS ERROR MESSAGE
            Initialization.TeamsErrorProvider.SendMessage(
            "ERROR: Environmental Health & Safety (EH&S) </br>" +
            "MESSAGE: " + ex.Message + "</br> " +
            "INNER EXCEPTION: " + ex.InnerException?.ToString());


            // SEND EMAIL ERROR MESSAGE
            Initialization.EmailProviderSmtp.SendMessage(
                "ERROR: Environmental Health & Safety (EH&S)",
                "ERROR: Environmental Health & Safety (EH&S) </br>" +
                    "MESSAGE: " + ex.Message + "</br> " +
                    "INNER EXCEPTION: " + ex.InnerException?.ToString() + "</br>",
                Initialization.EmailError,
                null,
                null,
                "High");
            //"",
            //null);
        }
    }
}
