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
using Moq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{
  [TestFixture]
  public class CompoundUrlMappingFileFinderTest
  {
    [Test]
    public void Initialize ()
    {
      var urlMappingFileFinders = new[] { new Mock<IUrlMappingFileFinder>().Object, new Mock<IUrlMappingFileFinder>().Object };

      var compoundFactory = new CompoundUrlMappingFileFinder(urlMappingFileFinders);

      Assert.That(compoundFactory.UrlMappingFileFinders, Is.Not.SameAs(urlMappingFileFinders));
      Assert.That(compoundFactory.UrlMappingFileFinders, Is.EqualTo(urlMappingFileFinders));
    }

    [Test]
    public void GetUrlMappingFilePaths ()
    {
      var urlMappingFileFinders = new IUrlMappingFileFinder[]
                             {
                                 new UrlMappingFileFinderStub(@"C:\query1.xml", @"C:\query2.xml"),
                                 new UrlMappingFileFinderStub(@"C:\Query3.xml"),
                             };
      var compoundQueryFileFinder = new CompoundUrlMappingFileFinder(urlMappingFileFinders);

      var expectedQueryFiles = new[]
                               {
                                   @"C:\query1.xml",
                                   @"C:\query2.xml",
                                   @"C:\Query3.xml"
                               };
      Assert.That(compoundQueryFileFinder.GetUrlMappingFilePaths(), Is.EqualTo(expectedQueryFiles));
    }

    [Test]
    public void GetUrlMappingFilePaths_RemovesDuplicateQueryFilePaths ()
    {
      var urlMappingFileFinders = new IUrlMappingFileFinder[]
                             {
                                 new UrlMappingFileFinderStub(@"C:\query2.xml", @"C:\Query2.xml", @"C:\query2.xml"),
                                 new UrlMappingFileFinderStub(@"C:\Query2.xml", @"C:\query2.xml", @"C:\query3.xml"),
                             };
      var compoundQueryFileFinder = new CompoundUrlMappingFileFinder(urlMappingFileFinders);

      var expectedQueryFiles = new[]
                               {
                                   @"C:\query2.xml",
                                   @"C:\query3.xml"
                               };
      Assert.That(compoundQueryFileFinder.GetUrlMappingFilePaths(), Is.EqualTo(expectedQueryFiles));
    }

    private class UrlMappingFileFinderStub : IUrlMappingFileFinder
    {
      private readonly string[] _values;

      public UrlMappingFileFinderStub (params string[] values)
      {
        _values = values;
      }

      public IEnumerable<string> GetUrlMappingFilePaths ()
      {
        return _values;
      }
    }
  }
}
