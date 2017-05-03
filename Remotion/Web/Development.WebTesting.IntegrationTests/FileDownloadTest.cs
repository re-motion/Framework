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
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FileDownloadTest : IntegrationTest
  {
    [Test]
    public void TestDownloadReplacesCurrentPage_WithExpectedFileName ()
    {
      if (Helper.BrowserConfiguration.IsInternetExplorer())
        Assert.Ignore ("File downloads which replace the current page are currently broken in IE. See https://github.com/SeleniumHQ/selenium/issues/1843 for more information.");

      const string fileName = "SampleFile.txt";

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadTxtReplaceSiteButton");
      button.Click();

      Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (fileName),
          Throws.Nothing);
    }

    [Test]
    public void TestDownloadReplacesCurrentPage_WithUnknownFileName ()
    {
      if (Helper.BrowserConfiguration.IsInternetExplorer())
        Assert.Ignore ("File downloads which replace the site are currently broken in IE. See https://github.com/SeleniumHQ/selenium/issues/1843 for more information.");

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadTxtReplaceSiteButton");
      button.Click();
      Assert.That (() => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
    }

    [Test]
    public void TestDownloadReplacesCurrentPage_AnchorWithSelf ()
    {
      if (Helper.BrowserConfiguration.IsInternetExplorer())
        Assert.Ignore ("File downloads which replace the site are currently broken in IE. See https://github.com/SeleniumHQ/selenium/issues/1843 for more information.");

      var home = Start();
      var anchor = home.Scope.FindId ("body_TargetSelfAnchor");
      anchor.Click();

      Assert.That (() => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithUnknownFileName ()
    {
      var home = Start();
      var button = home.Scope.FindId ("body_DownloadPostbackButton");
      button.Click();
      IDownloadedFile downloadedFile = null;

      Assert.That (() => downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
      Assert.That ("SampleFile.txt", Is.EqualTo (downloadedFile.FileName));
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithExpectedFileName ()
    {
      var home = Start();
      var button = home.Scope.FindId ("body_DownloadPostbackButton");
      button.Click();

      Assert.That (() => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName ("SampleFile.txt"), Throws.Nothing);
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithAnchorTargetBlank ()
    {
      var home = Start();
      var anchor = home.Scope.FindId ("body_TargetBlankAnchor");
      anchor.Click();
      IDownloadedFile downloadedFile = null;

      Assert.That (() => downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
      Assert.That ("SampleXmlFile.xml", Is.EqualTo (downloadedFile.FileName));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestDownloadOpensInNewWindow_DownloadTimeoutExceeded_WithUnknownFileName ()
    {
      var downloadStartedTimout = TimeSpan.FromSeconds (10);
      var downloadUpdatedTimeout = TimeSpan.FromSeconds (3);

      var startDownloadLambda = new Action (
        () =>
        {
          var home = Start();
          var button = home.Scope.FindId ("body_DownloadWith5SecondTimeout");
          button.Click();
        });

      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        var downloadDirectory = ((InternetExplorerDownloadHelper) Helper.BrowserConfiguration.DownloadHelper).DownloadDirectory;
        var filesInDownloadDirectoryBeforeDownload = Directory.GetFiles (downloadDirectory);

        startDownloadLambda();

        Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName (downloadStartedTimout, downloadUpdatedTimeout),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.StartsWith (
                string.Format (
                  "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                  downloadUpdatedTimeout)));

        //The browser continues writing in the download file, therefore we have to wait a little bit, so that it does not interfere with the next test.
        Thread.Sleep (TimeSpan.FromSeconds (3));

        //InternetExplorer sometimes opens a download information bar to inform us, that the downloaded file was deleted. 
        //We close the bar (by sending the keys "alt + q") as it potentially interferes with future tests.
        SendKeys.SendWait ("%q");

        //InternetExplorer works in the default download directory of the user. If cleanUpUnmatchedDownloadedFiles is set to false in config, the unmatched file is not deleted by the WebTestFramework. 
        //Therefore, we have to delete these files manually.
        CleanupPartialSampleFiles (downloadDirectory, filesInDownloadDirectoryBeforeDownload);
      }
      else if (Helper.BrowserConfiguration.IsChrome())
      {
        startDownloadLambda();

        Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName (downloadStartedTimout, downloadUpdatedTimeout),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.StartsWith (
                string.Format (
                  "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                  downloadUpdatedTimeout)));


        //The browser continues writing in the download file, therefore we have to wait a little bit so that it does not interfere with the next test.
        Thread.Sleep (TimeSpan.FromSeconds (3));
      }
    }

    [Test]
    [Category ("LongRunning")]
    public void TestDownloadOpensInNewWindow_DownloadTimeoutExceeded ()
    {
      var downloadStartedTimeout = TimeSpan.FromSeconds (10);
      var downloadUpdatedTimeout = TimeSpan.FromSeconds (3);
      const string fileName = "SampleFile.txt";

      var startDownloadLambda = new Action (
        () =>
        {
          var home = Start();
          var button = home.Scope.FindId ("body_DownloadWith5SecondTimeout");
          button.Click();
        });

      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        var downloadDirectory = ((InternetExplorerDownloadHelper) Helper.BrowserConfiguration.DownloadHelper).DownloadDirectory;
        var filesInDownloadDirectoryBeforeDownload = Directory.GetFiles (downloadDirectory);

        startDownloadLambda();

        Assert.That (
          () =>
            Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (
              fileName,
              downloadStartedTimeout,
              downloadUpdatedTimeout),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.StartsWith (
                string.Format (
                  "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                  downloadUpdatedTimeout)));

        //The browser continues writing in the download file, therefore we have to wait a little bit so that does not interfere with the next test.
        Thread.Sleep (TimeSpan.FromSeconds (3));

        //InternetExplorer sometimes opens a download information bar to inform us, that the downloaded file was deleted. 
        //We close the bar (by sending the keys "alt + q") as it potentially interferes with future tests.
        SendKeys.SendWait ("%q");

        //InternetExplorer works in the default download directory of the user. If cleanUpUnmatchedDownloadedFiles is set to false in config, the unmatched file is not deleted by the WebTestFramework. 
        //Therefore, we have to delete these files manually.
        CleanupPartialSampleFiles (downloadDirectory, filesInDownloadDirectoryBeforeDownload);
      }
      else if (Helper.BrowserConfiguration.IsChrome())
      {
        startDownloadLambda();
        
        Assert.That (
          () =>
            Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (
              fileName,
              downloadStartedTimeout,
              downloadUpdatedTimeout),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.StartsWith (
                string.Format (
                  "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                  downloadUpdatedTimeout)));

        //The browser continues writing in the download file, therefore we have to wait a little bit so that does not interfere with the next test.
        Thread.Sleep (TimeSpan.FromSeconds (3));
      }
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithExpectedFileName_FileWithWrongName ()
    {
      var home = Start();
      var button = home.Scope.FindId ("body_DownloadPostbackButton");
      button.Click();

      const string realFileName = "SampleFile.txt"; 
      const string expectedFileName = "WrongFileName";

      Assert.That (
        () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (expectedFileName),
        Throws.InstanceOf<DownloadResultNotFoundException>()
            .With.Message.EqualTo (
              string.Format (
                @"Did not find file with the name '{0}' in the download directory.

Unmatched files in the download directory (will be cleaned up by the infrastructure):
 - {1}",
                expectedFileName,
                realFileName)));

      //InternetExplorer works in the default download directory of the user. If cleanUpUnmatchedDownloadedFiles is set to false in config, the unmatched file is not deleted by the WebTestFramework. 
      //Therefore, we have to delete these files manually.
      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        var downloadDirectory = ((InternetExplorerDownloadHelper) Helper.BrowserConfiguration.DownloadHelper).DownloadDirectory;
        var fullFilePath = Path.Combine (downloadDirectory, realFileName);
        File.Delete (fullFilePath);
      }
    }

    [Test]
    public void TestDownloadOpensInNewWindow_NoDownloadTriggered()
    { 
      Start();

      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (
            "SampleFile.txt",
            TimeSpan.FromSeconds (1),
            TimeSpan.FromSeconds (1)),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.EqualTo (
                "Could not find the download information bar. This is probably because the download was not triggered correctly."));
      }
      else if (Helper.BrowserConfiguration.IsChrome ())
      {
        Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (
            "SampleFile.txt",
            TimeSpan.FromSeconds (1),
            TimeSpan.FromSeconds (1)),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.EqualTo (
                "Did not find any new files in the download directory."));
      }
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithXmlFile ()
    {
      // Note: test for Chrome "safebrowsing" (requires safebrowsing.enabled to be set to true in browser preferences - see Chrome configuration).
      const string fileName = "SampleXmlFile.xml";

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadXmlFile");
      button.Click();

      Assert.That (
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName (fileName), Throws.Nothing);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FileDownloadTest.aspx");
    }

    private void CleanupPartialSampleFiles (string downloadDirectory, IEnumerable<string> filesInDownloadDirectoryBeforeDownload)
    {
      var newFilesInDownloadDirectoryAfterDownload = Directory.GetFiles (downloadDirectory).Except (filesInDownloadDirectoryBeforeDownload);

      var filesToBeDeleted = newFilesInDownloadDirectoryAfterDownload.Select (e => new { FullPath = e, FileName = Path.GetFileName (e) })
          .Where (x => x.FileName.StartsWith ("SampleFile.txt.") && x.FileName.EndsWith ("partial"));

      foreach (var file in filesToBeDeleted)
      {
        File.Delete (file.FullPath);
      }
    }
  }
}