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
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.DownloadInfrastructure;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FileDownloadTest : IntegrationTest
  {
    private const string c_sampleZipFileName = "download_06d6ff4d-c124-4d3f-9d96-5e4f2d0c7b0c.zip";
    private const string c_sampleTxtFileNameWithoutExtension = "SampleFile_06d6ff4d-c124-4d3f-9d96-5e4f2d0c7b0c";
    private const string c_sampleTxtFileName = "SampleFile_06d6ff4d-c124-4d3f-9d96-5e4f2d0c7b0c.txt";
    private const string c_sampleXmlFileName = "SampleXmlFile_06d6ff4d-c124-4d3f-9d96-5e4f2d0c7b0c.xml";

    [Test]
    public void TestDownloadReplacesCurrentPage_WithExpectedFileName ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadTxtReplaceSiteButton");
      button.Click();

      Assert.That(
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileName),
          Throws.Nothing);
    }

    [Test]
    public void TestDownloadReplacesCurrentPage_WithUnknownFileName ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadTxtReplaceSiteButton");
      button.Click();
      Assert.That(() => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
    }

    [Test]
    public void TestDownloadReplacesCurrentPage_AnchorWithSelf ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var anchor = home.Scope.FindId("body_TargetSelfAnchor");
      anchor.Click();

      Assert.That(() => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithUnknownFileName ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName();
      Assert.That(downloadedFile.FileName, Is.EqualTo(c_sampleTxtFileName));
      Assert.That(Path.GetFileName(downloadedFile.FullFilePath), Is.EqualTo(c_sampleTxtFileName));
      Assert.That(
          new FileInfo(downloadedFile.FullFilePath).Directory.Parent.FullName,
          Is.EqualTo(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar)));
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithExpectedFileName ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileName);
      Assert.That(Path.GetFileName(downloadedFile.FullFilePath), Is.EqualTo(c_sampleTxtFileName));
      Assert.That(
          new FileInfo(downloadedFile.FullFilePath).Directory.Parent.FullName,
          Is.EqualTo(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar)));
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithAnchorTargetBlank ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      if (Helper.BrowserConfiguration.IsEdge())
        Assert.Ignore("RM-8738: Edge upgrade broke headless mode to still display a popup upon downloading xml-files, cannot automatically download");

      // Note: test for Chrome "safebrowsing" (requires safebrowsing.enabled to be set to true in browser preferences - see Chrome configuration).
      // This test fails if safebrowsing is set to false because downloading an XML file produces an additional user prompt.
      var home = Start();
      var anchor = home.Scope.FindId("body_TargetBlankAnchor");
      anchor.Click();
      IDownloadedFile downloadedFile = null;

      Assert.That(() => downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(), Throws.Nothing);
      Assert.That(downloadedFile.FileName, Is.EqualTo(c_sampleXmlFileName));
    }

    [Test]
    [Category("LongRunning")]
    public void TestDownloadOpensInNewWindow_DownloadTimeoutExceeded_WithUnknownFileName ()
    {
      var downloadStartedTimout = TimeSpan.FromSeconds(10);
      var downloadUpdatedTimeout = TimeSpan.FromSeconds(3);

      var startDownloadLambda = new Action(
          () =>
          {
            var home = Start();
            var button = home.Scope.FindId("body_DownloadWith5SecondTimeout");
            button.Click();
          });


      startDownloadLambda();

      var restartBrowserTask = RestartBrowserToCancelDownloadsAfterDelayAsync(downloadUpdatedTimeout + TimeSpan.FromSeconds(3));

      try
      {
        Assert.That(
            () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName(downloadStartedTimout, downloadUpdatedTimeout),
            Throws.InstanceOf<DownloadResultNotFoundException>()
                .With.Message.StartsWith(
                    string.Format(
                        "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                        downloadUpdatedTimeout)));
      }
      finally
      {
        restartBrowserTask.GetAwaiter().GetResult();
      }
    }

    [Test]
    [Category("LongRunning")]
    public void TestDownloadOpensInNewWindow_DownloadTimeoutExceeded ()
    {
      var downloadStartedTimeout = TimeSpan.FromSeconds(10);
      var downloadUpdatedTimeout = TimeSpan.FromSeconds(3);

      var startDownloadLambda = new Action(
          () =>
          {
            var home = Start();
            var button = home.Scope.FindId("body_DownloadWith5SecondTimeout");
            button.Click();
          });

      startDownloadLambda();

      var restartBrowserTask = RestartBrowserToCancelDownloadsAfterDelayAsync(downloadUpdatedTimeout + TimeSpan.FromSeconds(3));

      try
      {
        Assert.That(
            () =>
                Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(
                    c_sampleTxtFileName,
                    downloadStartedTimeout,
                    downloadUpdatedTimeout),
            Throws.InstanceOf<DownloadResultNotFoundException>()
                .With.Message.StartsWith(
                    string.Format(
                        "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
                        downloadUpdatedTimeout)));
      }
      finally
      {
        restartBrowserTask.GetAwaiter().GetResult();
      }
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithExpectedFileName_FileWithWrongName ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      const string expectedFileName = "WrongFileName";

      Assert.That(
        () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(expectedFileName),
        Throws.InstanceOf<DownloadResultNotFoundException>()
            .With.Message.EqualTo(
              string.Format(
                @"Did not find file with the name '{0}' in the download directory.

Unmatched files in the download directory (will be cleaned up by the infrastructure):
 - {1}",
                expectedFileName,
                c_sampleTxtFileName)));
    }

    [Test]
    public void TestDownloadOpensInNewWindow_NoDownloadTriggered ()
    {
      Start();

      Assert.That(
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(
              c_sampleTxtFileName,
              TimeSpan.FromSeconds(1),
              TimeSpan.FromSeconds(1)),
          Throws.InstanceOf<DownloadResultNotFoundException>()
              .With.Message.EqualTo(
                  "Did not find any new files in the download directory."));
    }

    [Test]
    public void TestDownloadOpensInNewWindow_WithPostback_WithXmlFile ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");


      if (Helper.BrowserConfiguration.IsEdge())
        Assert.Ignore("RM-8738: Edge upgrade broke headless mode to still display a popup upon downloading xml-files, cannot automatically download");
      // Note: test for Chrome "safebrowsing" (requires safebrowsing.enabled to be set to true in browser preferences - see Chrome configuration).
      // This test fails if safebrowsing is set to false because downloading an XML file produces an additional user prompt.

      var home = Start();
      var button = home.Scope.FindId("body_DownloadXmlFile");
      button.Click();

      Assert.That(
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleXmlFileName), Throws.Nothing);
    }

    [Test]
    public void TestDownloadTwice_WithUnknownFileName_PreventsFileNameConflicts ()
    {
      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile1 = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName();
      Assert.That(downloadedFile1.FileName, Is.EqualTo(c_sampleTxtFileName));
      Assert.That(Path.GetFileName(downloadedFile1.FullFilePath), Is.EqualTo(c_sampleTxtFileName));

      button.Click();

      var downloadedFile2 = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName();
      Assert.That(downloadedFile2.FileName, Is.EqualTo(c_sampleTxtFileName));
      Assert.That(Path.GetFileName(downloadedFile2.FullFilePath), Is.EqualTo(c_sampleTxtFileName));

      Assert.That(downloadedFile2.FullFilePath, Is.Not.EqualTo(downloadedFile1.FullFilePath));
    }

    [Test]
    public void TestDownloadTwice_WithExpectedFileName_PreventsFileNameConflicts ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile1 = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileName);
      Assert.That(downloadedFile1.FileName, Is.EqualTo(c_sampleTxtFileName));
      Assert.That(Path.GetFileName(downloadedFile1.FullFilePath), Is.EqualTo(c_sampleTxtFileName));

      button.Click();

      var downloadedFile2 = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileName);
      Assert.That(downloadedFile2.FileName, Is.EqualTo(c_sampleTxtFileName));
      Assert.That(Path.GetFileName(downloadedFile2.FullFilePath), Is.EqualTo(c_sampleTxtFileName));

      Assert.That(downloadedFile2.FullFilePath, Is.Not.EqualTo(downloadedFile1.FullFilePath));
    }

    [Test]
    public void TestDownload_WithUnknownFileName_DeleteFilesRemovesDownloadedFiles ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithDetectedFileName();

      Assert.That(File.Exists(downloadedFile.FullFilePath), Is.True);

      Helper.BrowserConfiguration.DownloadHelper.DeleteFiles();

      Assert.That(File.Exists(downloadedFile.FullFilePath), Is.False);
    }

    [Test]
    public void TestDownload_WithExpectedFileName_DeleteFilesRemovesDownloadedFiles ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadPostbackButton");
      button.Click();

      var downloadedFile = Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileName);

      Assert.That(File.Exists(downloadedFile.FullFilePath), Is.True);

      Helper.BrowserConfiguration.DownloadHelper.DeleteFiles();

      Assert.That(File.Exists(downloadedFile.FullFilePath), Is.False);
    }

    [Test]
    public void TestDownload_HandleDownloadWithoutFileExtension ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();
      var button = home.Scope.FindId("body_DownloadFileWithoutFileExtension");
      button.Click();

      //When downloading a file without a file extension, the Internet Explorer download information bar does not contain an Open-button.
      //This can lead to problems when trying to automate the button click.
      //This unit test is testing if our framework can handle the download information bar when the open-button is missing.
      //The test is not restricted to Internet Explorer, to ensure that no browser has problem with this behavior.
      Assert.That(
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleTxtFileNameWithoutExtension), Throws.Nothing);
    }

    [Test]
    public void TestDownload_HandleZipFileDownload ()
    {
      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("RM-7856: Test does not work properly after Firefox browser upgrade (Version 89).");

      var home = Start();

      var button = home.Scope.FindId("body_DownloadZipFile");
      button.Click();

      //When downloading a file ending with ".zip", Internet Explorer opens the download in a special window.
      //This unit test is testing if our framework can handle this special window.
      //The test is not restricted to Internet Explorer, to ensure that no browser has problem with this behavior.
      Assert.That(
          () => Helper.BrowserConfiguration.DownloadHelper.HandleDownloadWithExpectedFileName(c_sampleZipFileName),
          Throws.Nothing);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("FileDownloadTest.aspx");
    }

    private Task RestartBrowserToCancelDownloadsAfterDelayAsync (TimeSpan delay)
    {
      return Task.Delay(delay)
          .ContinueWith(_ => Helper.OnFixtureTearDown())
          .ContinueWith(_ => Helper.OnFixtureSetUp());
    }
  }
}
