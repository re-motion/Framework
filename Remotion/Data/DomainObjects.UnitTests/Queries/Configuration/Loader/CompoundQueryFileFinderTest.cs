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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration.Loader
{
  [TestFixture]
  public class CompoundQueryFileFinderTest
  {
    private class QueryFileFinderStub : IQueryFileFinder
    {
      private readonly string[] _values;

      public QueryFileFinderStub (params string[] values)
      {
        _values = values;
      }

      public IEnumerable<string> GetQueryFilePaths ()
      {
        return _values;
      }
    }

    [Test]
    public void Initialize ()
    {
      var queryFileFinders = new[] { new Mock<IQueryFileFinder>().Object, new Mock<IQueryFileFinder>().Object };

      var compoundFactory = new CompoundQueryFileFinder(queryFileFinders);

      Assert.That(compoundFactory.QueryFileFinders, Is.Not.SameAs(queryFileFinders));
      Assert.That(compoundFactory.QueryFileFinders, Is.EqualTo(queryFileFinders));
    }

    [Test]
    public void GetQueryFilePaths ()
    {
      var queryFileFinders = new IQueryFileFinder[]
                             {
                                 new QueryFileFinderStub(@"C:\query1.xml", @"C:\query2.xml"),
                                 new QueryFileFinderStub(@"C:\Query3.xml"),
                             };
      var compoundQueryFileFinder = new CompoundQueryFileFinder(queryFileFinders);

      var expectedQueryFiles = new[]
                               {
                                   @"C:\query1.xml",
                                   @"C:\query2.xml",
                                   @"C:\Query3.xml"
                               };
      Assert.That(compoundQueryFileFinder.GetQueryFilePaths(), Is.EqualTo(expectedQueryFiles));
    }

    [Test]
    public void GetQueryFilePaths_RemovesDuplicateQueryFilePaths ()
    {
      var queryFileFinders = new IQueryFileFinder[]
                             {
                                 new QueryFileFinderStub(@"C:\query2.xml", @"C:\Query2.xml", @"C:\query2.xml"),
                                 new QueryFileFinderStub(@"C:\Query2.xml", @"C:\query2.xml"),
                             };
      var compoundQueryFileFinder = new CompoundQueryFileFinder(queryFileFinders);

      var expectedQueryFiles = new[]
                               {
                                   @"C:\query2.xml",
                               };
      Assert.That(compoundQueryFileFinder.GetQueryFilePaths(), Is.EqualTo(expectedQueryFiles));
    }
  }
}
