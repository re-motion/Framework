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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.Win32WindowSupport;
using Remotion.WindowFinder;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer
{
  /// <summary>
  /// Handles the native windows (like the download information bar or the download manager) for the Internet Explorer download.
  /// </summary>
  public sealed class InternetExplorerNativeWindowHandler
  {
    private const string c_classNameOfDownloadInformationBar = "Frame Notification Bar";
    private const string c_classNameOfDownloadWindow = "#32770";
    private const string c_productNameContainedInDownloadManagerTitle = "Internet Explorer";

    private static readonly TimeSpan s_waitForDownloadInformationBarRetryInterval = TimeSpan.FromMilliseconds (250);

    private readonly IWindowFinder _nativeWindowFinder;
    private readonly IWin32WindowsNativeMethodsExtended _nativeWindowsMethods;

    public InternetExplorerNativeWindowHandler ([NotNull] IWindowFinder nativeWindowFinder, [NotNull] IWin32WindowsNativeMethodsExtended nativeWindowsMethods)
    {
      ArgumentUtility.CheckNotNull ("nativeWindowFinder", nativeWindowFinder);
      ArgumentUtility.CheckNotNull ("nativeWindowsMethods", nativeWindowsMethods);

      _nativeWindowFinder = nativeWindowFinder;
      _nativeWindowsMethods = nativeWindowsMethods;
    }

    /// <summary>
    /// Waits for the download information bar to appear.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <para>Thrown if either no download information bar appeared until the <paramref name="downloadTimeout"/> </para>
    /// <para>- or -</para>
    /// <para>the Internet Explorer Download Manager is detected to be open.</para>
    /// </exception>
    public void WaitForDownloadInformationBar (TimeSpan downloadTimeout)
    {
      var stopwatch = Stopwatch.StartNew();
      while (!IsDownloadInformationBarVisible())
      {
        if (stopwatch.Elapsed >= downloadTimeout)
        {
          if (IsDownloadManagerOpen())
          {
            throw new InvalidOperationException (
                "The Internet Explorer Download Manager window is open. It is not possible to correctly handle the download until the window has been closed. " +
                "Ensure that there is no iexplore.exe instance running in the background and that the Download Manager Window is closed.");
          }
            
          throw new InvalidOperationException ("Could not find the download information bar. This is probably because the download was not triggered correctly.");
        }
       
        Thread.Sleep (s_waitForDownloadInformationBarRetryInterval);
      }
    }

    /// <summary>
    /// Checks if the download information bar is currently visible.
    /// </summary>
    public bool IsDownloadInformationBarVisible ()
    {
      var foregroundWindowHandle = _nativeWindowsMethods.GetForegroundWindow();
      var processID = _nativeWindowsMethods.GetWindowThreadProcessID (foregroundWindowHandle);

      return IsSubclassVisible (processID, c_classNameOfDownloadInformationBar);
    }
    
    private bool IsSubclassVisible (int processID, string className)
    {
      var findWindows = _nativeWindowFinder.FindWindows (
          new WindowFilterCriteria { IncludeChildWindows = true, ProcessID = processID, ClassName = new Regex (className) });

      if (findWindows.Length != 1)
        return false;

      return _nativeWindowsMethods.IsWindowVisible (findWindows.First().WindowHandle);
    }

    private bool IsDownloadManagerOpen ()
    {
      var foundWindows = _nativeWindowFinder.FindWindows (new WindowFilterCriteria { ClassName = new Regex (c_classNameOfDownloadWindow), IncludeChildWindows = true});

      return foundWindows.Any (window => window.WindowText.Contains (c_productNameContainedInDownloadManagerTitle) && _nativeWindowsMethods.IsWindowVisible (window.WindowHandle));
    }
  }
}