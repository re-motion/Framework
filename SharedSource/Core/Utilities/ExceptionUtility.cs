// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Reflection;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  static partial class ExceptionUtility
  {
    private static readonly Lazy<Action<Exception>> s_internalPreserveStackTrace = new Lazy<Action<Exception>>(GetInternalPreserveStackTrace);

    public static Exception PreserveStackTrace (this Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException("exception");

      // http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx

      s_internalPreserveStackTrace.Value(exception);
      return exception;
    }

    private static Action<Exception> GetInternalPreserveStackTrace ()
    {
      var methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
      if (methodInfo == null)
        throw new InvalidOperationException("Type 'System.Exception' does not contain method InternalPreserveStackTrace().");

      return (Action<Exception>)methodInfo.CreateDelegate(typeof(Action<Exception>));
    }
  }
}
