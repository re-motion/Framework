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
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using log4net;
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
    [StructLayout (LayoutKind.Sequential)]
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

    private static readonly ILog s_log = LogManager.GetLogger (typeof (ProcessUtils));

    /// <summary>
    /// Retrieves information about the specified process. See https://msdn.microsoft.com/en-us/library/windows/desktop/ms684280.aspx .
    /// </summary>
    /// <returns>The function returns an NTSTATUS success or error code.</returns>
    [DllImport ("ntdll.dll")]
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
      ArgumentUtility.CheckNotNull ("target", target);

      // Query the process information
      var info = new ParentInfo();
      int status;

      try
      {
        int returnLength;
        status = NtQueryInformationProcess (target.Handle, 0, ref info, Marshal.SizeOf (info), out returnLength);
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
    public static void GracefulProcessShutdown ([NotNull] Process process, int timeout)
    {
      ArgumentUtility.CheckNotNull ("process", process);

      if (timeout < 0)
        throw new ArgumentOutOfRangeException ("timeout", "Timeout can not be smaller that zero.");

      if (process.HasExited)
        return;

      // Try to gracefully close the process
      if (process.CloseMainWindow())
      {
        if (process.WaitForExit (timeout))
          return;
      }

      // Force closing the process
      process.Kill();

      if (process.WaitForExit (timeout))
        return;

      throw new InvalidOperationException (string.Format ("Process '{0}' (id: '{1}') did not exit in the expected amount of time.", process.ProcessName, process.Id));
    }

    /// <summary>
    /// Kills all processes with the given <paramref name="processName"/>. All exceptions are swallowed => this method is a best-effort approach.
    /// </summary>
    /// <param name="processName">The process name without the file extension.</param>
    public static void KillAllProcessesWithName ([NotNull] string processName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("processName", processName);

      s_log.DebugFormat ("Process killing has been called for '{0}'...", processName);

      foreach (var process in Process.GetProcessesByName (processName))
      {
        try
        {
          s_log.DebugFormat ("Killing process '{0}'...", processName);
          process.Kill();
        }
        catch (Exception ex)
        {
          s_log.Warn (string.Format ("Killing process '{0}' failed.", processName), ex);
          // Ignore exception, process is already closing or we do not have the required privileges anyway.
        }
      }
    }
  }
}