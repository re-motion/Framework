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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.Win32WindowSupport;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.WindowFinder.Windows;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer
{
  /// <summary>
  /// Implements the <see cref="IDownloadHelper"/> for Internet Explorer.
  /// </summary>
  public class InternetExplorerDownloadHelper : DownloadHelperBase
  {
    private const string c_partialFileEnding = ".partial";
    private const string c_downloadsDirectoryName = "Downloads";
    private const int c_maximumAttemptyForClosingDownloadInformationBar = 10; //An arbitrary number of maximum closings to not run into an endless loop if the browser does not close the bar. Normally we close one or two bars.

    private static readonly ILog s_log = LogManager.GetLogger (typeof (InternetExplorerDownloadHelper));
    private static readonly TimeSpan s_retryIntervalForClosingDownloadInformationBar = TimeSpan.FromMilliseconds (150);
    
    private readonly TimeSpan _downloadStartedGracePeriod;
    private readonly bool _cleanUpDownloadFolderOnError;
    private readonly string _downloadDirectory;

    /// <summary>
    /// Creates a new <see cref="InternetExplorerDownloadHelper"/>.
    /// </summary>
    /// <param name="downloadStartedTimeout">
    /// Specifies how long the <see cref="DownloadHelperBase"/> should wait before looking for the downloaded file.
    /// </param>
    /// <param name="downloadUpdatedTimeout">
    /// Specifies how long the DownloadHelper should maximally download a file before it fails.
    /// </param>
    /// <param name="cleanUpDownloadFolderOnError">
    /// Clean up the download folder on error.
    /// </param>
    public InternetExplorerDownloadHelper (TimeSpan downloadStartedTimeout, TimeSpan downloadUpdatedTimeout, bool cleanUpDownloadFolderOnError)
        : base (downloadStartedTimeout, downloadUpdatedTimeout)
    {
      //We don't need a grace period, as we trigger the download of the file and therefore know exactly when the download started
      _downloadStartedGracePeriod = TimeSpan.Zero;
      _cleanUpDownloadFolderOnError = cleanUpDownloadFolderOnError;

      //It is not possible to set the Downloads Directory of Internet Explorer programmatically, so we have to use the default DownloadDirectory
      var userProfilePath = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);

      _downloadDirectory = Path.Combine (userProfilePath, c_downloadsDirectoryName);
    }

    public TimeSpan DownloadStartedGracePeriod
    {
      get { return _downloadStartedGracePeriod; }
    }

    public bool CleanUpDownloadFolderOnError 
    {
      get { return _cleanUpDownloadFolderOnError; }
    }

    public string DownloadDirectory 
    {
      get { return _downloadDirectory; }
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForExpectedFileName (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      return new DownloadedFileFinder (_downloadDirectory, c_partialFileEnding, _downloadStartedGracePeriod, new InternetExplorerExpectedFileNameFinderStrategy (fileName));
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForUnknownFileName ()
    {
      return new DownloadedFileFinder (_downloadDirectory, c_partialFileEnding, _downloadStartedGracePeriod, new InternetExplorerUnknownFileNameFileFinderStrategy (c_partialFileEnding));
    }

    protected override IDownloadedFile HandleDownload (DownloadedFileFinder downloadedFileFinder, TimeSpan downloadStartedTimeout, TimeSpan downloadUpdatedTimeout)
    {
      ArgumentUtility.CheckNotNull ("downloadedFileFinder", downloadedFileFinder);

      var filesInDownloadDirectoryBeforeDownload = Directory.GetFiles (_downloadDirectory).Select (Path.GetFileName).ToList();

      EnsureBrowserRespondedToTriggeredDownload (downloadStartedTimeout);
      StartBrowserDownload();

      DownloadedFile downloadedFile = null;

      try
      {
        downloadedFile = downloadedFileFinder.WaitForDownloadCompleted (downloadStartedTimeout, downloadUpdatedTimeout, filesInDownloadDirectoryBeforeDownload);
      }
      catch (DownloadResultNotFoundException ex)
      {
        if (_cleanUpDownloadFolderOnError)
          CleanUpUnmatchedDownloadedFiles (ex.GetUnmatchedFilesInDownloadDirectory().ToList());

        throw;
      }
      finally
      {
        //We ensured that the download information bar appeared, therefore we should ensure it is guaranteed closed after the download to not interfere with future tests.
        //For the current test, the download information bar wont be contained in any screenshot. We expect all relevant information to be contained in the exception.
        AfterDownloadCompleted();
      }

      var correctedDownloadedFile = NormalizeFileName (downloadedFile);

      var movedDownloadedFile = MoveDownloadedFile (correctedDownloadedFile);
      return movedDownloadedFile;
    }

    private void EnsureBrowserRespondedToTriggeredDownload (TimeSpan downloadStartedTimeout)
    {
      var nativeMethodsExtended = new Win32WindowsNativeMethodsExtended ();
      var windowFinder = new NativeWindowFinder (new Win32WindowsNativeMethods());

      var internetExplorerNativeWindowsHandler = new InternetExplorerNativeWindowHandler (windowFinder, nativeMethodsExtended);
      try
      {
        internetExplorerNativeWindowsHandler.WaitForDownloadInformationBar (downloadStartedTimeout);
      }
      catch (InvalidOperationException ex)
      {
        throw new DownloadResultNotFoundException (ex.Message, new List<string>());
      }
    }

    private void StartBrowserDownload ()
    {
      //Wait a little bit, following keypress does not get registered correctly when we go in too fast. 
      //Needs min. 400 ms on a fast developer machine. So we take 400 * 3 to be sure it works on slower machines.
      Thread.Sleep (1200); 
      SendKeys.SendWait ("%s"); //"Alt + s" -> Press the save button of the download information bar
    }

    private void AfterDownloadCompleted ()
    {
      var nativeMethodsExtended = new Win32WindowsNativeMethodsExtended();
      var windowFinder = new NativeWindowFinder (new Win32WindowsNativeMethods());
      
      var internetExplorerNativeWindosHandler = new InternetExplorerNativeWindowHandler (windowFinder, nativeMethodsExtended);

      for (var i = 0; i < c_maximumAttemptyForClosingDownloadInformationBar && internetExplorerNativeWindosHandler.IsDownloadInformationBarVisible(); i++)
      {
        SendKeys.SendWait ("%q"); //"Alt + q" -> Close the download information bar
        Thread.Sleep (s_retryIntervalForClosingDownloadInformationBar);
      }

      if (internetExplorerNativeWindosHandler.IsDownloadInformationBarVisible())
        throw new InvalidOperationException ("Could not close the download information bar.");
    }

    /// <summary>
    /// In some cases, Internet Explorer appends 8 random characters between file name and file extension.
    /// Changes the <see cref="DownloadedFile.FileName"/> of the <see cref="DownloadedFile"/> wrapper, but does not change the physical location of the file.
    /// </summary>
    private DownloadedFile NormalizeFileName (DownloadedFile downloadedFile)
    {
      var fileNameExtension = Path.GetExtension (downloadedFile.FileName);
      if (fileNameExtension == null)
        return downloadedFile;

      var pattern = string.Format ("(.*)([0-9A-Z]{{8}})({0})", Regex.Escape (fileNameExtension));

      if (Regex.IsMatch (downloadedFile.FileName, pattern))
        return downloadedFile.Rename (Regex.Replace (downloadedFile.FileName, pattern, "$1$3"));

      return downloadedFile;
    }

    private void CleanUpUnmatchedDownloadedFiles ([NotNull] IEnumerable<string> unmatchedFiles)
    {
      ArgumentUtility.CheckNotNull ("unmatchedFiles", unmatchedFiles);

      foreach (var file in unmatchedFiles)
      {
        var fullFilePath = Path.Combine (_downloadDirectory, file);
        
        try
        {
          //We don't wait for the file to be deleted, as we expect it to be deleted in time
          File.Delete (fullFilePath);
        }
        catch (IOException ex)
        {
          s_log.WarnFormat (@"Could not delete '{0}'.
{1}", fullFilePath, ex);
        }
      }
    }

    protected override void BrowserSpecificCleanup ()
    {
      //No Browser specific cleanup needed, as every file gets moved from the default download directory into the temp directory.
    }
  }
}