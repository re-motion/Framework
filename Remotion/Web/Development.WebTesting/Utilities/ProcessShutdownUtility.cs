// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities;

internal static class ProcessShutdownUtility
{
  public static Task<int> RegisterProcessShutdownOnCancellationToken (
      Process process,
      CancellationToken cancellationToken,
      int timeoutInMilliseconds)
  {
    ArgumentUtility.CheckNotNull(nameof(process), process);
    ArgumentUtility.CheckNotNull(nameof(cancellationToken), cancellationToken);
    ArgumentUtility.CheckNotNull(nameof(timeoutInMilliseconds), timeoutInMilliseconds);

    var tcs = new TaskCompletionSource<int>();
    cancellationToken.Register(
        () =>
        {
          if (StopProcessAndWaitForExit(process, timeoutInMilliseconds, out var innerException))
          {
            tcs.SetResult(process.ExitCode);
          }
          else
          {
            var exception = new InvalidOperationException(
                $"Could not stop process '{process.ProcessName}' ({process.Id}).",
                innerException);
            tcs.SetException(exception);
          }
        });

    return tcs.Task;
  }

  private static bool StopProcessAndWaitForExit (
      Process process,
      int timeoutInMilliseconds,
      out Exception? innerException)
  {
    innerException = null;
    if (process.HasExited)
      return true;

    try
    {
      process.CloseMainWindow();
      if (process.WaitForExit(timeoutInMilliseconds))
        return true;

      process.Kill();
      if (process.WaitForExit(timeoutInMilliseconds))
        return true;
    }
    catch (Exception ex)
    {
      // If the process has existed ignore any of the errors
      if (process.HasExited)
        return true;

      innerException = ex;
    }

    return false;
  }
}
