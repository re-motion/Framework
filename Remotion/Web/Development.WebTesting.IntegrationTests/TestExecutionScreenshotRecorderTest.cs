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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TestExecutionScreenshotRecorderTest : IntegrationTest
  {
    private string _tempSavePath = "";

    [SetUp]
    public void SetUp ()
    {
      _tempSavePath = Path.GetTempPath() + Path.GetRandomFileName();

      while (Directory.Exists(_tempSavePath))
        _tempSavePath = Path.GetTempPath() + Path.GetRandomFileName();

      IntegrationTestSetUp();
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists(_tempSavePath))
      {
        var files = Directory.GetFiles(_tempSavePath, "*.png");

        foreach (var file in files)
          File.Delete(file);

        Directory.Delete(_tempSavePath);
      }

      if (IsAlertDialogPresent((IWebDriver)Helper.MainBrowserSession.Driver.Native))
        Helper.MainBrowserSession.Window.AcceptModalDialog();

      IntegrationTestTearDown();
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeDesktopScreenshot_SavesToCorrectPath ()
    {
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var tempFileName = "RandomFileName";
      var suffix = "Desktop";
      var extension = "png";

      var fullPath = CombineToFullPath(_tempSavePath, tempFileName, suffix, extension);

      Assert.That(File.Exists(fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeDesktopScreenshot(tempFileName);
      Assert.That(File.Exists(fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeDesktopScreenshot_ReplacesInvalidFileNameChars ()
    {
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var tempFileName = "<Random\"File\"Na|me>";
      var suffix = "Desktop";
      var extension = "png";

      var tempFileNameWitCharReplaced = "_Random_File_Na_me_";
      var fullPathWitCharReplaced = CombineToFullPath(_tempSavePath, tempFileNameWitCharReplaced, suffix, extension);

      Assert.That(File.Exists(fullPathWitCharReplaced), Is.False);
      testExecutionScreenshotRecorder.TakeDesktopScreenshot(tempFileName);
      Assert.That(File.Exists(fullPathWitCharReplaced), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_SavesToCorrectPath ()
    {
      //Just open the browser so we can take a browser Screenshot
      Start();

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var tempFileName = "RandomFileName";
      var suffix = "Browser0-0";
      var extension = "png";

      var fullPath = CombineToFullPath(_tempSavePath, tempFileName, suffix, extension);

      Assert.That(File.Exists(fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeBrowserScreenshot(tempFileName, new[] {Helper.MainBrowserSession }, Helper.BrowserConfiguration.Locator);
      Assert.That(File.Exists(fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_ReplacesInvalidFileNameChars ()
    {
      //Just open the browser so we can take a browser Screenshot
      Start();

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var tempFileName = "<Random\"File\"Na|me>";
      var suffix = "Browser0-0";
      var extension = "png";

      var tempFileNameWitCharReplaced = "_Random_File_Na_me_";
      var fullPathWitCharReplaced = CombineToFullPath(_tempSavePath, tempFileNameWitCharReplaced, suffix, extension);

      Assert.That(File.Exists(fullPathWitCharReplaced), Is.False);
      testExecutionScreenshotRecorder.TakeBrowserScreenshot(tempFileName, new[] { Helper.MainBrowserSession }, Helper.BrowserConfiguration.Locator);
      Assert.That(File.Exists(fullPathWitCharReplaced), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesNotThrowWhenBrowserDisposed ()
    {
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var fileName = "RandomFileName";
      var fullPath = CombineToFullPath(_tempSavePath, fileName, "Browser0-0", "png");

      var browserSession = Helper.CreateNewBrowserSession();
      browserSession.Dispose();

      Assert.That(
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot(
                  fileName,
                  new[] { browserSession },
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing);

      Assert.That(File.Exists(fullPath), Is.False);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesNotThrowWhenBrowserDisposed_AndTakesNextScreenshotCorrectly ()
    {
      //Just open the browser so we can take a browser Screenshot
      Start();

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var fileName = "RandomFileName";
      var fullPath = CombineToFullPath(_tempSavePath, fileName, "Browser1-0", "png");

      var secondBrowserSession = Helper.CreateNewBrowserSession();
      secondBrowserSession.Dispose();

      Assert.That(File.Exists(fullPath), Is.False);
      Assert.That(
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot(
                  fileName,
                  new[] { secondBrowserSession, Helper.MainBrowserSession},
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing);

      Assert.That(File.Exists(fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesNotThrowWhenAlertWindowIsOpen ()
    {
      if (Helper.BrowserConfiguration.IsEdge())
        Assert.Ignore("Edge v79 does not find an alert dialog. (RM-7387)");

      if (Helper.BrowserConfiguration.IsChrome())
        Assert.Ignore("Chrome v102 does not find an alert dialog. (RM-7387)");

      if (Helper.BrowserConfiguration.IsFirefox())
        Assert.Ignore("Firefox does not show an alert dialog.");

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var fileName = "RandomFileName";

      var home = Start();

      // Produce an alert dialog
      home.Frame.TextBoxes().GetByLocalID("MyTextBox").FillWith("MyText", FinishInput.Promptly);
      var loadFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click(Opt.ContinueImmediately());
      Thread.Sleep(TimeSpan.FromSeconds(1)); //Cannot use normal CompletionDetection, as it would require to close the alert dialog

      Assert.That(IsAlertDialogPresent((IWebDriver)home.Context.Browser.Driver.Native), Is.True);


      Assert.That(
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot(
                  fileName,
                  new[] { Helper.MainBrowserSession },
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing);
    }

    [Test]
    [Ignore("Is currently not working. See RM-6837 for Details.")]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesCreateScreenshotWhenAlertWindowIsOpen ()
    {
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var fileName = "RandomFileName";
      var fullPath = CombineToFullPath(_tempSavePath, fileName, "Browser0-0", "png");

      var home = Start();

      // Produce an alert dialog
      home.Frame.TextBoxes().GetByLocalID("MyTextBox").FillWith("MyText", FinishInput.Promptly);
      var loadFrameFunctionInFrameButton = home.WebButtons().GetByID("LoadFrameFunctionInFrame");
      loadFrameFunctionInFrameButton.Click(Opt.ContinueImmediately());
      Thread.Sleep(TimeSpan.FromSeconds(1)); //Cannot use normal CompletionDetection, as it would require to close the alert dialog

      Assert.That(IsAlertDialogPresent((IWebDriver)home.Context.Browser.Driver.Native), Is.True);

      Assert.That(File.Exists(fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeBrowserScreenshot(
          fileName,
          new[] { Helper.MainBrowserSession },
          Helper.BrowserConfiguration.Locator);

      Assert.That(File.Exists(fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_SavesScreenshotWithShortenedName ()
    {
      //Just open the browser
      Start();

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);
      var fileName = new String('A', 300);
      var suffix = "Browser0-0";
      var extension = "png";

      var fileNameShortened = new String('A', 259 - (_tempSavePath.Length + suffix.Length + extension.Length + 3));
      var fullPath = CombineToFullPath(_tempSavePath, fileNameShortened, suffix, extension);

      Assert.That(File.Exists(fullPath), Is.False);
      Assert.That(
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot(
                  fileName,
                  new[] { Helper.MainBrowserSession },
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing);
      Assert.That(File.Exists(fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeDesktopScreenshot_SavesScreenshotWithShortenedName ()
    {
      var fileName = new String('A', 300);
      var suffix = "Desktop";
      var extension = "png";

      var fileNameShortened = new String('A', 259 - (_tempSavePath.Length + suffix.Length + extension.Length + 3));
      var fullPath = CombineToFullPath(_tempSavePath, fileNameShortened, suffix, extension);

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder(_tempSavePath, NullLoggerFactory.Instance);

      Assert.That(File.Exists(fullPath), Is.False);
      Assert.That(
          () =>
              testExecutionScreenshotRecorder.TakeDesktopScreenshot(
                  fileName),
          Throws.Nothing);
      Assert.That(File.Exists(fullPath), Is.True);
    }

    private string CombineToFullPath (string tempPath, string fileName, string suffix, string extension)
    {
      var fullFileNameWitCharReplaced = string.Format("{0}.{1}.{2}", fileName, suffix, extension);
      var fullPathWitCharReplaced = string.Concat(tempPath, "/", fullFileNameWitCharReplaced);
      return fullPathWitCharReplaced;
    }

    private bool IsAlertDialogPresent (IWebDriver selenium)
    {
      try
      {
        return selenium.SwitchTo() != null && selenium.SwitchTo().Alert() != null;
      }
      catch (NoAlertPresentException)
      {
        return false;
      }
    }

    private MultiWindowTestPageObject Start ()
    {
      return Start<MultiWindowTestPageObject>("MultiWindowTest/Main.wxe");
    }
  }
}
