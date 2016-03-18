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

using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FileDownloadTest : IntegrationTest
  {
    [Test]
    public void Test ()
    {
      const string fileName = "SampleFile.txt";

      var downloadHelper = NewDownloadHelper (fileName);
      downloadHelper.AssertFileDoesNotExistYet();

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadButton");
      downloadHelper.PerformDownload (() => button.Click());
      downloadHelper.DeleteFile();
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FileDownloadTest.aspx");
    }
  }

  // Own TestFixture until we know why IE driver is dead after downloading a file (only on TC agents).
  [TestFixture]
  public class XmlFileDownloadTest : IntegrationTest
  {
    [Test]
    public void TestXml ()
    {
      // Note: test for Chrome "safebrowsing" (requires safebrowsing.enabled to be set to true in browser preferences - see App.config).
      const string fileName = "SampleXmlFile.xml";

      var downloadHelper = NewDownloadHelper (fileName);
      downloadHelper.AssertFileDoesNotExistYet();

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadXmlButton");
      downloadHelper.PerformDownload (() => button.Click());
      downloadHelper.DeleteFile();
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FileDownloadTest.aspx");
    }
  }
}