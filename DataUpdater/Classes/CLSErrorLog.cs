using System;
using System.Diagnostics;

namespace DataUpdater.Classes
{
    class CLSErrorLog
    {public static void WriteLog(Exception ex)
        {
            var appName = "FingerPrintSoft";
            if (!EventLog.Exists(appName)) 
                EventLog.CreateEventSource(appName,appName);

            EventLog.WriteEntry(appName,ex.Message,EventLogEntryType.Error);
        }
    }
}
