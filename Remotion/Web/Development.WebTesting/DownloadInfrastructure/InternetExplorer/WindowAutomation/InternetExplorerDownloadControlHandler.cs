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
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.AutomationElementWrapper;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.Win32WindowSupport;
using Remotion.WindowFinder;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation
{
  public class InternetExplorerDownloadControlHandler
  {
    private const string c_classNameOfDownloadInformationBar = "Frame Notification Bar";
    private const string c_classNameOfDownloadWindow = "#32770";
    private const string c_productNameContainedInDownloadManagerTitle = "Internet Explorer";

    private static readonly TimeSpan s_retryIntervalForSearchingDownloadInformationBarOrDownloadManager = TimeSpan.FromMilliseconds (50);
    private static readonly TimeSpan s_waitTimeBeforeInteractingWithNewAutomationElement = TimeSpan.FromMilliseconds (1200);
    private readonly int _processID;
    private readonly IWindowFinder _windowFinder;
    private readonly IWin32WindowsNativeMethodsExtended _win32WindowsNativeMethodsExtended;

    public InternetExplorerDownloadControlHandler (
        [NotNull] IWindowFinder windowFinder,
        [NotNull] IWin32WindowsNativeMethodsExtended win32WindowsNativeMethodsExtended,
        int pid)
    {
      ArgumentUtility.CheckNotNull ("windowFinder", windowFinder);
      ArgumentUtility.CheckNotNull ("win32WindowsNativeMethodsExtended", win32WindowsNativeMethodsExtended);

      _windowFinder = windowFinder;
      _win32WindowsNativeMethodsExtended = win32WindowsNativeMethodsExtended;
      _processID = pid;
    }

    public void StartDownload (TimeSpan downloadStartedTimeout)
    {
      var hasDownloadStarted = false;
      InternetExplorerDownloadManagerWrapper downloadManager = null;

      var stopwatch = Stopwatch.StartNew();
      do
      {
        Thread.Sleep (s_retryIntervalForSearchingDownloadInformationBarOrDownloadManager);

        // Handle the download manager
        if (downloadManager == null)
          downloadManager = GetDownloadManager();
        if (downloadManager != null)
        {
          downloadManager.Refresh();

          var download = GetPossibleDownloadItem (downloadManager);
          if (download != null)
          {
            // Pressing the save button to early does not cause a click event to happen
            Thread.Sleep (s_waitTimeBeforeInteractingWithNewAutomationElement);

            download.Save();
            downloadManager.Close();

            hasDownloadStarted = true;
            break;
          }
        }

        // Handle the download bar
        var downloadBar = GetDownloadBar();
        if (downloadBar != null)
        {
          // Pressing the save button to early does not cause a click event to happen
          Thread.Sleep (s_waitTimeBeforeInteractingWithNewAutomationElement);

          downloadBar.Save();

          hasDownloadStarted = true;
          break;
        }
      } while (stopwatch.Elapsed < downloadStartedTimeout);

      if (!hasDownloadStarted)
        throw new InvalidOperationException ("Could not start the download: Could not find download information bar or download manager.");
    }

    /// <summary>
    /// Returns <see langword="true"/> if a notification bar was open and has been closed, <see langword="false"/> otherwise.
    /// </summary>
    public bool TryCloseOpenNotificationBar ()
    {
      var notificationBar = GetDownloadBar();
      if (notificationBar == null)
        return false;

      notificationBar.Close();
      return true;
    }

    [CanBeNull]
    private InternetExplorerDownloadListItemWrapper GetPossibleDownloadItem (InternetExplorerDownloadManagerWrapper downloadManager)
    {
      var possibleDownloads = downloadManager.Items.Take (2).ToArray();
      if (possibleDownloads.Count (d => d.HasSaveButton) > 1)
        throw new NotSupportedException ("Multiple queued downloads were found but the download handling only supports one download at a time.");

      if (possibleDownloads[0].HasSaveButton)
        return possibleDownloads[0];

      return null;
    }

    [CanBeNull]
    private InternetExplorerDownloadNotificationBarWrapper GetDownloadBar ()
    {
      // Find the download bar window
      var downloadBarWindow = _windowFinder.FindWindows (
          new WindowFilterCriteria
          {
              ClassName = new Regex (c_classNameOfDownloadInformationBar),
              IncludeChildWindows = true,
              ProcessID = _processID,
          }).FirstOrDefault (w => _win32WindowsNativeMethodsExtended.IsWindowVisible (w.WindowHandle));

      if (downloadBarWindow == null)
        return null;

      return InternetExplorerDownloadNotificationBarWrapper.CreateFromHandle (downloadBarWindow.WindowHandle);
    }

    [CanBeNull]
    private InternetExplorerDownloadManagerWrapper GetDownloadManager ()
    {
      // Find the download manager window
      var downloadManagerWindow = _windowFinder.FindWindows (
          new WindowFilterCriteria
          {
              ClassName = new Regex (c_classNameOfDownloadWindow),
              IncludeChildWindows = true
          }).FirstOrDefault (w => w.WindowText.Contains (c_productNameContainedInDownloadManagerTitle) && _win32WindowsNativeMethodsExtended.IsWindowVisible (w.WindowHandle));

      if (downloadManagerWindow == null)
        return null;

      return InternetExplorerDownloadManagerWrapper.CreateFromHandle (downloadManagerWindow.WindowHandle);
    }
  }
}