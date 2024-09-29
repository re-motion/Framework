// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// <see cref="Process"/> utility methods.
  /// </summary>
  public static class ProcessUtils
  {
    /// <summary>
    /// Struct for the Interopt call to <see cref="NtQueryInformationProcess"/> in <see cref="GetParentProcessID"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct ParentInfo
    {
      // ReSharper disable FieldCanBeMadeReadOnly.Local
      private IntPtr Unused1;

      private IntPtr Unused2;
      private IntPtr Unused3;
      private IntPtr Unused4;

      private IntPtr Unused5;
      // ReSharper restore FieldCanBeMadeReadOnly.Local

      internal IntPtr InheritedFromUniqueProcessID;
    }

    /// <summary>
    /// Retrieves information about the specified process. See https://msdn.microsoft.com/en-us/library/windows/desktop/ms684280.aspx .
    /// </summary>
    /// <returns>The function returns an NTSTATUS success or error code.</returns>
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess (
        IntPtr processHandle,
        int processInformationClass,
        ref ParentInfo processInformation,
        int processInformationLength,
        out int returnLength);

    /// <summary>
    /// Returns the process id of the parent process if available, or <c>-1</c> if no process could be found.
    /// </summary>
    /// <param name="target">The process whose parent should be searched.</param>
    /// <returns>The process id of the parent process, or <c>-1</c> if no parent process could be found.</returns>
    public static int GetParentProcessID ([NotNull] Process target)
    {
      ArgumentUtility.CheckNotNull("target", target);

      // Query the process information
      var info = new ParentInfo();
      int status;

      try
      {
        int returnLength;
        status = NtQueryInformationProcess(target.Handle, 0, ref info, Marshal.SizeOf(info), out returnLength);
      }
      catch (InvalidOperationException)
      {
        return -1;
      }

      // Check if the query was successful
      if (status != 0)
        return -1;

      // Return the process id from the query result.
      return info.InheritedFromUniqueProcessID.ToInt32();
    }

    /// <summary>
    /// Tries to shutdown a process gracefully and if it is not closed after <paramref name="timeout"/> it ensures that it is closed.
    /// </summary>
    /// <param name="process">The process that should be shut down.</param>
    /// <param name="timeout">The wait timeout after a shutdown has been requested.</param>
    /// <remarks>
    /// <p>
    /// This method will block until the process has exited. Shutdown procedure is as follows:
    /// </p>
    /// <list type="number">
    /// <item>
    /// <description>Check if the process has already exited.</description>
    /// </item>
    /// <item>
    /// <description>Close the main window of the process and wait <paramref name="timeout"/> ms for the process to exit.</description>
    /// </item>
    /// <item>
    /// <description>If the process is still not closed, kill it and wait for the exit.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static void GracefulProcessShutdown ([NotNull] Process process, TimeSpan timeout)
    {
      ArgumentUtility.CheckNotNull("process", process);

      GracefulProcessShutdown(new[] { process }, timeout);
    }

    /// <summary>
    /// Tries to shutdown a list of processes gracefully and if they are not closed after <paramref name="timeout"/> it ensures that they are closed.
    /// </summary>
    /// <param name="processes">The processes that should be shut down.</param>
    /// <param name="timeout">The wait timeout after a shutdown has been requested.</param>
    /// <remarks>
    /// <p>
    /// This method will block until the processes have exited. Shutdown procedure is as follows:
    /// </p>
    /// <list type="number">
    /// <item>
    /// <description>Check if the processes have already exited.</description>
    /// </item>
    /// <item>
    /// <description>Close the main windows of the processes and wait for the processes to exit or the <paramref name="timeout"/> to run out.</description>
    /// </item>
    /// <item>
    /// <description>If the processes are still not closed, kill them and wait for the processes to exit or the <paramref name="timeout"/> is reached.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static void GracefulProcessShutdown (IReadOnlyList<Process> processes, TimeSpan timeout)
    {
      ArgumentUtility.CheckNotNullOrItemsNull(nameof(processes), processes);

      var timeoutInMilliseconds = (int)timeout.TotalMilliseconds;
      if (timeoutInMilliseconds < 0)
        throw new ArgumentOutOfRangeException("timeout", "Timeout can not be smaller that zero.");

      IReadOnlyList<Process> remainingProcesses = processes.ToList();

      // Clear out any processes that already exited
      remainingProcesses = WaitForProcessesExits(remainingProcesses, TimeSpan.Zero);

      // Try to gracefully close the process
      var anyMainWindowClosed = remainingProcesses.Any(CloseMainWindow);
      if (anyMainWindowClosed)
        remainingProcesses = WaitForProcessesExits(remainingProcesses, timeout);

      // Force closing the process and wait for process exits
      foreach (var remainingProcess in remainingProcesses)
        KillProcess(remainingProcess);

      remainingProcesses = WaitForProcessesExits(remainingProcesses, timeout);
      if (remainingProcesses.Any())
        throw CreateProcessesDidNotExitInTimeException(remainingProcesses, timeout);
    }

    private static bool CloseMainWindow (Process process)
    {
      try
      {
        return process.CloseMainWindow();
      }
      catch (InvalidOperationException)
      {
        // Thrown if the process has already exited, or no process is associated with the Process object.
        // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.closemainwindow#remarks
        // This exception can be swallowed safely, as we are sure that a process is associated and the process has
        // already exited, therefore the method can return true at this point.
        return true;
      }
    }

    private static void KillProcess (Process process)
    {
      try
      {
        process.Kill();
      }
      catch (Win32Exception)
      {
        // Thrown if the .Kill() method is called while the process is currently terminating.
        // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.kill#remarks
        // This exception can be swallowed safely, as the process that should be killed is already terminating.
      }
      catch (InvalidOperationException)
      {
        // Thrown if the process has already exited, or no process is associated with the Process object.
        // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.kill#System_Diagnostics_Process_Kill
        // This exception can be swallowed safely, as we are sure that a process is associated and the process has
        // already exited, therefore the method can return at this point.
      }
    }

    /// <summary>
    /// Waits for the specified <paramref name="processes"/> to exit or until the <paramref name="sharedTimeout"/> is reached.
    /// </summary>
    /// <param name="processes">The processes that should be waited on to exit.</param>
    /// <param name="sharedTimeout">The wait timeout after which waiting will be canceled.</param>
    /// <returns>A subset of the specified <paramref name="processes"/> that did not exit withing the <paramref name="sharedTimeout"/>.</returns>
    [MustUseReturnValue]
    private static IReadOnlyList<Process> WaitForProcessesExits (IEnumerable<Process> processes, TimeSpan sharedTimeout)
    {
      var remainingProcesses = new List<Process>();

      // Go through all processes and call .WaitForExit on each using a shared timeout
      // If the timeout runs out still go through all remaining process to make sure to clear any exited processes
      var stopwatch = Stopwatch.StartNew();
      foreach (var process in processes)
      {
        var timeToTimeoutEnd = sharedTimeout - stopwatch.Elapsed;

        // Use 0 (process.WaitForExit returns instantly) if we exceeded our timeout, otherwise use the remaining timeout
        var remainingTimeoutInMilliseconds = Math.Max((int)timeToTimeoutEnd.TotalMilliseconds, 0);
        if (!process.WaitForExit(remainingTimeoutInMilliseconds))
          remainingProcesses.Add(process);
      }

      return remainingProcesses;
    }

    private static Exception CreateProcessesDidNotExitInTimeException (IEnumerable<Process> processes, TimeSpan timeout)
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("The following processes did not exit within the timeout of {0:0.##} seconds:", timeout.TotalSeconds).AppendLine();
      foreach (var process in processes)
        stringBuilder.AppendFormat(" - Process '{0}' (id: {1})", process.ProcessName, process.Id).AppendLine();

      return new InvalidOperationException(stringBuilder.ToString());
    }

    /// <summary>
    /// Kills all processes with the given <paramref name="processName"/>. All exceptions are swallowed => this method is a best-effort approach.
    /// </summary>
    /// <param name="processName">The process name without the file extension.</param>
    /// <param name="logger">The <see cref="ILogger"/> used when generating diagnostic outout. Use <see cref="NullLogger"/> if no logs are required.</param>
    public static void KillAllProcessesWithName ([NotNull] string processName, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNullOrEmpty("processName", processName);
      ArgumentUtility.CheckNotNull("logger", logger);

      logger.LogDebug("Process killing has been called for '{0}'...", processName);

      foreach (var process in Process.GetProcessesByName(processName))
      {
        try
        {
          logger.LogDebug("Killing process '{0}'...", processName);
          process.Kill();
        }
        catch (Exception ex)
        {
          logger.LogWarning(string.Format("Killing process '{0}' failed.", processName), ex);
          // Ignore exception, process is already closing or we do not have the required privileges anyway.
        }
      }
    }
  }
}
