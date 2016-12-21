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
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Wraps an IIS Express instance hosting the web application under test.
  /// </summary>
  public class IisExpressProcessWrapper : IDisposable
  {
    private readonly Process _iisProcess;

    /// <summary>
    /// Initializes the wrapper, does not yet run the IIS Express process.
    /// </summary>
    /// <param name="webApplicationPath">Absolute file path to the web application under test.</param>
    /// <param name="webApplicationPort">Port to be used when hosting the web application.</param>
    public IisExpressProcessWrapper ([NotNull] string webApplicationPath, int webApplicationPort)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);

      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Minimized,
                          ErrorDialog = true,
                          LoadUserProfile = true,
                          CreateNoWindow = false,
                          UseShellExecute = false
                      };

      var programFilesPath = GetProgramFilesPath (startInfo);
      var iisExpressExecutablePath = Path.Combine (programFilesPath, "IIS Express", "iisexpress.exe");

      startInfo.FileName = iisExpressExecutablePath;
      startInfo.Arguments = string.Format ("/path:\"{0}\" /port:\"{1}\"", webApplicationPath, webApplicationPort);

      _iisProcess = new Process { StartInfo = startInfo };
    }

    /// <summary>
    /// Starts the IIS Express process and thereby hosts the web application.
    /// </summary>
    public void Run ()
    {
      _iisProcess.Start();
      _iisProcess.WaitForExit();
    }

    /// <summary>
    /// Stops the IIS Express process and thereby "unhosts" the web application.
    /// </summary>
    public void Dispose ()
    {
      if (_iisProcess != null && !_iisProcess.HasExited)
      {
        _iisProcess.CloseMainWindow();
        _iisProcess.Dispose();
      }
    }

    private static string GetProgramFilesPath (ProcessStartInfo startInfo)
    {
      return string.IsNullOrEmpty (startInfo.EnvironmentVariables["ProgramFiles"])
          ? startInfo.EnvironmentVariables["ProgramFiles(x86)"]
          : startInfo.EnvironmentVariables["ProgramFiles"];
    }
  }
}