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
using System.Xml;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{
  [TestFixture]
  public class UrlMappingFileLoaderTest
  {
    private const string c_validUrlMappingContent = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<urlMapping
    xmlns=""http://www.re-motion.org/Web/ExecutionEngine/UrlMapping/1.0"" 
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://www.re-motion.org/Web/ExecutionEngine/UrlMapping/1.0 UrlMapping.xsd"">
  <add type=""Remotion.Web.UnitTests::Core.ExecutionEngine.TestFunctions.TestFunction"" resource=""Test1.wxe"" />
  <add type=""Remotion.Web.UnitTests::Core.ExecutionEngine.TestFunctions.TestFunction2"" id=""test2"" resource=""Test2.wxe"" />
</urlMapping>
";

    private string _testFilePath;

    [SetUp]
    public void SetUp ()
    {
      _testFilePath = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown ()
    {
      File.Delete(_testFilePath);
    }

    [Test]
    public void LoadUrlMappingEntries ()
    {
      var urlMappingFileLoader = new UrlMappingFileLoader();
      File.WriteAllText(_testFilePath, c_validUrlMappingContent);

      var entries = urlMappingFileLoader.LoadUrlMappingEntries(_testFilePath);
      Assert.That(entries.Count, Is.EqualTo(2));
      Assert.That(entries[0].FunctionType, Is.EqualTo(typeof(TestFunction)));
      Assert.That(entries[0].ID, Is.Null);
      Assert.That(entries[0].Resource, Is.EqualTo("~/Test1.wxe"));
      Assert.That(entries[1].FunctionType, Is.EqualTo(typeof(TestFunction2)));
      Assert.That(entries[1].ID, Is.EqualTo("test2"));
      Assert.That(entries[1].Resource, Is.EqualTo("~/Test2.wxe"));
    }

    [Test]
    public void LoadUrlMappingEntries_InvalidXml_ThrowsException ()
    {
      var urlMappingFileLoader = new UrlMappingFileLoader();

      Assert.That(
          () => urlMappingFileLoader.LoadUrlMappingEntries(_testFilePath),
          Throws.TypeOf<XmlException>()
              .With.Message.EqualTo($"Error reading {new Uri(_testFilePath)} (0,0): There is an error in XML document (0, 0)."));
    }
  }
}
