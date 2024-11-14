// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;

namespace Remotion.Logging
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  public static class LogExtensions
  {
    public static T LogAndReturnValue<T> (this T value, ILog log, LogLevel logLevel, Func<T, string> messageCreator)
    {
      throw new NotImplementedException();
    }

    public static IEnumerable<T> LogAndReturnItems<T> (
        this IEnumerable<T> sequence,
        ILog log,
        LogLevel logLevel,
        Func<int, string> iterationCompletedMessageCreator)
    {
      throw new NotImplementedException();
    }


    private static IEnumerable<T> LogAndReturnWithIteration<T> (
        IEnumerable<T> sequence,
        ILog log,
        LogLevel logLevel,
        Func<int, string> iterationCompletedMessageCreator)
    {
      throw new NotImplementedException();
    }

    public static void Log (this ILog log, LogLevel logLevel, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Log (this ILog log, LogLevel logLevel, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Log (this ILog log, LogLevel logLevel, object message)
    {
      throw new NotImplementedException();
    }

    public static void LogFormat (this ILog log, LogLevel logLevel, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void LogFormat (this ILog log, LogLevel logLevel, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void LogFormat (this ILog log, LogLevel logLevel, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void Debug (this ILog log, int eventID, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Debug (this ILog log, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Debug (this ILog log, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Debug (this ILog log, object message)
    {
      throw new NotImplementedException();
    }

    public static void DebugFormat (this ILog log, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void DebugFormat (this ILog log, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void DebugFormat (this ILog log, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void DebugFormat (this ILog log, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void Info (this ILog log, int eventID, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Info (this ILog log, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Info (this ILog log, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Info (this ILog log, object message)
    {
      throw new NotImplementedException();
    }

    public static void InfoFormat (this ILog log, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void InfoFormat (this ILog log, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void InfoFormat (this ILog log, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void InfoFormat (this ILog log, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void Warn (this ILog log, int eventID, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Warn (this ILog log, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Warn (this ILog log, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Warn (this ILog log, object message)
    {
      throw new NotImplementedException();
    }

    public static void WarnFormat (this ILog log, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void WarnFormat (this ILog log, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void WarnFormat (this ILog log, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void WarnFormat (this ILog log, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void Error (this ILog log, int eventID, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Error (this ILog log, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Error (this ILog log, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Error (this ILog log, object message)
    {
      throw new NotImplementedException();
    }

    public static void ErrorFormat (this ILog log, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void ErrorFormat (this ILog log, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void ErrorFormat (this ILog log, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void ErrorFormat (this ILog log, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void Fatal (this ILog log, int eventID, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Fatal (this ILog log, int eventID, object message)
    {
      throw new NotImplementedException();
    }

    public static void Fatal (this ILog log, object message, Exception exceptionObject)
    {
      throw new NotImplementedException();
    }

    public static void Fatal (this ILog log, object message)
    {
      throw new NotImplementedException();
    }

    public static void FatalFormat (this ILog log, int eventID, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void FatalFormat (this ILog log, int eventID, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void FatalFormat (this ILog log, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static void FatalFormat (this ILog log, Exception exceptionObject, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static bool IsDebugEnabled (this ILog log)
    {
      throw new NotImplementedException();
    }

    public static bool IsInfoEnabled (this ILog log)
    {
      throw new NotImplementedException();
    }

    public static bool IsWarnEnabled (this ILog log)
    {
      throw new NotImplementedException();
    }

    public static bool IsErrorEnabled (this ILog log)
    {
      throw new NotImplementedException();
    }

    public static bool IsFatalEnabled (this ILog log)
    {
      throw new NotImplementedException();
    }
  }
}
