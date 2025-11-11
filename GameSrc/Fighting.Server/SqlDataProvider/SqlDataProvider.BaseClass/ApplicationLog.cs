// Decompiled with JetBrains decompiler
// Type: SqlDataProvider.BaseClass.ApplicationLog
// Assembly: SqlDataProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F6DE9576-B5AF-4392-BBCE-95C72793F7EA
// Assembly location: D:\2020-Fixed\Files\SqlDataProvider.dll

using System.Diagnostics;

namespace SqlDataProvider.BaseClass
{
  public static class ApplicationLog
  {
    public static void WriteError(string message) => ApplicationLog.WriteLog(TraceLevel.Error, message);

    private static void WriteLog(TraceLevel level, string messageText)
    {
      try
      {
        EventLogEntryType type = level != TraceLevel.Error ? EventLogEntryType.Error : EventLogEntryType.Error;
        string str = "Application";
        if (!EventLog.SourceExists(str))
          EventLog.CreateEventSource(str, "BIZ");
        new EventLog(str, ".", str).WriteEntry(messageText, type);
      }
      catch
      {
      }
    }
  }
}
