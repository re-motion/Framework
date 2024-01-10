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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{
  [TestFixture]
  public class UrlMappingLoaderTest
  {
    private class StubUrlMappingFileFinder : IUrlMappingFileFinder
    {
      private readonly string[] _urlMappingFilePaths;

      public StubUrlMappingFileFinder (params string[] urlMappingFilePaths)
      {
        _urlMappingFilePaths = urlMappingFilePaths;
      }

      /// <inheritdoc />
      public IEnumerable<string> GetUrlMappingFilePaths ()
      {
        return _urlMappingFilePaths;
      }
    }

    private class StubUrlMappingFileLoader : IUrlMappingFileLoader
    {
      private readonly Dictionary<string, IReadOnlyList<UrlMappingEntry>> _resultLookup = new();

      public StubUrlMappingFileLoader ()
      {
      }

      public void AddFileEntries (string file, params UrlMappingEntry[] entries) => _resultLookup.Add(file, entries);

      /// <inheritdoc />
      public IReadOnlyList<UrlMappingEntry> LoadUrlMappingEntries (string urlMappingFile) => _resultLookup[urlMappingFile];
    }

    [Test]
    public void CreateUrlMappingConfiguration ()
    {
      var entry1 = new UrlMappingEntry(null, typeof(TestFunction), "Test1.wxe");
      var entry2 = new UrlMappingEntry(null, typeof(TestFunction), "Test2.wxe");
      var entry3 = new UrlMappingEntry(null, typeof(TestFunction), "Test3.wxe");

      var stubUrlMappingFileFinder = new StubUrlMappingFileFinder("a", "b", "c");

      var urlMappingFileLoader = new StubUrlMappingFileLoader();
      urlMappingFileLoader.AddFileEntries("a", entry1, entry2);
      urlMappingFileLoader.AddFileEntries("b");
      urlMappingFileLoader.AddFileEntries("c", entry3);

      var urlMappingLoader = new UrlMappingLoader(stubUrlMappingFileFinder, urlMappingFileLoader);
      var urlMappingConfiguration = urlMappingLoader.CreateUrlMappingConfiguration();

      Assert.That(urlMappingConfiguration, Is.Not.Null);

      var urlMappingEntries = new[] { entry1, entry2, entry3 };
      Assert.That(urlMappingConfiguration.Mappings, Is.EquivalentTo(urlMappingEntries));
    }

    [Test]
    public void CreateUrlMappingConfiguration_SameIDDifferentResource_ThrowsException ()
    {
      var urlMappingLoader = CreateMappingLoader(
          new UrlMappingEntry("id1", typeof(TestFunction), "Test1.wxe"),
          new UrlMappingEntry("id1", typeof(TestFunction), "Test2.wxe"));

      Assert.That(
          () => urlMappingLoader.CreateUrlMappingConfiguration(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Two URL mapping entries from files 'second-mapping.xml' and 'first-mapping.xml' have the same ID 'id1', "
                  + "but they point to different resources: '~/Test1.wxe' and '~/Test2.wxe'."));
    }

    [Test]
    public void CreateUrlMappingConfiguration_SameResource_UsesLastEntry ()
    {
      var entry1 = new UrlMappingEntry(null, typeof(TestBaseFunctionWithParameters), "Test1.wxe");
      var entry2 = new UrlMappingEntry(null, typeof(TestDerivedFunctionWithParameters), "Test1.wxe");
      var urlMappingLoader = CreateMappingLoader(entry1, entry2);

      var urlMappingConfiguration = urlMappingLoader.CreateUrlMappingConfiguration();
      Assert.That(urlMappingConfiguration, Is.Not.Null);

      var urlMappingEntries = new[] { entry2 };
      Assert.That(urlMappingConfiguration.Mappings, Is.EquivalentTo(urlMappingEntries));
    }

    [Test]
    public void CreateUrlMappingConfiguration_SameResource_IncompatibleTypes ()
    {
      var urlMappingLoader = CreateMappingLoader(
          new UrlMappingEntry(null, typeof(TestFunction), "Test1.wxe"),
          new UrlMappingEntry(null, typeof(TestFunction2), "Test1.wxe"));

      Assert.That(
          () => urlMappingLoader.CreateUrlMappingConfiguration(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "URL mapping entry for resource '~/Test1.wxe' from the file 'second-mapping.xml' cannot override the existing "
                  + "URL mapping entry from the file 'first-mapping.xml' as the function type "
                  + "'Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions.TestFunction2' is not assignable to the existing "
                  + "function type 'Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions.TestFunction'."));
    }

    private UrlMappingLoader CreateMappingLoader (params UrlMappingEntry[] entries)
    {
      var stubUrlMappingFileFinder = entries.Length > 1
          ? new StubUrlMappingFileFinder("first-mapping.xml", "second-mapping.xml")
          : new StubUrlMappingFileFinder("first-mapping.xml");

      var stubUrlMappingFileLoader = new StubUrlMappingFileLoader();
      stubUrlMappingFileLoader.AddFileEntries("first-mapping.xml", entries.Take(1).ToArray());
      if (entries.Length > 1)
        stubUrlMappingFileLoader.AddFileEntries("second-mapping.xml", entries.Skip(1).ToArray());

      return new UrlMappingLoader(stubUrlMappingFileFinder, stubUrlMappingFileLoader);
    }
  }
}
