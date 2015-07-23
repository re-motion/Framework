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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests which must deal with downloads.
  /// </summary>
  public class DownloadHelper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (DownloadHelper));

    private readonly IBrowserConfiguration _browserConfiguration;
    private readonly string _fileName;
    private readonly string _fullFilePath;
    private readonly TimeSpan _maxDownloadTimeSpan;

    public DownloadHelper ([NotNull] IBrowserConfiguration browserConfiguration, [NotNull] string fileName, TimeSpan maxDownloadTimeSpan)
    {
      ArgumentUtility.CheckNotNull ("browserConfiguration", browserConfiguration);
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      _browserConfiguration = browserConfiguration;
      _fileName = fileName;
      _fullFilePath = Path.Combine (GetDownloadsDirectory(), _fileName);
      _maxDownloadTimeSpan = maxDownloadTimeSpan;
    }

    /// <summary>
    /// Asserts that there is no local file with the given name yet.
    /// </summary>
    public void AssertFileDoesNotExistYet ()
    {
      var message = string.Format (
          "Make sure that the file '{0}' does not exist in the user's download directory (full path: '{1}').",
          _fileName,
          _fullFilePath);

      Assertion.IsFalse (File.Exists (_fullFilePath), message);
    }

    /// <summary>
    /// Performs the actual download (triggered by the given <paramref name="downloadTrigger"/>). The download is only allowed to take at most the
    /// configured maximum donwload time before PerformDownload throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="downloadTrigger">An action triggering the start of the download.</param>
    /// <returns>The full path of the downloaded file.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the completion of the download takes too long.</exception>
    public string PerformDownload ([NotNull] Action downloadTrigger)
    {
      s_log.DebugFormat ("Performing download of '{0}'...", _fileName);

      // Note: run download trigger in different thread - Selenium does not return until the IE download handling has been performed.
      Task.Factory.StartNew (downloadTrigger);

      // Todo RM-6337: Find a better way to handle the yellow download bar in IE11.
      if (_browserConfiguration.BrowserIsInternetExplorer())
      {
        Thread.Sleep (1500); // do not press too fast, IE-security in place
        SendKeys.SendWait ("{F6}{TAB}{ENTER}");
      }

      var errorMessage = string.Format ("File download of '{0}' failed.", _fileName);
      new RetryUntilTimeout (
          () => Assertion.IsTrue (File.Exists (_fullFilePath), errorMessage),
          _maxDownloadTimeSpan,
          TimeSpan.FromMilliseconds (250)).Run();

      if (_browserConfiguration.BrowserIsInternetExplorer())
      {
        Thread.Sleep (1500); // do not press too fast, IE-security in place
        SendKeys.SendWait ("{F6}{TAB}{TAB}{TAB}{ENTER}");
      }

      s_log.DebugFormat ("Download to '{0}' successfully completed", _fullFilePath);

      return _fullFilePath;
    }

    /// <summary>
    /// Deletes the local file.
    /// </summary>
    public void DeleteFile ()
    {
      File.Delete (_fullFilePath);
      Assertion.IsFalse (File.Exists (_fullFilePath), string.Format ("Deletion of file '{0}' failed.", _fullFilePath));
    }

    private static string GetDownloadsDirectory ()
    {
      var userProfilePath = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
      const string downloadsDirectoryName = "Downloads";

      return Path.Combine (userProfilePath, downloadsDirectoryName);
    }
  }
}