// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderApplicationTest : AclToolsTestBase
  {
    private const string c_cssFileContent = 
    #region 
        @"table 
{
  background-color: white;
  border-color: black;
  border-style: solid;
  border-width: 1px;
  table-layout: auto;
  border-collapse: collapse;
  border-spacing: 10px;
  empty-cells: show;
  caption-side: top;
  font-family: Arial, Helvetica, sans-serif;
  vertical-align: text-top;
  text-align: left;
}

th, td
{
  border-style: solid;
  border-color: black;
  border-width: 1px;
  table-layout: auto;
  border-collapse: collapse;
  border-spacing: 1px;
  padding: 5px;
} 

th
{
   background-color: #CCCCCC;
}    ";
    #endregion


    [Test]
    public void FindAllUsersTest ()
    {
      var settings = new AclExpanderApplicationSettings();
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (20));
    }


    [Test]
    public void FirstNameFilterTest ()
    {
      const string firstName = "test";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.FirstName, Is.EqualTo (firstName));
      }
    }


    [Test]
    public void LastNameFilterTest ()
    {
      const string lastName = "user1";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserLastName = lastName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (4));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.LastName, Is.EqualTo (lastName));
      }
    }


    [Test]
    public void UserNameFilterTest ()
    {
      const string userName = "test.user";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.UserName, Is.EqualTo (userName));
      }
    }



    [Test]
    public void AllNamesFilterTest ()
    {
      const string firstName = "test";
      const string lastName = "user";
      const string userName = "test.user";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      settings.UserLastName = lastName;
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (8));
      foreach (AclExpansionEntry entry in aclExpansion)
      {
        Assert.That (entry.User.FirstName, Is.EqualTo (firstName));
        Assert.That (entry.User.LastName, Is.EqualTo (lastName));
        Assert.That (entry.User.UserName, Is.EqualTo (userName));
      }
    }


    [Test]
    public void AllNamesFilterTest2 ()
    {
      const string firstName = "test";
      const string lastName = "user";
      const string userName = "group1/user2";
      var settings = new AclExpanderApplicationSettings ();
      settings.UserFirstName = firstName;
      settings.UserLastName = lastName;
      settings.UserName = userName;
      var aclExpansion = CreateAclExpanderApplicationAndCallGetAclExpansion (settings);
      Assert.That (aclExpansion.Count, Is.EqualTo (0));
    }



    [Test]
    public void RunSingleFileOutputDirectoryAndExtensionSettingTest ()
    {
      const string directory = "The Directory";

      var mocks = new MockRepository();
      var textWriterFactoryMock = mocks.DynamicMock<ITextWriterFactory> ();

      textWriterFactoryMock.Expect (mock => mock.Directory = directory); 
      textWriterFactoryMock.Expect (mock => mock.CreateTextWriter (Arg<String>.Is.Anything)).Return (new StringWriter ());
      textWriterFactoryMock.Expect (mock => mock.CreateTextWriter (Arg<String>.Is.Anything, Arg<String>.Is.Anything, Arg<String>.Is.Anything)).Return (new StringWriter ());
      
      textWriterFactoryMock.Replay();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = directory;
      var application = new AclExpanderApplication (textWriterFactoryMock);

      application.Run (settings, TextWriter.Null, TextWriter.Null);

      textWriterFactoryMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetCultureNameTest ()
    {
      AssertGetCultureName(null,null);
      AssertGetCultureName ("", null);
      AssertGetCultureName ("de-AT", "de-AT");
      AssertGetCultureName ("en-US", "en-US");
    }

    [Test]
    public void MultipleFileOutputWritingTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = true;
      string path = Path.GetTempPath ();
      //string path = "c:\\temp";
      string testDirectory = Path.Combine(path, "TestDirectory");
      settings.Directory = testDirectory;
      var application = new AclExpanderApplication (streamWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      string outputDirectory = streamWriterFactory.Directory;
      AssertFileExists (outputDirectory, "group0_user1.html");
      AssertFileExists (outputDirectory, "group0_user2.html");
      AssertFileExists (outputDirectory, "group1_user1.html");
      AssertFileExists (outputDirectory, "group1_user2.html");

      AssertFileExists (outputDirectory, "_AclExpansionMain_.html");
      AssertFileExists (outputDirectory, "AclExpansion.css");

      AssertFileExists (outputDirectory, "test.user.html"); 
    }


    [Test]
    public void MultipleFileOutputCssFileWritingTest ()
    {
      var stringWriterFactory = new StringWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = true;
      settings.Directory = "";
      var application = new AclExpanderApplication (stringWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      // Multifile HTML output => expect at least 3 files (CSS, main HTML, detail HTML files)
      Assert.That (stringWriterFactory.Count, Is.GreaterThanOrEqualTo (3));

      const string cssFileName = AclExpanderApplication.CssFileName;  
      TextWriterData cssTextWriterData;
      bool cssFileExists = stringWriterFactory.NameToTextWriterData.TryGetValue(cssFileName,out cssTextWriterData);
      Assert.That (cssFileExists, Is.True);

      string result = cssTextWriterData.TextWriter.ToString();
      //Clipboard.SetText (AclExpansionHtmlWriterTest.CreateLiteralResultExpectedString (result));
      Assert.That (result, Is.EqualTo (c_cssFileContent));
    }


    [Test]
    public void SingleFileOutputCssFileWritingTest ()
    {
      var stringWriterFactory = new StringWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = "";
      var application = new AclExpanderApplication (stringWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      // Single file HTML output => expect 2 files (CSS, HTML file)
      Assert.That (stringWriterFactory.Count, Is.EqualTo (2));

      const string cssFileName = AclExpanderApplication.CssFileName;
      TextWriterData cssTextWriterData;
      bool cssFileExists = stringWriterFactory.NameToTextWriterData.TryGetValue (cssFileName, out cssTextWriterData);
      Assert.That (cssFileExists, Is.True);

      string result = cssTextWriterData.TextWriter.ToString ();
      //Clipboard.SetText (AclExpansionHtmlWriterTest.CreateLiteralResultExpectedString (result));
      Assert.That (result, Is.EqualTo (c_cssFileContent));
    }



    [Test]
    public void MultipleFileOutputCssFileWritingUsingStreamWriterTest ()
    {
      string path = Path.Combine(Path.GetTempPath (),"mf");
      if (Directory.Exists (path))
      {
        Directory.Delete (path, true);
      }
      var streamWriterFactory = new StreamWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = true;
      settings.Directory = path;
      var application = new AclExpanderApplication (streamWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      const string cssFileName = AclExpanderApplication.CssFileName;
      TextWriterData cssTextWriterData;
      streamWriterFactory.NameToTextWriterData.TryGetValue (cssFileName, out cssTextWriterData);

      // Multifile HTML output => expect at least 3 files (CSS, main HTML, detail HTML files)
      Assert.That (Directory.GetFiles (cssTextWriterData.Directory).Length, Is.EqualTo (8));

      Assert.That (File.Exists (Path.Combine (cssTextWriterData.Directory, Path.ChangeExtension (cssFileName, "css"))), Is.True);
    }

    [Test]
    public void SingleFileOutputCssFileWritingUsingStreamWriterTest ()
    {
      string path = Path.Combine (Path.GetTempPath (), "sf");
      if (Directory.Exists (path))
      {
        Directory.Delete (path, true);
      }
      var streamWriterFactory = new StreamWriterFactory ();

      var settings = new AclExpanderApplicationSettings ();
      settings.UseMultipleFileOutput = false;
      settings.Directory = path;
      var application = new AclExpanderApplication (streamWriterFactory);
      application.Run (settings, TextWriter.Null, TextWriter.Null);

      // Single file HTML output => expect 2 files (CSS, HTML file)
      Assert.That (Directory.GetFiles (path).Length, Is.EqualTo (2));

      const string cssFileName = AclExpanderApplication.CssFileName;
      Assert.That (File.Exists (Path.Combine (path, Path.ChangeExtension(cssFileName,"css"))), Is.True);


    }


    [Test]
    public void VerboseSettingTest ()
    {
      var mocks = new MockRepository ();
      var textWriterFactoryStub = mocks.Stub<ITextWriterFactory> ();
      var applicationMock = mocks.PartialMock<AclExpanderApplication> (textWriterFactoryStub);


      var aclExpansionEntries = new List<AclExpansionEntry>();
      applicationMock.Expect (x => x.GetAclExpansion ()).Return(aclExpansionEntries);
      applicationMock.Expect (x => x.WriteAclExpansionAsHtmlToStreamWriter (Arg<List<AclExpansionEntry>>.Is.Equal (aclExpansionEntries)));

      mocks.ReplayAll();

      var settings = new AclExpanderApplicationSettings ();
      applicationMock.Run (settings, TextWriter.Null, TextWriter.Null);

      applicationMock.VerifyAllExpectations();
    }



    private List<AclExpansionEntry> CreateAclExpanderApplicationAndCallGetAclExpansion (AclExpanderApplicationSettings settings)
    {
      var application = new AclExpanderApplication ();
      application.Init (settings);
      return application.GetAclExpansion(); 
    }

    private static void AssertGetCultureName (string cultureNameIn, string cultureNameOut)
    {
      var textWriterFactoryStub = MockRepository.GenerateStub<ITextWriterFactory> ();
      var application = new AclExpanderApplication (textWriterFactoryStub);
      var settings = new AclExpanderApplicationSettings ();
      settings.CultureName = cultureNameIn;
      application.Init (settings);
      string cultureName = application.GetCultureName ();
      Assert.That (cultureName, Is.EqualTo (cultureNameOut));
    }

    private static void AssertFileExists (string testDirectory, string fileName)
    {
      string fileNameExpected = Path.Combine (testDirectory, fileName);
      Assert.That (File.Exists (fileNameExpected), Is.True);
    }


  }
}

