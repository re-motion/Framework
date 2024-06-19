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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{

[TestFixture]
public class DefaultPropertyAccessResolverTests
{
  [Test]
  public void ResolveStorageAccessForLoadingDomainObjectsByObjectID_ReturnsReadWrite ()
  {
    var storageAccessResolver = new DefaultStorageAccessResolver();
    var result = storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectsByObjectID();
    Assert.That(result, Is.EqualTo(StorageAccessType.ReadWrite));
  }

  [Test]
  public void ResolveStorageAccessForLoadingDomainObjectRelation_ReturnsReadWrite ()
  {
    var storageAccessResolver = new DefaultStorageAccessResolver();
    var result = storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectRelation();
    Assert.That(result, Is.EqualTo(StorageAccessType.ReadWrite));
  }

  [Test]
  [TestCase(QueryType.CollectionReadOnly, typeof(DomainObjectCollection))]
  [TestCase(QueryType.CollectionReadWrite, typeof(DomainObjectCollection))]
  [TestCase(QueryType.CustomReadOnly, null)]
  [TestCase(QueryType.CustomReadWrite, null)]
  [TestCase(QueryType.ScalarReadOnly, null)]
  [TestCase(QueryType.ScalarReadWrite, null)]
  public void ResolveStorageAccessForQuery_ReturnsReadWrite (QueryType queryType, Type collectionType)
  {
    var query = new Mock<IQuery>();
    query.Setup(q => q.QueryType).Throws(new InvalidOperationException("This should not be required in this method."));
    query.Setup(q => q.CollectionType).Throws(new InvalidOperationException("This should not be required in this method."));
    query.Setup(q => q.Metadata).Throws(new InvalidOperationException("This should not be required in this method."));

    var storageAccessResolver = new DefaultStorageAccessResolver();
    var result = storageAccessResolver.ResolveStorageAccessForQuery(query.Object);
    Assert.That(result, Is.EqualTo(StorageAccessType.ReadWrite));
  }
}
}
